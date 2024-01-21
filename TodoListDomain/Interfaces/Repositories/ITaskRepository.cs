namespace TodoList.Domain.Interfaces.Repositories;

public interface ITaskRepository
{
    IEnumerable<Task> GetAllTasks();
    Task GetTaskById(string id);
    bool AddTask(Task task);
    bool UpdateTask(Task task);
    bool DeleteTaskById(string id);
    bool DeleteTaskByIds(IEnumerable<string> taskTagIds);
}
