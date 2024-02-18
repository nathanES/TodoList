using TodoList.Application.DTOs;
using TodoList.Domain.Entities;
using TodoList.Domain.Exceptions;
using TodoList.Domain.Interfaces.Logger;
using TodoList.Domain.Interfaces.Repositories;
using static TodoList.Domain.Entities.TaskTag;

namespace TodoList.Application.Services;
public class TagService
{
    private readonly ITagRepository _tagRepository;
    private readonly ILogger _logger;

    public TagService(ITagRepository tagRepository, ILogger logger)
    {
        _tagRepository = tagRepository;
        _logger = logger;
    }

    public IEnumerable<TagDto> GetAllTags()
    {
        return _tagRepository.GetAllTags().Select(t => (TagDto)t);
    }

    public TagDto GetTagById(Guid tagId)
    {
        return (TagDto)_tagRepository.GetTagById(tagId);
    }

    public void AddTag(TagDto tagDto)
    {
        Tag tag = (Tag)tagDto;
        _ = _tagRepository.AddTag(tag);
    }
    public void UpdateTag(TagDto tagDto)
    {
        Tag tag = (Tag)tagDto;
        _ = _tagRepository.UpdateTag(tag);
    }

    // Peut être mettre le tasktagService à la place de taskTagRepository, si cela se complexifie.
    public void DeleteTagById(Guid tagId, ITaskTagRepository taskTagRepository)
    {
        UnassignAllTaskFromTag(tagId, taskTagRepository);
        _ = _tagRepository.DeleteTagById(tagId);
    }
    public void DeleteTagByIds(IEnumerable<Guid> tagIds, ITaskTagRepository taskTagRepository)
    {
        foreach (Guid tagId in tagIds)
        {
            UnassignAllTaskFromTag(tagId, taskTagRepository);
        }
        _ = _tagRepository.DeleteTagByIds(tagIds);
    }
    public void AssignTaskToTag(Guid taskId, Guid tagId, ITaskTagRepository taskTagRepository, ITaskRepository taskRepository)
    {
        if (taskTagRepository.IsRelationExists(taskId, tagId))
            return; //The task already got this tag

        Task task = taskRepository.GetTaskById(taskId);
        Tag tag = _tagRepository.GetTagById(tagId);

        if (task == null && tag == null)
            throw new NotFoundException("Task and Tag not found");
        if (task == null)
            throw new NotFoundException("Task not found");
        if (tag == null)
            throw new NotFoundException("Tag not found");

        TaskTag taskTag = new TaskTagBuilder(task, tag).Build();
        _ = taskTagRepository.AddTaskTag(taskTag);
    }
    public void AssignTasksToTag(IEnumerable<Guid> taskIds, Guid tagId, ITaskTagRepository taskTagRepository, ITaskRepository taskRepository)
    {
        foreach (Guid taskId in taskIds)
        {
            AssignTaskToTag(taskId, tagId, taskTagRepository, taskRepository);
        }
    }
    public void AssignTaskToTags(Guid taskId, IEnumerable<Guid> tagIds, ITaskTagRepository taskTagRepository, ITaskRepository taskRepository)
    {
        foreach (Guid tagId in tagIds)
        {
            AssignTaskToTag(taskId, tagId, taskTagRepository, taskRepository);
        }
    }
    public void UnassignTagFromTask(Guid tagId, Guid taskId, ITaskTagRepository taskTagRepository)
    {
        TaskTag taskTag = taskTagRepository.GetTaskTagsByTaskId(taskId).FirstOrDefault(t => t.TagId == tagId);

        if (taskTag == null)
            return; //The task doesn't have this tag

        _ = taskTagRepository.DeleteTaskTagById(taskTag.Id);
    }
    public void UnassignAllTaskFromTag(Guid tagId, ITaskTagRepository taskTagRepository)
    {
        IEnumerable<TaskTag> taskTags = taskTagRepository.GetTaskTagsByTagId(tagId);

        if (!taskTags.Any())
            return; //The tag does not have task

        _ = taskTagRepository.DeleteTaskTagByIds(taskTags.Select(tt => tt.Id));
    }
    public void UnassignAllTaskFromTags(IEnumerable<Guid> tagIds, ITaskTagRepository taskTagRepository)
    {
        IEnumerable<TaskTag> taskTags = taskTagRepository.GetTaskTagsByTagIds(tagIds);
        if (taskTags == null)
            return; //The tag does not have task

        _ = taskTagRepository.DeleteTaskTagByIds(taskTags.Select(t => t.Id));
    }
}