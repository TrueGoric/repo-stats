using FluentAssertions;
using RepoStats.Data;
using RepoStats.Generator;
using Xunit;

namespace RepoStats.Tests;

public class CharacterStatisticsGathererTests
{
    private readonly CharacterStatisticsGatherer _statisticsGatherer;

    public CharacterStatisticsGathererTests()
    {
        _statisticsGatherer = new CharacterStatisticsGatherer();
    }

    [Fact]
    public void CharacterStatisticsGatherer_GathersStatistics()
    {
        var testString = "d$%&dfnsDFAdDGJM67(KJJKhjjkkkk";
        var container = new StatisticsContainer();
        
        _statisticsGatherer.GatherStatistics(container, testString);

        container['d'].Should().Be(new CharacterStatistics('d', 3));
        container['$'].Should().Be(new CharacterStatistics('$', 1));
        container['D'].Should().Be(new CharacterStatistics('D', 2));
        container['k'].Should().Be(new CharacterStatistics('k', 4));
        container['6'].Should().Be(new CharacterStatistics('6', 1));
        container['7'].Should().Be(new CharacterStatistics('7', 1));
        container['J'].Should().Be(new CharacterStatistics('J', 3));
        container['K'].Should().Be(new CharacterStatistics('K', 2));
    }
}