using System.Diagnostics;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;

namespace RepoStats.Logging;

public class PerformanceCounter : IDisposable
{
    private readonly ILogger _logger;
    private readonly LogLevel _logLevel;
    private readonly string _additionalCallerInfo;
    private readonly string _callerMemberName;
    private readonly Stopwatch _stopwatch;

    private PerformanceCounter(
        ILogger logger,
        string additionalCallerInfo,
        LogLevel logLevel,
        string callerMemberName)
    {
        _logger = logger;
        _logLevel = logLevel;
        _callerMemberName = callerMemberName;
        _additionalCallerInfo = additionalCallerInfo;
        _stopwatch = new Stopwatch();
    }

    public static PerformanceCounter CreateAndStart(
        ILogger logger,
        string additionalCallerInfo,
        LogLevel logLevel,
        [CallerMemberName] string callerMemberName = "")
    {
        var counter = new PerformanceCounter(logger, additionalCallerInfo, logLevel, callerMemberName);

        counter._logger.Log(logLevel, "{Caller}(...) [{AdditionalCallerInfo}] - begin performance measurement",
            counter._callerMemberName, counter._additionalCallerInfo);
        counter._stopwatch.Start();

        return counter;
    }

    public void Dispose()
    {
        _stopwatch.Stop();
        _logger.Log(_logLevel, "{Caller}(...) [{AdditionalCallerInfo}] - finished in {FinishedStopwatch}",
            _callerMemberName, _additionalCallerInfo, _stopwatch.Elapsed);
    }
}