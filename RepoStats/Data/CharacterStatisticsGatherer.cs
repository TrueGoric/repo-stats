using RepoStats.Data;

namespace RepoStats.Generator;

public class CharacterStatisticsGatherer : ICharacterStatisticsGatherer
{
    public void GatherStatistics(StatisticsContainer statistics, ReadOnlySpan<char> chars)
    {
        for (int i = 0; i < chars.Length; i++)
        {
            var stat = new CharacterStatistics(chars[i], 1);
            
            statistics.AppendStatistics(stat);
        }
    }
}