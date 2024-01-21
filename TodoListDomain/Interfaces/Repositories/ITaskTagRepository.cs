using TodoList.Domain.Entities;

namespace TodoList.Domain.Interfaces.Repositories;

public interface ITaskTagRepository
{
    IEnumerable<TaskTag> GetAllTaskTags();
    TaskTag GetTaskTagById(string id);
    bool AddTaskTag(TaskTag taskTag);
    bool UpdateTaskTag(TaskTag taskTag);
    bool DeleteTaskTagById(string taskTagId);
    bool DeleteTaskTagByIds(IEnumerable<string> taskTagIds);
    IEnumerable<TaskTag> GetTaskTagsByTaskId(string taskId);
    IEnumerable<TaskTag> GetTaskTagsByTaskIds(IEnumerable<string> taskIds);
    IEnumerable<TaskTag> GetTaskTagsByTagId(string tagId);
    IEnumerable<TaskTag> GetTaskTagsByTagIds(IEnumerable<string> tagIds);
    bool IsRelationExists(string taskId, string tagId);
}
