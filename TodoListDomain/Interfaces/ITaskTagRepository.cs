using TodoList.Domain.Entities;

namespace TodoList.Domain.Interfaces
{
  public interface ITaskTagRepository
  {
      IEnumerable<TaskTag> GetAllTaskTags();
      TaskTag GetTaskTagById(string id);//TODO: Check if this is needed
      void AddTaskTag(TaskTag taskTag);
      void UpdateTaskTag(TaskTag taskTag);
      void DeleteTaskTagById(string taskTagId);
      void DeleteTaskTagByIds(IEnumerable<string> taskTagIds);
      IEnumerable<TaskTag> GetTaskTagsByTaskId(string taskId);
      IEnumerable<TaskTag> GetTaskTagsByTaskIds(IEnumerable<string> taskIds);
      IEnumerable<TaskTag> GetTaskTagsByTagId(string tagId);
      bool IsRelationExists(string taskId, string tagId);
  }
}
