using System.Threading.Channels;
using Microsoft.Extensions.Logging;
using RepoStats.Data;
using RepoStats.Logging;

namespace RepoStats.Generator.PipelineBlocks;

public class StatisticsGeneratorAggregator
{
    private readonly ILogger? _logger;
    private readonly ChannelReader<StatisticsContainer> _statisticsReader;

    public StatisticsGeneratorAggregator(ILogger? logger, ChannelReader<StatisticsContainer> statisticsReader)
    {
        _logger = logger;
        _statisticsReader = statisticsReader;
    }

    public async Task<StatisticsContainer> RunAggregator(CancellationToken cancellationToken = default)
    {
        var statisticsContainer = new StatisticsContainer();

        using (var _ = _logger?.StartPerformanceMeasurement("Gathering Statistics"))
        {
            await foreach (var statistics in _statisticsReader.ReadAllAsync(cancellationToken))
            {
                statisticsContainer.Join(statistics);
            }
        }

        return statisticsContainer;
    }
}