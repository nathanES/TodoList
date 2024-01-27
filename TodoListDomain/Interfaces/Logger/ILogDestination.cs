namespace TodoList.Domain.Interfaces.Logger;
public interface ILogDestination
{
    void WriteLog(string message);
}
