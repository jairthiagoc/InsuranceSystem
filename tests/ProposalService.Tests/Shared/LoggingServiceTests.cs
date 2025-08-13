using InsuranceSystem.Shared.Infrastructure.Logging;
using Microsoft.Extensions.Logging;
using FluentAssertions;

namespace ProposalService.Tests.Shared;

public class LoggingServiceTests
{
    private sealed class TestLogger<T> : ILogger<T>
    {
        public List<(LogLevel level, string message)> Entries { get; } = new();
        public IDisposable BeginScope<TState>(TState state) => NullScope.Instance;
        public bool IsEnabled(LogLevel logLevel) => true;
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            Entries.Add((logLevel, formatter(state, exception)));
        }
        private sealed class NullScope : IDisposable { public static readonly NullScope Instance = new(); public void Dispose() { } }
    }

    [Fact]
    public void LoggingMethods_ShouldWriteEntries()
    {
        var logger = new TestLogger<LoggingService>();
        var svc = new LoggingService(logger);

        svc.LogInformation("info {0}", 1);
        svc.LogWarning("warn");
        svc.LogDebug("dbg");
        svc.LogTrace("trc");
        svc.LogError("err");
        svc.LogError("err-ex", new InvalidOperationException("x"));

        // Apenas valida que os logs foram escritos (não valida sinks reais)
        logger.Entries.Should().NotBeEmpty();
        logger.Entries.Any(e => e.level == LogLevel.Information).Should().BeTrue();
        logger.Entries.Any(e => e.level == LogLevel.Warning).Should().BeTrue();
        logger.Entries.Any(e => e.level == LogLevel.Debug).Should().BeTrue();
        logger.Entries.Any(e => e.level == LogLevel.Trace).Should().BeTrue();
        logger.Entries.Any(e => e.level == LogLevel.Error).Should().BeTrue();
    }
} 