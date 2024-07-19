using Microsoft.Extensions.Logging;
using RepoStats.Data;

namespace RepoStats.Generator;

public class StatisticsGeneratorBuilder
{
    private IFilePopulator? _filePopulator;
    private ICharacterStatisticsGatherer? _statisticsGatherer;
    private StatisticsGeneratorOptions? _options;

    public ILogger? Logger { get; }

    public StatisticsGeneratorBuilder(ILogger? logger)
    {
        Logger = logger;
    }

    public StatisticsGeneratorBuilder WithFilePopulator(IFilePopulator filePopulator)
    {
        _filePopulator = filePopulator;
        return this;
    }

    public StatisticsGeneratorBuilder WithCharacterStatisticsGatherer(ICharacterStatisticsGatherer statisticsGatherer)
    {
        _statisticsGatherer = statisticsGatherer;
        return this;
    }

    public StatisticsGeneratorBuilder WithOptions(StatisticsGeneratorOptions options)
    {
        _options = options;
        return this;
    }

    public StatisticsGenerator Build()
    {
        if (_filePopulator is null)
            throw new InvalidOperationException($"{nameof(IFilePopulator)} must be set to build {nameof(StatisticsGenerator)}!");
        if (_statisticsGatherer is null)
            throw new InvalidOperationException($"{nameof(ICharacterStatisticsGatherer)} must be set to build {nameof(StatisticsGenerator)}!");
        if (_options is null)
            _options = new StatisticsGeneratorOptions();

        return new StatisticsGenerator(_filePopulator, _statisticsGatherer, _options, Logger);
    }
}