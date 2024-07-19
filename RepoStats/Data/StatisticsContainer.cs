namespace RepoStats.Data;

public class StatisticsContainer
{
    private Dictionary<char, CharacterStatistics> _statistics = new();

    public CharacterStatistics this[char index] => _statistics[index];
    public IEnumerable<CharacterStatistics> Statistics => _statistics.Values;

    public void AppendStatistics(CharacterStatistics statistics)
    {
        if (_statistics.TryGetValue(statistics.Character, out var existingStatistics))
            _statistics[statistics.Character] = existingStatistics.Join(statistics);
        else
            _statistics.Add(statistics.Character, statistics);
    }

    public IEnumerable<CharacterStatistics> GetSortedStatisticsDescending<TOrderingKey>(
        Func<CharacterStatistics, TOrderingKey> predicate)
        => _statistics.Values.OrderByDescending(predicate);

    public void Join(StatisticsContainer otherContainer)
    {
        foreach (var statistics in otherContainer.Statistics)
        {
            AppendStatistics(statistics);
        }
    }
}