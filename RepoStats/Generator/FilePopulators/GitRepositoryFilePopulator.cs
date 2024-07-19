using Microsoft.Extensions.Logging;
using RepoStats.Extensions;
using RepoStats.Git;
using RepoStats.Logging;

namespace RepoStats.Generator.FilePopulators;

public class GitRepositoryFilePopulator : IFilePopulator
{
    private readonly ILogger? _logger;
    private readonly IGitService _gitService;
    private readonly Uri _repositoryUri;
    private readonly string _repositoryPath;
    private readonly Func<string, bool>? _fileFilter;

    public GitRepositoryFilePopulator(
        IGitService gitService,
        Uri repositoryUri, 
        string? repositoryPath = default,
        ILogger? logger = null,
        Func<string, bool>? fileFilter = default)
    {
        _gitService = gitService;
        _repositoryUri = repositoryUri;
        _logger = logger;
        _repositoryPath = repositoryPath ?? DirectoryHelpers.GenerateTemporaryPath();
        _fileFilter = fileFilter;
    }

    public async Task Prepare()
    {
        using var perf = _logger?.StartPerformanceMeasurement();
        
        _logger?.LogInformation("Cloning repo '{Repository}' to '{LocalPath}...", _repositoryUri, _repositoryPath);
        
        await Task.Run(() => _gitService.CloneRepository(_repositoryUri, _repositoryPath));
        
        _logger?.LogInformation("Repository '{Repository}' successfully cloned!", _repositoryUri);
    }

    public IEnumerable<string> GetFiles()
        =>_gitService.ListFiles(_repositoryPath, _fileFilter);
}