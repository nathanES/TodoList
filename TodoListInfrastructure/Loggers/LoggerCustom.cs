using TodoList.Domain.Interfaces.Logger;

namespace TodoList.Infrastructure.Loggers;
public class LoggerCustom : ILogger
{
    private readonly LogLevel _minimumLogLevel;
    public LoggerCustom(LogLevel _minimumLogLevel = LogLevel.Information)
    {
        this._minimumLogLevel = _minimumLogLevel;
    }
    public void Log(LogLevel logLevel, string message, params object[] args)
    {
        if (string.IsNullOrEmpty(message))
            return;
        if (!IsEnabled(logLevel))
            return;
        Console.WriteLine($"{DateTime.Now} [{logLevel}]: {(args == null || args.Length == 0 ? message : string.Format(message, args))}");
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
        //TODO : Vérifier si cela fonctionne bien
        string fullMessage = string.Format("{0} {1}", message ?? "An Exception Occurs : ", exception == null ? "Exception Message Empty" : exception.Message);
        Log(logLevel, fullMessage, args);
    }

    public bool IsEnabled(LogLevel level)
    {
        return level >= _minimumLogLevel;
    }

    public IDisposable BeginScope<TState>(TState state)
    {
        return new LoggingScope<TState>(state);
    }

    private class LoggingScope<TState> : IDisposable
    {
        private readonly TState _state;

        public LoggingScope(TState state)
        {
            _state = state;
            // Ici, vous pourriez écrire un log pour débuter le scope
            Console.WriteLine($"Début du scope: {_state}");
        }

        public void Dispose()
        {
            // Ici, vous pourriez écrire un log pour terminer le scope
            Console.WriteLine($"Fin du scope: {_state}");
        }
    }
}
