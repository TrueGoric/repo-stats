using LibGit2Sharp;

namespace RepoStats.Git;

public class GitService : IGitService
{
    public void CloneRepository(Uri repositoryUri, string repositoryPath)
    {
        Repository.Clone(repositoryUri.AbsoluteUri, repositoryPath);
    }

    public IEnumerable<string> ListFiles(string repositoryPath, Func<string, bool>? filter = null)
    {
        using var repository = new Repository(repositoryPath);

        foreach (var indexEntry in repository.Index)
        {
            var path = Path.Join(repositoryPath, indexEntry.Path);

            if (filter is null || filter(path))
                yield return path;
        }
    }
}