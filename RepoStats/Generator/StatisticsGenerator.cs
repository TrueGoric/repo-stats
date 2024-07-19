using System.Threading.Channels;
using Microsoft.Extensions.Logging;
using RepoStats.Data;
using RepoStats.Extensions;
using RepoStats.Generator.Pipeline;
using RepoStats.Generator.PipelineBlocks;
using RepoStats.Logging;

namespace RepoStats.Generator;

public class StatisticsGenerator : IStatisticsGenerator
{
    private readonly ILogger? _logger;
    private readonly IFilePopulator _filePopulator;
    private readonly ICharacterStatisticsGatherer _statisticsGatherer;
    private readonly StatisticsGeneratorOptions _options;

    public StatisticsGenerator(IFilePopulator filePopulator, ICharacterStatisticsGatherer statisticsGatherer,
        StatisticsGeneratorOptions options, ILogger? logger)
    {
        _filePopulator = filePopulator;
        _statisticsGatherer = statisticsGatherer;
        _options = options;
        _logger = logger;
    }

    public async Task<StatisticsContainer> Generate(CancellationToken cancellationToken = default)
    {
        var channelOptions = new BoundedChannelOptions(_options.BufferSize)
        {
            FullMode = BoundedChannelFullMode.Wait,
            SingleReader = false,
            SingleWriter = false,
            AllowSynchronousContinuations = true
        };
        var pathChannel = Channel.CreateBounded<string>(channelOptions);
        var statisticsChannel = Channel.CreateBounded<StatisticsContainer>(channelOptions);

        var aggregator = new StatisticsGeneratorAggregator(_logger, statisticsChannel.Reader);
        var processor = new StatisticsGeneratorProcessor(
            pathChannel.Reader,
            statisticsChannel.Writer,
            _options.MaxParallelismDegree,
            _statisticsGatherer,
            _logger);

        await _filePopulator.Prepare();

        var processorTask = processor.RunProcessor(cancellationToken);
        var gatherTask = aggregator.RunAggregator(cancellationToken);

        return await ProcessFiles(pathChannel, processorTask, gatherTask, cancellationToken);
    }

    private async Task<StatisticsContainer> ProcessFiles(
        Channel<string> pathChannel,
        Task processorTask,
        Task<StatisticsContainer> gatherTask,
        CancellationToken cancellationToken = default)
    {
        using var perf = _logger?.StartPerformanceMeasurement("Process");
        {
            foreach (var file in _filePopulator.GetFiles())
                await pathChannel.Writer.WriteAsync(file, cancellationToken);

            pathChannel.Writer.Complete();

            await processorTask;

            var statisticsContainer = await gatherTask;

            return statisticsContainer;
        }
    }
}