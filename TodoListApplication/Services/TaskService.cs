using TodoList.Application.DTOs;
using TodoList.Domain.Entities;
using TodoList.Domain.Exceptions;
using TodoList.Domain.Interfaces.Logger;
using TodoList.Domain.Interfaces.Repositories;
using static TodoList.Domain.Entities.TaskTag;

namespace TodoList.Application.Services;

public class TaskService
{
    private readonly ITaskRepository _taskRepository;
    private readonly ILogger _logger;

    public TaskService(ITaskRepository taskRepository, ILogger logger)
    {
        _taskRepository = taskRepository;
        _logger = logger;
    }

    public IEnumerable<TaskDto> GetAllTasks()
    {
        return _taskRepository.GetAllTasks().Select(t => (TaskDto)t);
    }

    public TaskDto GetTaskById(Guid taskId)
    {
        return (TaskDto)_taskRepository.GetTaskById(taskId);
    }

    public IEnumerable<TaskDto> GetTasksByTagId(Guid tagId, ITaskTagRepository taskTagRepository)
    {
        IEnumerable<TaskTag> taskTags = taskTagRepository.GetTaskTagsByTagId(tagId);
        if (taskTags == null)
            yield break;

        foreach (TaskTag taskTag in taskTags)
        {
            yield return (TaskDto)taskTag.Task;
        }
    }
    public void AddTask(TaskDto taskDto)
    {
        Task task = (Task)taskDto;
        _ = _taskRepository.AddTask(task);
    }
    public void UpdateTask(TaskDto taskDto)
    {
        Task task = (Task)taskDto;
        _ = _taskRepository.UpdateTask(task);
    }
    public void DeleteTaskById(Guid taskId, ITaskTagRepository taskTagRepository)
    {
        UnassignAllTagsFromTask(taskId, taskTagRepository);
        _ = _taskRepository.DeleteTaskById(taskId);
    }
    public void DeleteTaskByIds(IEnumerable<Guid> taskIds, ITaskTagRepository taskTagRepository)
    {
        UnassignAllTagsFromTasks(taskIds, taskTagRepository);
        _ = _taskRepository.DeleteTaskByIds(taskIds);
    }
    public void AssignTagToTask(Guid taskId, Guid tagId, ITaskTagRepository taskTagRepository, ITagRepository tagRepository)
    {
        if (IsTagAssignedToTask(taskId, tagId, taskTagRepository))
            return; //The task already got this tag

        Task task = _taskRepository.GetTaskById(taskId);
        Tag tag = tagRepository.GetTagById(tagId);

        if (task == null && tag == null)
            throw new NotFoundException("Task and Tag not found");
        if (task == null)
            throw new NotFoundException("Task not found");
        if (tag == null)
            throw new NotFoundException("Tag not found");

        TaskTag taskTag = new TaskTagBuilder(task, tag).Build();
        _ = taskTagRepository.AddTaskTag(taskTag);
    }
    public void AssignTagsToTask(Guid taskId, IEnumerable<Guid> tagIds, ITaskTagRepository taskTagRepository, ITagRepository tagRepository)
    {
        foreach (Guid tagId in tagIds)
        {
            AssignTagToTask(taskId, tagId, taskTagRepository, tagRepository);
        }
    }
    public void AssignTagToTasks(Guid tagId, IEnumerable<Guid> taskIds, ITaskTagRepository taskTagRepository, ITagRepository tagRepository)
    {
        foreach (Guid taskId in taskIds)
        {
            AssignTagToTask(taskId, tagId, taskTagRepository, tagRepository);
        }
    }
    public void UnassignTagFromTask(Guid taskId, Guid tagId, ITaskTagRepository taskTagRepository)
    {
        TaskTag taskTag = taskTagRepository.GetTaskTagsByTaskId(taskId).FirstOrDefault(t => t.TagId == tagId);

        if (taskTag == null)
            return; //The task doesn't have this tag

        _ = taskTagRepository.DeleteTaskTagById(taskTag.Id);
    }
    public void UnassignAllTagsFromTask(Guid taskId, ITaskTagRepository taskTagRepository)
    {
        IEnumerable<TaskTag> taskTags = taskTagRepository.GetTaskTagsByTaskId(taskId);
        if (taskTags == null)
            return;

        _ = taskTagRepository.DeleteTaskTagByIds(taskTags.Select(t => t.Id));
    }
    public void UnassignAllTagsFromTasks(IEnumerable<Guid> taskIds, ITaskTagRepository taskTagRepository)
    {
        IEnumerable<TaskTag> taskTags = taskTagRepository.GetTaskTagsByTaskIds(taskIds);
        if (taskTags == null)
            return;

        _ = taskTagRepository.DeleteTaskTagByIds(taskTags.Select(t => t.Id));
    }
    private bool IsTagAssignedToTask(Guid taskId, Guid tagId, ITaskTagRepository taskTagRepository)
    {
        return taskTagRepository.GetTaskTagsByTaskId(taskId).Any(t => t.TagId == tagId);
    }
}
