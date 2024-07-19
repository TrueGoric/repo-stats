using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using RepoStats.Extensions;

namespace RepoStats.Logging;

public static class LoggingExtensions
{
    public static PerformanceCounter StartPerformanceMeasurement(this ILogger logger,
        string additionalCallerInfo = "",
        LogLevel logLevel = LogLevel.Information,
        [CallerMemberName] string callerMemberName = "")
        => PerformanceCounter.CreateAndStart(logger, additionalCallerInfo, logLevel, callerMemberName);
}