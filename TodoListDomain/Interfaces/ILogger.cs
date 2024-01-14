using TodoList.Domain.Enum;

namespace TodoList.Domain.Interfaces;
public interface ILogger
{
    void Log(LogLevel logLevel, string message, params object[] args);
    void LogTrace(string message, params object[] args);
    void LogDebug(string message, params object[] args);
    void LogInformation(string message, params object[] args);
    void LogWarning(string message, params object[] args);
    void LogError(string message, params object[] args);
    void LogCritical(string message, params object[] args);
    void LogException(Exception exception, string? message, LogLevel logLevel, params object[] args);
    bool IsEnabled(LogLevel level);
    IDisposable BeginScope<TState>(TState state);
}
