using TodoList.Domain.Entities;

namespace TodoList.Domain.Interfaces.Repositories;

public interface ITaskTagRepository
{
    IEnumerable<TaskTag> GetAllTaskTags();
    TaskTag GetTaskTagById(Guid id);
    bool AddTaskTag(TaskTag taskTag);
    bool UpdateTaskTag(TaskTag taskTag);
    bool DeleteTaskTagById(Guid taskTagId);
    bool DeleteTaskTagByIds(IEnumerable<Guid> taskTagIds);
    IEnumerable<TaskTag> GetTaskTagsByTaskId(Guid taskId);
    IEnumerable<TaskTag> GetTaskTagsByTaskIds(IEnumerable<Guid> taskIds);
    IEnumerable<TaskTag> GetTaskTagsByTagId(Guid tagId);
    IEnumerable<TaskTag> GetTaskTagsByTagIds(IEnumerable<Guid> tagIds);
    bool IsRelationExists(Guid taskId, Guid tagId);
}
