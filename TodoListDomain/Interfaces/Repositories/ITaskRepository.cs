namespace TodoList.Domain.Interfaces.Repositories;

public interface ITaskRepository
{
    IEnumerable<Task> GetAllTasks();
    Task GetTaskById(Guid id);
    bool AddTask(Task task);
    bool UpdateTask(Task task);
    bool DeleteTaskById(Guid id);
    bool DeleteTaskByIds(IEnumerable<Guid> taskTagIds);
}
