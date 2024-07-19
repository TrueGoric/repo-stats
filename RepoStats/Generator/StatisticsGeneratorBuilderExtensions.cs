using RepoStats.Generator;
using RepoStats.Generator.FilePopulators;
using RepoStats.Git;

namespace RepoStats.Extensions;

public static class StatisticsGeneratorBuilderExtensions
{
    public static StatisticsGeneratorBuilder WithGitRepositoryFilePopulator(this StatisticsGeneratorBuilder builder,
        Uri repositoryUri, Func<string, bool>? pathFilter = null, string? repositoryPath = null,
        IGitService? gitService = null)
    {
        var populator = new GitRepositoryFilePopulator(
            gitService ?? new GitService(),
            repositoryUri,
            repositoryPath,
            builder.Logger,
            pathFilter);

        return builder.WithFilePopulator(populator);
    }

    public static StatisticsGeneratorBuilder WithLocalGitRepositoryFilePopulator(
        this StatisticsGeneratorBuilder builder,
        string repositoryPath, Func<string, bool>? pathFilter = null, IGitService? gitService = null)
    {
        var populator = new LocalGitRepositoryFilePopulator(
            gitService ?? new GitService(),
            repositoryPath,
            builder.Logger,
            pathFilter);

        return builder.WithFilePopulator(populator);
    }

    public static StatisticsGeneratorBuilder WithDefaultStatisticsGatherer(this StatisticsGeneratorBuilder builder)
    {
        var gatherer = new CharacterStatisticsGatherer();

        return builder.WithCharacterStatisticsGatherer(gatherer);
    }

    public static StatisticsGeneratorBuilder WithDefaultOptions(this StatisticsGeneratorBuilder builder)
        => builder.WithOptions(new StatisticsGeneratorOptions());
}