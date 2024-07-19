namespace RepoStats.Git;

public interface IGitService
{
    void CloneRepository(Uri repositoryUri, string repositoryPath);
    IEnumerable<string> ListFiles(string repositoryPath, Func<string, bool>? filter = null);
}