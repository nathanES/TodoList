using TodoList.Domain.Enum;
using TodoList.Domain.Interfaces;

namespace TodoList.Infrastructure;
public class LoggerCustom : ILogger
{
    public void Log(LogLevel logLevel, string message, params object[] args)
    {
        if (!IsEnabled(logLevel))
            return;
        Console.WriteLine($"{logLevel}: {string.Format(message, args)}");
    }

    public void LogTrace(string message, params object[] args)
    {
        Log(LogLevel.Trace, message, args);
    }

    public void LogDebug(string message, params object[] args)
    {
        Log(LogLevel.Debug, message, args);
    }

    public void LogInformation(string message, params object[] args)
    {
        Log(LogLevel.Information, message, args);
    }

    public void LogWarning(string message, params object[] args)
    {
        Log(LogLevel.Warning, message, args);
    }

    public void LogError(string message, params object[] args)
    {
        Log(LogLevel.Error, message, args);
    }

    public void LogCritical(string message, params object[] args)
    {
        Log(LogLevel.Critical, message, args);
    }

    public void LogException(Exception exception, string? message, LogLevel logLevel = LogLevel.Error, params object[] args)
    {
        string fullMessage = string.Format("{Message} {Exception.Message}", message ?? "An Exception Occurs : ", exception.Message);
        Log(logLevel, fullMessage, args);
    }

    public bool IsEnabled(LogLevel level)
    {
        return true;
    }

    public IDisposable BeginScope<TState>(TState state)
    {
        throw new NotImplementedException();
    }
}
