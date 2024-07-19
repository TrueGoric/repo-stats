namespace RepoStats.Data;

public readonly record struct CharacterStatistics(
    char Character,
    int Occurrences)
{
    public CharacterStatistics Join(CharacterStatistics other)
        => this with { Occurrences = Occurrences + other.Occurrences };

    public override string ToString()
        => $"Character '{Character}': {Occurrences} times";
}