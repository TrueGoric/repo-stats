namespace RepoStats.Generator;

public interface IFilePopulator
{
    Task Prepare();
    IEnumerable<string> GetFiles();
}