using Microsoft.Extensions.Logging;
using RepoStats.Extensions;
using RepoStats.Git;
using RepoStats.Logging;

namespace RepoStats.Generator.FilePopulators;

public class LocalGitRepositoryFilePopulator : IFilePopulator
{
    private readonly ILogger? _logger;
    private readonly IGitService _gitService;
    private readonly string _repositoryPath;
    private Func<string, bool>? _fileFilter;

    public LocalGitRepositoryFilePopulator(
        IGitService gitService,
        string repositoryPath,
        ILogger? logger = null,
        Func<string, bool>? fileFilter = default)
    {
        _gitService = gitService;
        _logger = logger;
        _repositoryPath = repositoryPath;
        _fileFilter = fileFilter;
    }

    public Task Prepare() => Task.CompletedTask;

    public IEnumerable<string> GetFiles()
    {
        using var perf = _logger?.StartPerformanceMeasurement();

        return _gitService.ListFiles(_repositoryPath, _fileFilter);
    }
}