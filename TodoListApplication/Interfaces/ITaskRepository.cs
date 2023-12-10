using System;
namespace TodoList.Application.Interfaces
{
  public interface ITaskRepository
  {
    IEnumerable<Task> GetAllTasks();
    Task GetTaskById(string id);
    void AddTask(Task task);
    void UpdateTask(Task task);
    void DeleteTask(Task task);
  }
}
