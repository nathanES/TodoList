using TodoList.Domain.Entities;

namespace TodoList.Application.Interfaces
{
  public interface ITaskTagRepository
  {
    IEnumerable<TaskTag> GetAllTaskTags();
    TaskTag GetTaskTagById(string id);//TODO: Check if this is needed
    void AddTaskTag(TaskTag taskTag);
    void UpdateTaskTag(TaskTag taskTag);
    void DeleteTaskTag(TaskTag taskTag);
    IEnumerable<TaskTag> GetTaskTagsByTaskId(string taskId);
    IEnumerable<TaskTag> GetTaskTagsByTagId(string tagId);
  }
}
