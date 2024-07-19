using System.Threading.Channels;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using RepoStats.Data;
using RepoStats.Generator;
using RepoStats.Generator.Pipeline;
using Xunit;

namespace RepoStats.Tests;

public class StatisticsGeneratorWorkerTests
{
    private readonly StatisticsGeneratorWorker _worker;
    private readonly Channel<string> _pathChannel;
    private readonly Channel<StatisticsContainer> _statisticsChannel;

    public StatisticsGeneratorWorkerTests()
    {
        var gatherer = new CharacterStatisticsGatherer();
        var loggerFactory = LoggerFactory.Create(b => { });

        _pathChannel = Channel.CreateBounded<string>(512);
        _statisticsChannel = Channel.CreateBounded<StatisticsContainer>(512);
        _worker = new StatisticsGeneratorWorker("TestWorker", loggerFactory.CreateLogger<StatisticsGeneratorWorker>(),
            gatherer, _pathChannel.Reader, _statisticsChannel.Writer);
    }

    [Fact]
    public async Task StatisticsGeneratorWorker_ShouldGenerateStatistics()
    {
        var workerTask = _worker.Run();
        var tempPath = Path.GetTempFileName();
        var testString = @"HEROD
Pour me forth wine [wine is brought.] Salomé, come drink a little wine with me.
I have here a wine that is exquisite. Cæsar himself sent it me. Dip into it thy little red lips, that I may drain the cup.
SALOMÉ
I am not thirsty, Tetrarch.
HEROD
You hear how she answers me, this daughter of yours?
HERODIAS
She does right. Why are you always gazing at her?
HEROD
Bring me ripe fruits [fruits are brought.] Salomé, come and eat fruits with me. I love to see in a fruit the mark of thy little teeth. Bite but a little of this fruit that I may eat what is left.
SALOMÉ
I am not hungry, Tetrarch.";

        File.WriteAllText(tempPath, testString);

        await _pathChannel.Writer.WriteAsync(tempPath);
        _pathChannel.Writer.Complete();
        await workerTask;

        var result = await _statisticsChannel.Reader.ReadAsync();
        _statisticsChannel.Writer.Complete();

        result.Statistics.Should().NotBeEmpty();
        result['u'].Should().Be(new CharacterStatistics('u', 16));
        result[' '].Should().Be(new CharacterStatistics(' ', 105));
        result['e'].Should().Be(new CharacterStatistics('e', 47));
    }
}