using Moq;
using TodoList.Domain.Enum;
using TodoList.Domain.Interfaces.Logger;
using TodoList.Infrastructure.Loggers;

namespace TodoList.Infrastructure.UnitTest.Logger;
[TestClass]
public class LoggerTest
{
    [TestMethod]
    [DataRow("TestScopeState")]
    public void BeginScope_CreatesAndDisposesScope_WithCorrectLogs(string messageScope)
    {
        Mock<ILogDestination> logDestinationMock = new();
        using ILogger logger = new LoggerCustom(logDestinationMock.Object, LogLevel.Trace);

        IDisposable scope = logger.BeginScope(messageScope);
        scope.Dispose();

        logDestinationMock.Verify(ld => ld.WriteLog(It.Is<string>(s => s.Contains("Début du scope: " + messageScope))), Times.Once);
        logDestinationMock.Verify(ld => ld.WriteLog(It.Is<string>(s => s.Contains("Fin du scope: " + messageScope))), Times.Once);
    }
    [TestMethod]
    [DataRow("TestScopeState 2")]
    public void BeginScope_CreatesAndDisposesScope_WithCorrectLogs_2(string messageScope)
    {
        Mock<ILogDestination> logDestinationMock = new();
        using ILogger logger = new LoggerCustom(logDestinationMock.Object, LogLevel.Trace);

        using (logger.BeginScope(messageScope))
        {
            Console.WriteLine("helo");
        }

        logDestinationMock.Verify(ld => ld.WriteLog(It.Is<string>(s => s.Contains("Début du scope: " + messageScope))), Times.Once);
        logDestinationMock.Verify(ld => ld.WriteLog(It.Is<string>(s => s.Contains("Fin du scope: " + messageScope))), Times.Once);
    }
    [TestMethod]
    public void IsEnabled_ReturnsTrue_WhenLogLevelIsAboveOrEqualMinimumLevel()
    {
        using ILogger logger = new LoggerCustom(null, LogLevel.Warning);

        Assert.IsTrue(logger.IsEnabled(LogLevel.Warning)); // Niveau égal
        Assert.IsTrue(logger.IsEnabled(LogLevel.Error));   // Niveau supérieur
    }
    [TestMethod]
    public void IsEnabled_ReturnsFalse_WhenLogLevelIsBelowMinimumLevel()
    {
        using ILogger logger = new LoggerCustom(null, LogLevel.Warning);

        Assert.IsFalse(logger.IsEnabled(LogLevel.Information)); // Niveau inférieur
        Assert.IsFalse(logger.IsEnabled(LogLevel.Debug));       // Niveau inférieur
    }

    #region Trace
    [TestMethod]
    [DataRow("LogTrace")]
    public void LoggerCustom_WriteTrace_LogsWhenLevelIsTrace(string message)
    {
        Mock<ILogDestination> logDestinationMock = new();
        using ILogger logger = new LoggerCustom(logDestinationMock.Object, LogLevel.Trace);

        logger.LogTrace(message);

        logDestinationMock.Verify(ld => ld.WriteLog(It.Is<string>(s => s.Contains(message))), Times.Once);
    }

    [TestMethod]
    [DataRow("LogTrace", "Arg")]
    public void LoggerCustom_WriteTrace_LogsWhenLevelIsTrace(string message, string arg)
    {
        Mock<ILogDestination> logDestinationMock = new();
        using ILogger logger = new LoggerCustom(logDestinationMock.Object, LogLevel.Trace);

        logger.LogTrace(message + "{0}", arg);

        logDestinationMock.Verify(ld => ld.WriteLog(It.Is<string>(s => s.Contains(string.Format(message + "{0}", arg)))), Times.Once);
    }

    [TestMethod]
    [DataRow("LogTrace")]
    public void LoggerCustom_WriteTrace_DoesNotLogWhenLevelIsHigherThanTrace(string message)
    {
        Mock<ILogDestination> logDestinationMock = new();
        using ILogger logger = new LoggerCustom(logDestinationMock.Object, LogLevel.Debug); // Niveau supérieur à Trace

        logger.LogTrace(message);

        logDestinationMock.Verify(ld => ld.WriteLog(It.IsAny<string>()), Times.Never);
    }

    [TestMethod]
    [DataRow("LogTrace", "Arg")]
    public void LoggerCustom_WriteTrace_DoesNotLogWhenLevelIsHigherThanTrace(string message, string arg)
    {
        Mock<ILogDestination> logDestinationMock = new();
        using ILogger logger = new LoggerCustom(logDestinationMock.Object, LogLevel.Debug);

        logger.LogTrace(message + "{0}", arg);

        logDestinationMock.Verify(ld => ld.WriteLog(It.IsAny<string>()), Times.Never);
    }
    #endregion

    #region Debug
    [TestMethod]
    [DataRow("LogDebug")]
    public void LoggerCustom_WriteDebug_LogsWhenLevelIsDebug(string message)
    {
        Mock<ILogDestination> logDestinationMock = new();
        using ILogger logger = new LoggerCustom(logDestinationMock.Object, LogLevel.Debug);

        logger.LogDebug(message);

        logDestinationMock.Verify(ld => ld.WriteLog(It.Is<string>(s => s.Contains(message))), Times.Once);
    }

    [TestMethod]
    [DataRow("LogDebug", "Arg")]
    public void LoggerCustom_WriteDebug_LogsWhenLevelIsDebug(string message, string arg)
    {
        Mock<ILogDestination> logDestinationMock = new();
        using ILogger logger = new LoggerCustom(logDestinationMock.Object, LogLevel.Debug);

        logger.LogDebug(message + "{0}", arg);

        logDestinationMock.Verify(ld => ld.WriteLog(It.Is<string>(s => s.Contains(string.Format(message + "{0}", arg)))), Times.Once);
    }

    [TestMethod]
    [DataRow("LogDebug")]
    public void LoggerCustom_WriteDebug_DoesNotLogWhenLevelIsHigherThanDebug(string message)
    {
        Mock<ILogDestination> logDestinationMock = new();
        using ILogger logger = new LoggerCustom(logDestinationMock.Object, LogLevel.Information); // Niveau supérieur à Debug

        logger.LogDebug(message);

        logDestinationMock.Verify(ld => ld.WriteLog(It.IsAny<string>()), Times.Never);
    }

    [TestMethod]
    [DataRow("LogDebug", "Arg")]
    public void LoggerCustom_WriteDebug_DoesNotLogWhenLevelIsHigherThanDebug(string message, string arg)
    {
        Mock<ILogDestination> logDestinationMock = new();
        using ILogger logger = new LoggerCustom(logDestinationMock.Object, LogLevel.Information);

        logger.LogDebug(message + "{0}", arg);

        logDestinationMock.Verify(ld => ld.WriteLog(It.IsAny<string>()), Times.Never);
    }
    #endregion

    #region Information
    [TestMethod]
    [DataRow("LogInfo")]
    public void LoggerCustom_WriteInformation_LogsWhenLevelIsInformation(string message)
    {
        Mock<ILogDestination> logDestinationMock = new();
        using ILogger logger = new LoggerCustom(logDestinationMock.Object, LogLevel.Information);

        logger.LogInformation(message);

        logDestinationMock.Verify(ld => ld.WriteLog(It.Is<string>(s => s.Contains(message))), Times.Once);
    }

    [TestMethod]
    [DataRow("LogInfo", "Arg")]
    public void LoggerCustom_WriteInformation_LogsWhenLevelIsInformation(string message, string arg)
    {
        Mock<ILogDestination> logDestinationMock = new();
        using ILogger logger = new LoggerCustom(logDestinationMock.Object, LogLevel.Information);

        logger.LogInformation(message + "{0}", arg);

        logDestinationMock.Verify(ld => ld.WriteLog(It.Is<string>(s => s.Contains(string.Format(message + "{0}", arg)))), Times.Once);
    }

    [TestMethod]
    [DataRow("LogInfo")]
    public void LoggerCustom_WriteInformation_DoesNotLogWhenLevelIsHigherThanInformation(string message)
    {
        Mock<ILogDestination> logDestinationMock = new();
        using ILogger logger = new LoggerCustom(logDestinationMock.Object, LogLevel.Critical);

        logger.LogInformation(message);

        logDestinationMock.Verify(ld => ld.WriteLog(It.IsAny<string>()), Times.Never);
    }

    [TestMethod]
    [DataRow("LogInfo", "Arg")]
    public void LoggerCustom_WriteInformation_DoesNotLogWhenLevelIsHigherThanInformation(string message, string arg)
    {
        Mock<ILogDestination> logDestinationMock = new();
        using ILogger logger = new LoggerCustom(logDestinationMock.Object, LogLevel.Critical);

        logger.LogInformation(message + "{0}", arg);

        logDestinationMock.Verify(ld => ld.WriteLog(It.IsAny<string>()), Times.Never);
    }
    #endregion

    #region Warning
    [TestMethod]
    [DataRow("LogWarning")]
    public void LoggerCustom_WriteWarning_LogsWhenLevelIsWarning(string message)
    {
        Mock<ILogDestination> logDestinationMock = new();
        using ILogger logger = new LoggerCustom(logDestinationMock.Object, LogLevel.Warning);

        logger.LogWarning(message);

        logDestinationMock.Verify(ld => ld.WriteLog(It.Is<string>(s => s.Contains(message))), Times.Once);
    }

    [TestMethod]
    [DataRow("LogWarning", "Arg")]
    public void LoggerCustom_WriteWarning_LogsWhenLevelIsWarning(string message, string arg)
    {
        Mock<ILogDestination> logDestinationMock = new();
        using ILogger logger = new LoggerCustom(logDestinationMock.Object, LogLevel.Warning);

        logger.LogWarning(message + "{0}", arg);

        logDestinationMock.Verify(ld => ld.WriteLog(It.Is<string>(s => s.Contains(string.Format(message + "{0}", arg)))), Times.Once);
    }

    [TestMethod]
    [DataRow("LogWarning")]
    public void LoggerCustom_WriteWarning_DoesNotLogWhenLevelIsHigherThanWarning(string message)
    {
        Mock<ILogDestination> logDestinationMock = new();
        using ILogger logger = new LoggerCustom(logDestinationMock.Object, LogLevel.Error); // Niveau supérieur à Warning

        logger.LogWarning(message);

        logDestinationMock.Verify(ld => ld.WriteLog(It.IsAny<string>()), Times.Never);
    }

    [TestMethod]
    [DataRow("LogWarning", "Arg")]
    public void LoggerCustom_WriteWarning_DoesNotLogWhenLevelIsHigherThanWarning(string message, string arg)
    {
        Mock<ILogDestination> logDestinationMock = new();
        using ILogger logger = new LoggerCustom(logDestinationMock.Object, LogLevel.Error);

        logger.LogWarning(message + "{0}", arg);

        logDestinationMock.Verify(ld => ld.WriteLog(It.IsAny<string>()), Times.Never);
    }
    #endregion 

    #region Error
    [TestMethod]
    [DataRow("LogError")]
    public void LoggerCustom_WriteError_LogsWhenLevelIsError(string message)
    {
        Mock<ILogDestination> logDestinationMock = new();
        using ILogger logger = new LoggerCustom(logDestinationMock.Object, LogLevel.Error);

        logger.LogError(message);

        logDestinationMock.Verify(ld => ld.WriteLog(It.Is<string>(s => s.Contains(message))), Times.Once);
    }

    [TestMethod]
    [DataRow("LogError", "Arg")]
    public void LoggerCustom_WriteError_LogsWhenLevelIsError(string message, string arg)
    {
        Mock<ILogDestination> logDestinationMock = new();
        using ILogger logger = new LoggerCustom(logDestinationMock.Object, LogLevel.Error);

        logger.LogError(message + "{0}", arg);

        logDestinationMock.Verify(ld => ld.WriteLog(It.Is<string>(s => s.Contains(string.Format(message + "{0}", arg)))), Times.Once);
    }

    [TestMethod]
    [DataRow("LogError")]
    public void LoggerCustom_WriteError_DoesNotLogWhenLevelIsHigherThanError(string message)
    {
        Mock<ILogDestination> logDestinationMock = new();
        using ILogger logger = new LoggerCustom(logDestinationMock.Object, LogLevel.Critical); // Niveau supérieur à Error

        logger.LogError(message);

        logDestinationMock.Verify(ld => ld.WriteLog(It.IsAny<string>()), Times.Never);
    }

    [TestMethod]
    [DataRow("LogError", "Arg")]
    public void LoggerCustom_WriteError_DoesNotLogWhenLevelIsHigherThanError(string message, string arg)
    {
        Mock<ILogDestination> logDestinationMock = new();
        using ILogger logger = new LoggerCustom(logDestinationMock.Object, LogLevel.Critical);

        logger.LogError(message + "{0}", arg);

        logDestinationMock.Verify(ld => ld.WriteLog(It.IsAny<string>()), Times.Never);
    }
    #endregion

    #region Critical
    [TestMethod]
    [DataRow("LogCritical")]
    public void LoggerCustom_WriteCritical_LogsWhenLevelIsCritical(string message)
    {
        Mock<ILogDestination> logDestinationMock = new();
        using ILogger logger = new LoggerCustom(logDestinationMock.Object, LogLevel.Critical);

        logger.LogCritical(message);

        logDestinationMock.Verify(ld => ld.WriteLog(It.Is<string>(s => s.Contains(message))), Times.Once);
    }

    [TestMethod]
    [DataRow("LogCritical {0}", "Arg")]
    public void LoggerCustom_WriteCritical_LogsWhenLevelIsCritical(string message, string arg)
    {
        Mock<ILogDestination> logDestinationMock = new();
        using ILogger logger = new LoggerCustom(logDestinationMock.Object, LogLevel.Critical);

        logger.LogCritical(message, arg);

        logDestinationMock.Verify(ld => ld.WriteLog(It.Is<string>(s => s.Contains(string.Format(message, arg)))), Times.Once);
    }
    #endregion

    #region Exception
    [TestMethod]
    [DataRow("LogCritical", "Test exception")]
    public void LoggerCustom_WriteException_RecordsExceptionDetails(string message, string exceptionMessage)
    {
        Mock<ILogDestination> logDestinationMock = new();
        using ILogger logger = new LoggerCustom(logDestinationMock.Object, LogLevel.Information);

        InvalidOperationException exception = new(exceptionMessage);
        logger.LogException(exception, message, LogLevel.Error);

        logDestinationMock.Verify(ld => ld.WriteLog(It.Is<string>(s => s.Contains(exceptionMessage) && s.Contains(message))), Times.Once);
    }

    [TestMethod]
    [DataRow("Test exception")]

    public void LoggerCustom_LogException_WithNullMessage_RecordsExceptionDetails(string exceptionMessage)
    {
        Mock<ILogDestination> logDestinationMock = new();
        using ILogger logger = new LoggerCustom(logDestinationMock.Object, LogLevel.Information);

        InvalidOperationException exception = new(exceptionMessage);
        logger.LogException(exception, null, LogLevel.Error);

        logDestinationMock.Verify(ld => ld.WriteLog(It.Is<string>(s => s.Contains(exceptionMessage) && s.Contains("An Exception Occurred"))), Times.Once);
    }
    [TestMethod]
    public void LoggerCustom_LogException_WithNullException_DoesNotLog()
    {
        Mock<ILogDestination> logDestinationMock = new();
        using ILogger logger = new LoggerCustom(logDestinationMock.Object, LogLevel.Information);

        logger.LogException(null, "An error occurred", LogLevel.Error);

        logDestinationMock.Verify(ld => ld.WriteLog(It.IsAny<string>()), Times.Never);
    }
    [TestMethod]
    [DataRow("Error processing user {0}", "Alice", "Test exception")]
    public void LoggerCustom_LogException_WithAdditionalArgs_RecordsFormattedMessage(string customMessage, string arg, string exceptionMessage)
    {
        Mock<ILogDestination> logDestinationMock = new();
        using ILogger logger = new LoggerCustom(logDestinationMock.Object, LogLevel.Information);
        InvalidOperationException exception = new(exceptionMessage);

        logger.LogException(exception, customMessage, LogLevel.Error, arg);

        logDestinationMock.Verify(ld => ld.WriteLog(It.Is<string>(s => s.Contains(exceptionMessage) && s.Contains(string.Format(customMessage, arg)))), Times.Once);
    }

    [TestMethod]
    public void LoggerCustom_LogException_WithIncorrectFormatting_DoesNotThrowException()
    {
        Mock<ILogDestination> logDestinationMock = new();
        using ILogger logger = new LoggerCustom(logDestinationMock.Object, LogLevel.Trace);
        Exception exception = new("Test exception");

        logger.LogException(exception, "Error {0} {1}", LogLevel.Error, "arg1");

        // Vérifier que le log est enregistré, mais sans lancer d'exception pour un formatage incorrect
        logDestinationMock.Verify(ld => ld.WriteLog(It.IsAny<string>()), Times.Exactly(2));
    }

    #endregion
    #region Edge cases
    [TestMethod]
    public void LoggerCustom_LogWithNullMessage_DoesNotThrowException()
    {
        Mock<ILogDestination> logDestinationMock = new();
        using ILogger logger = new LoggerCustom(logDestinationMock.Object, LogLevel.Trace);

        logger.LogInformation(null);
        logger.LogDebug(string.Empty);

        // Vérifier que rien n'est enregistré, mais aussi qu'aucune exception n'est lancée
        logDestinationMock.Verify(ld => ld.WriteLog(It.IsAny<string>()), Times.Never);
    }

    [TestMethod]
    public void LoggerCustom_Dispose_MultipleTimes_DoesNotThrowException()
    {
        Mock<ILogDestination> logDestinationMock = new();
        LoggerCustom logger = new(logDestinationMock.Object, LogLevel.Trace);

        logger.Dispose();
        logger.Dispose(); // Deuxième appel à Dispose

        // Vérifier qu'aucune exception n'est lancée
    }

    [TestMethod]
    public void LoggerCustom_LogAfterDispose_DoesNotLog()
    {
        Mock<ILogDestination> logDestinationMock = new();
        LoggerCustom logger = new(logDestinationMock.Object, LogLevel.Trace);

        logger.Dispose();
        logger.LogInformation("Test after dispose");

        logDestinationMock.Verify(ld => ld.WriteLog(It.IsAny<string>()), Times.Never);
    }

    [TestMethod]
    public void LoggerCustom_BeginScope_WithNullState_DoesNotThrowException()
    {
        Mock<ILogDestination> logDestinationMock = new();
        using ILogger logger = new LoggerCustom(logDestinationMock.Object, LogLevel.Trace);

        using (IDisposable scope = logger.BeginScope<object>(null))
        {
            // Test avec un état de scope nul
        }

        // Vérifier que les logs de début et fin du scope sont gérés correctement même avec un état nul
        logDestinationMock.Verify(ld => ld.WriteLog(It.IsAny<string>()), Times.Exactly(2));
    }

    #endregion
    //TODO voir plus tard pour faire les tests sur les threads
}
