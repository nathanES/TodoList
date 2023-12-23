namespace TodoList.Domain.Interfaces
{
  public interface ITaskRepository
  {
    IEnumerable<Task> GetAllTasks();
    Task GetTaskById(string id);
    void AddTask(Task task);
    void UpdateTask(Task task);
    void DeleteTaskById(string id);
    void DeleteTaskByIds(IEnumerable<string> taskTagIds);


  }
}
