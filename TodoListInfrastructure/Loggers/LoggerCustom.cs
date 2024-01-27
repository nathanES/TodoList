using Newtonsoft.Json;
using System.Text;
using TodoList.Domain.Interfaces.Logger;

namespace TodoList.Infrastructure.Loggers;
public sealed class LoggerCustom : ILogger, IDisposable
{
    private readonly int _maxBufferSize = 100; //Nombre maximum de logs avant le flush
    //Todo : Est-ce que ca a un intéret de faire une queue et un stringbuilder ?
    private readonly Queue<string> _logQueue = new();
    private readonly StringBuilder _logBuilder = new();
    private readonly ILogDestination _logDestination;
    private readonly Thread _logThread;
    private readonly object _lockObject = new();
    private bool _running = true;
    private readonly LogLevel _minimumLogLevel;
    private readonly ManualResetEvent _logEvent = new(false);
    private bool _disposed = false;
    public LoggerCustom(ILogDestination logDestination, LogLevel _minimumLogLevel = LogLevel.Information)
    {
        _logDestination = logDestination;
        this._minimumLogLevel = _minimumLogLevel;
        _logThread = new Thread(ProcessLogQueue)
        {
            IsBackground = true
        };
        _logThread.Start();

        AppDomain.CurrentDomain.UnhandledException += (sender, eventArgs)
            => Dispose();
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
        if (exception == null)
        {
            Log(logLevel, "Passed exception is null", args);
            return;
        }
        var exceptionData = new
        {
            Message = message ?? "An Exception Occurred",
            ExceptionMessage = exception.Message,
            exception.StackTrace
        };
        string jsonExceptionData = JsonConvert.SerializeObject(exceptionData);

        Log(logLevel, jsonExceptionData, args);
    }

    public bool IsEnabled(LogLevel level)
    {
#if DEBUG
        return true;
#endif
        return level >= _minimumLogLevel;
    }

    public void Log(LogLevel logLevel, string message, params object[] args)
    {
        if (string.IsNullOrEmpty(message))
            return;
        if (!IsEnabled(logLevel))
            return;
        string logEntry = CreateLogEntry(logLevel, message, args);

        lock (_logQueue)
        {
            _logQueue.Enqueue(logEntry);
            if (_logQueue.Count >= _maxBufferSize)
            {
                FlushLogs();
            }
        }
        _ = _logEvent.Set();
    }

    private string CreateLogEntry(LogLevel logLevel, string message, params object[] args)
    {
        string formattedMessage = args == null || args.Length == 0
            ? message
            : string.Format(message, args);
        return $"{DateTime.Now} [{logLevel}]: {formattedMessage}";
    }
    private void WriteLog(string logEntry)
    {
        _logDestination.WriteLog(logEntry);
    }

    public void FlushLogs()
    {
        lock (_logQueue)
        {
            while (_logQueue.Count > 0)
            {
                string logEntry = _logQueue.Dequeue();
                _ = _logBuilder.AppendLine(logEntry);
            }
        }

        if (_logBuilder.Length > 0)
        {
            _logDestination.WriteLog(_logBuilder.ToString());
            _ = _logBuilder.Clear();
        }
    }

    private void ProcessLogQueue()
    {
        while (_running)
        {
            try
            {
                string? logEntry = null;

                lock (_lockObject)
                {
                    if (_logQueue.Count > 0)
                    {
                        logEntry = _logQueue.Dequeue();
                    }
                }

                if (logEntry != null)
                {
                    WriteLog(logEntry);
                }
                else
                {
                    _ = _logEvent.Reset();
                    _ = _logEvent.WaitOne();
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Erreur Log");
                //TODO faire une meilleure gestion des erreurs, l'ecrire dans un fichier de log
                throw;
            }
        }
    }
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    private void Dispose(bool disposing)
    {
        if (_disposed)
            return;
        if (disposing)
        {        // Indiquer au thread de logging de terminer son exécution
            _running = false;
            // Réveiller le thread s'il est en attente
            _ = _logEvent.Set();
            // Attendre que le thread de logging ait terminé
            _logThread.Join();
            FlushLogs();
        }
        _disposed = true;
    }

    public IDisposable BeginScope<TState>(TState state)
    {
        return new LoggingScope<TState>(this, state);
    }

    private class LoggingScope<TState> : IDisposable
    {
        private readonly TState _state;
        private readonly ILogger _logger;

        public LoggingScope(ILogger logger, TState state)
        {
            _state = state;
            _logger = logger;
            _logger.LogTrace("Début du scope: {0}", _state);
        }

        public void Dispose()
        {
            _logger.LogTrace("Fin du scope: {0}", _state);
        }
    }
}

