namespace TodoList.Domain.Interfaces.Logger;
public interface ILogDestination
{
    void WriteLog(string message);
    System.Threading.Tasks.Task WriteLogAsync(string message);
}
