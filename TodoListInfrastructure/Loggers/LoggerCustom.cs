using Newtonsoft.Json;
using System.Text;
using TodoList.Domain.Interfaces.Logger;

namespace TodoList.Infrastructure.Loggers;
public sealed class LoggerCustom : ILogger
{
    //Gestion du stockage des logs
    private readonly int _maxBufferSize = 100; //Nombre maximum de logs avant le flush
    private readonly Queue<string> _logQueue = new();
    private readonly StringBuilder _logBuilder = new();

    //Gestion des threads de logging
    private readonly Thread _logThread;
    private bool _running = true;
    private readonly ManualResetEvent _logEvent = new(false);//Indique au thread de logging qu'il y a des logs à traiter

    //Gestion de la suppression de l'objet
    private bool _disposed = false;

    //Gestion des appels concurrents
    private readonly object _lockObject = new();

    private readonly ILogDestination _logDestination;
    private readonly LogLevel _minimumLogLevel;

    public LoggerCustom(ILogDestination logDestination, LogLevel minimumLogLevel = LogLevel.Information)
    {
        _logDestination = logDestination;
        _minimumLogLevel = minimumLogLevel;
        _logThread = new Thread(ProcessLogQueue)
        {
            IsBackground = true
        };
        _logThread.Start();

        AppDomain.CurrentDomain.UnhandledException += (sender, eventArgs)
            => Dispose();
    }
    #region Logs
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
            return;
        if (!TryFormatMessage(ref message, args))
        {
            string argsEntry = $"Arguments: {string.Join(", ", args.Select(arg => arg?.ToString() ?? "null"))}";
            message = "Original message: " + message + ",  " + argsEntry;
            Log(logLevel, message);
            return;
        }

        var exceptionData = new
        {
            Message = message ?? "An Exception Occurred",
            ExceptionType = exception.GetType().FullName,
            ExceptionMessage = exception.Message,
            exception.StackTrace,
        };
        string jsonExceptionData = JsonConvert.SerializeObject(exceptionData);

        Log(logLevel, jsonExceptionData, args);
    }

    public void Log(LogLevel logLevel, string message, params object[] args)
    {
        if (string.IsNullOrEmpty(message))
            return;
        if (!IsEnabled(logLevel))
            return;
        string logEntry = CreateLogEntry(logLevel, message, args);

        AddLogToQueue(logEntry);

    }
    #endregion

    public bool IsEnabled(LogLevel level)
    {
        //#if DEBUG
        //        return true;
        //#endif
        return level >= _minimumLogLevel;
    }

    #region Formatage des logs
    private string CreateLogEntry(LogLevel logLevel, string message, params object[] args)
    {
        if (!TryFormatMessage(ref message, args))
        {
            string argsEntry = $"Arguments: {string.Join(", ", args.Select(arg => arg?.ToString() ?? "null"))}";
            message = "Original message: " + message + ",  " + argsEntry;
        }

        return $"{DateTime.Now} [{logLevel}]: {message}";
    }
    private bool TryFormatMessage(ref string message, object[] args)
    {
        try
        {
            message = FormatMessage(message, args);
            return true;
        }
        catch (FormatException formatException)
        {
            LogFormatException(formatException);
            return false;
        }
    }
    private string FormatMessage(string message, object[] args)
    {
        return args == null || args.Length == 0 ? message : string.Format(message, args);
    }
    private void LogFormatException(FormatException formatException)
    {
        string errorEntry = $"{DateTime.Now} [{LogLevel.Error}]: Format exception: {formatException.Message}, StackTrace : {formatException.StackTrace}";
        AddLogToQueue(errorEntry);
    }
    #endregion

    #region Ecriture des logs
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
    private void AddLogToQueue(string logEntry)
    {
        lock (_lockObject)
        {
            _logQueue.Enqueue(logEntry);
            if (_logQueue.Count >= _maxBufferSize)
            {
                FlushLogs();
            }
        }
        _ = _logEvent.Set();
    }
    private void WriteLog(string logEntry)
    {
        _logDestination.WriteLog(logEntry);
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
    #endregion

    #region Liberation ressources
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
        {
            // Indiquer au thread de logging de terminer son exécution
            _running = false;
            // Réveiller le thread s'il est en attente
            _ = _logEvent.Set();
            // Attendre que le thread de logging ait terminé
            _logThread.Join();
            FlushLogs();
        }
        _disposed = true;
    }
    #endregion

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
            _logger.LogInformation("Début du scope: {0}", _state);
        }

        public void Dispose()
        {
            _logger.LogInformation("Fin du scope: {0}", _state);
        }
    }
}

