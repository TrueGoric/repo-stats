using RepoStats.Data;

namespace RepoStats.Generator;

public interface IStatisticsGenerator
{
    Task<StatisticsContainer> Generate(CancellationToken cancellationToken = default);
}