namespace RepoStats;

public static class DirectoryHelpers
{
    public static string GenerateTemporaryPath()
    {
        var tempDirName = Guid.NewGuid().ToString();
        var tempPath = Path.Combine(Path.GetTempPath(), tempDirName);

        return tempPath;
    }
}