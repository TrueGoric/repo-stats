using Microsoft.Extensions.Logging;
using RepoStats.Extensions;
using RepoStats.Generator;

namespace RepoStats;

class Program
{
    static async Task Main(string[] args)
    {
        var loggerFactory = LoggerFactory.Create(f => f.AddConsole());
        var repositoryUri = new Uri("https://github.com/lodash/lodash.git");
        var filePredicate = (string s) =>
            s.EndsWith(".js") || s.EndsWith(".ts") || s.EndsWith(".jsx") || s.EndsWith(".tsx");

        var generator = new StatisticsGeneratorBuilder(loggerFactory.CreateLogger("Generator"))
            .WithDefaultOptions()
            .WithDefaultStatisticsGatherer()
            .WithGitRepositoryFilePopulator(repositoryUri, filePredicate)
            .Build();

        var statistics = await generator.Generate();
        var mostUsed = statistics.GetSortedStatisticsDescending(c => c.Occurrences).Take(20);

        foreach (var stats in mostUsed)
        {
            Console.WriteLine(stats);
        }
    }
}