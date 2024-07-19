using System.Threading.Channels;
using Microsoft.Extensions.Logging;
using RepoStats.Data;

namespace RepoStats.Generator.Pipeline;

public class StatisticsGeneratorProcessor
{
    private readonly ILogger? _logger;

    private readonly int _workerCount;
    private readonly ICharacterStatisticsGatherer _statisticsGatherer;
    private readonly ChannelReader<string> _pathReader;
    private readonly ChannelWriter<StatisticsContainer> _statisticsWriter;

    public StatisticsGeneratorProcessor(
        ChannelReader<string> pathReader,
        ChannelWriter<StatisticsContainer> statisticsWriter,
        int workerCount,
        ICharacterStatisticsGatherer statisticsGatherer,
        ILogger? logger)
    {
        _workerCount = workerCount;
        _statisticsGatherer = statisticsGatherer;
        _logger = logger;

        _pathReader = pathReader;
        _statisticsWriter = statisticsWriter;
    }

    public Task RunProcessor(CancellationToken cancellationToken = default)
    {
        var workers = Enumerable
            .Range(0, _workerCount)
            .Select(i => new StatisticsGeneratorWorker(
                $"Worker #{i}",
                _logger,
                _statisticsGatherer,
                _pathReader,
                _statisticsWriter))
            .Select(w => w.Run(cancellationToken));

        var completionTask = Task.Run(async () =>
        {
            await Task.WhenAll(workers);

            _statisticsWriter.Complete();
        }, cancellationToken);

        return completionTask;
    }
}