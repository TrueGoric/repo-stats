using FluentAssertions;
using RepoStats.Git;
using Xunit;

namespace RepoStats.Tests;

public class GitServiceTests
{
    private readonly GitService _gitService;

    public GitServiceTests()
    {
        _gitService = new GitService();
    }

    [Theory]
    [InlineData("https://github.com/TrueGoric/k8s-insider.git")]
    public void GitService_ClonesRepositorySuccessfully(string repositoryAddress)
    {
        var repositoryUri = new Uri(repositoryAddress);
        var tempPath = DirectoryHelpers.GenerateTemporaryPath();

        _gitService.CloneRepository(repositoryUri, tempPath);

        var clonedFiles = Directory.EnumerateFiles(tempPath);
        clonedFiles.Should().NotBeEmpty();
    }
    
    [Theory]
    [InlineData("https://github.com/TrueGoric/k8s-insider.git", "README.md")]
    public void GitService_ListsFilesInRepository(string repositoryAddress, string filterFileName)
    {
        var repositoryUri = new Uri(repositoryAddress);
        var tempPath = DirectoryHelpers.GenerateTemporaryPath();

        _gitService.CloneRepository(repositoryUri, tempPath);
        
        var files = _gitService
            .ListFiles(tempPath, p => p.EndsWith(filterFileName))
            .ToList();

        files.Should().NotBeEmpty();
        files.Should().AllSatisfy(f => File.Exists(f));
    }
}