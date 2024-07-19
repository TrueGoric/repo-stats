namespace RepoStats.Data;

public interface ICharacterStatisticsGatherer
{
    public void GatherStatistics(StatisticsContainer statistics, ReadOnlySpan<char> chars);
}