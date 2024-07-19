using System.Threading.Channels;
using Microsoft.Extensions.Logging;
using RepoStats.Data;

namespace RepoStats.Generator.Pipeline;

public class StatisticsGeneratorWorker
{
    private readonly string _workerName;

    private readonly ILogger? _logger;
    private readonly ICharacterStatisticsGatherer _characterStatisticsGatherer;
    private readonly ChannelReader<string> _pathReader;
    private readonly ChannelWriter<StatisticsContainer> _statisticsWriter;

    public StatisticsGeneratorWorker(
        string workerName,
        ILogger? logger,
        ICharacterStatisticsGatherer characterStatisticsGatherer,
        ChannelReader<string> pathReader,
        ChannelWriter<StatisticsContainer> statisticsWriter)
    {
        _workerName = workerName;
        _logger = logger;
        _characterStatisticsGatherer = characterStatisticsGatherer;
        _pathReader = pathReader;
        _statisticsWriter = statisticsWriter;
    }

    public async Task Run(CancellationToken cancellationToken = default)
    {
        _logger?.LogDebug("Worker '{WorkerName}' initialized...", _workerName);

        await foreach (var path in _pathReader.ReadAllAsync(cancellationToken))
        {
            _logger?.LogDebug("Worker {WorkerName} processing '{FilePath}'...", _workerName, path);

            try
            {
                await ProcessFile(cancellationToken, path);
            }
            catch (Exception e)
            {
                _logger?.LogError(e,
                    "Worker '{WorkerName}' encountered an error while processing '{FilePath}' - {ErrorMessage}",
                    _workerName, path, e.Message);
            }
        }

        _logger?.LogDebug("Worker '{WorkerName}' shutting down...", _workerName);
    }

    private async Task ProcessFile(CancellationToken cancellationToken, string path)
    {
        using var stream = File.OpenText(path);
        var statistics = new StatisticsContainer();

        while (!stream.EndOfStream)
        {
            var line = await stream.ReadLineAsync(cancellationToken);
            if (line is null)
                break;

            _characterStatisticsGatherer.GatherStatistics(statistics, line);
        }

        await _statisticsWriter.WriteAsync(statistics, cancellationToken);
    }
}