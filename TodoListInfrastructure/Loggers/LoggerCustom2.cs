using Newtonsoft.Json;
using System.Text;
using TodoList.Domain.Interfaces.Logger;

namespace TodoList.Infrastructure.Loggers;
public class LoggerCustom2 : ILogger
{
    //TODO voir pour qu'il s'autoFlush quand le traitement est terminé
    private readonly int _maxBufferSize = 100; //Nombre maximum de logs avant le flush
    private readonly Queue<string> _logQueue = new();
    private readonly StringBuilder _logBuilder = new();
    private readonly ILogDestination _logDestination;
    private readonly Thread _logThread;
    private readonly object _lockObject = new();
    private bool _running = true;
    private readonly LogLevel _minimumLogLevel;
    private readonly ManualResetEvent _logEvent = new(false);

    public LoggerCustom2(ILogDestination logDestination, LogLevel _minimumLogLevel = LogLevel.Information)
    {
        _logDestination = logDestination;
        this._minimumLogLevel = _minimumLogLevel;
        _logThread = new Thread(ProcessLogQueue)
        {
            IsBackground = true
        };
        _logThread.Start();
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

    private void FlushLogs()
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

    private async System.Threading.Tasks.Task FlushLogsAsync()
    {
        string? logsToSend = null;

        lock (_logQueue)
        {
            while (_logQueue.Count > 0)
            {
                string logEntry = _logQueue.Dequeue();
                _ = _logBuilder.AppendLine(logEntry);
            }

            if (_logBuilder.Length > 0)
            {
                logsToSend = _logBuilder.ToString();
                _ = _logBuilder.Clear();
            }
        }

        if (logsToSend != null)
        {
            await _logDestination.WriteLogAsync(logsToSend);
        }
    }

    private void ProcessLogQueue()
    {
        while (_running)
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
    }

    public void Dispose()
    {
        //TODO Voir pour logger les logs restantes
        _running = false;
        _logThread.Join(); // Attend que le thread de log termine
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

