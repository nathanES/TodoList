using TodoList.Domain.Interfaces.Logger;

namespace TodoList.Infrastructure.Loggers;
public class ConsoleLogDestination : ILogDestination
{
    public void WriteLog(string message)
    {
        Console.WriteLine(message);
    }
    public System.Threading.Tasks.Task WriteLogAsync(string message)
    {
        return System.Threading.Tasks.Task.Run(() => Console.WriteLine(message));
    }
}
