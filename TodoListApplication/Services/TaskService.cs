using TodoList.Application.DTOs;
using TodoList.Domain.Entities;
using TodoList.Domain.Interfaces;

namespace TodoList.Application.Services;

//TODO Faire les différents services pour les Tags et les TaskTags,
//  voir si pour les taskTags on peut pas passer par le service des tags et tasks
public class TaskService
{
  private readonly ITaskRepository taskRepository;

  public TaskService(ITaskRepository taskRepository) => this.taskRepository = taskRepository;

  public IEnumerable<TaskDto> GetAllTasks() => taskRepository.GetAllTasks().Select(t => (TaskDto)t);
  public TaskDto GetTaskById(string taskId) => (TaskDto)taskRepository.GetTaskById(taskId);
  public IEnumerable<TaskDto> GetTasksByTagId(string tagId, ITaskTagRepository taskTagRepository)
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
    taskRepository.AddTask(task);
  }
  public void UpdateTask(TaskDto taskDto)
  {
    Task task = (Task)taskDto;
    taskRepository.UpdateTask(task);
  }
  public void DeleteTaskById(string taskId, ITaskTagRepository taskTagRepository)
  {
    UnassignAllTagsFromTask(taskId, taskTagRepository);
    taskRepository.DeleteTaskById(taskId);
  }
  public void DeleteTaskByIds(IEnumerable<string> taskIds, ITaskTagRepository taskTagRepository)
  {
    UnassignAllTagsFromTasks(taskIds, taskTagRepository);
    taskRepository.DeleteTaskByIds(taskIds);
  }
  public void AssignTagToTask(string taskId, string tagId, ITaskTagRepository taskTagRepository, ITagRepository tagRepository)
  {
    if (taskTagRepository.GetTaskTagsByTaskId(taskId).Any(t => t.TagId == tagId))
      return; //The task already got this tag

    Task task = taskRepository.GetTaskById(taskId);
    Tag tag = tagRepository.GetTagById(tagId);

    if (task == null || tag == null)
      throw new Exception("Task or Tag not found"); //TODO mettre une exception personnalisée

    TaskTag taskTag = new(task, tag);
    taskTagRepository.AddTaskTag(taskTag);
  }
  public void AssignTagsToTask(string taskId, IEnumerable<string> tagIds, ITaskTagRepository taskTagRepository, ITagRepository tagRepository)
  {
    foreach (string tagId in tagIds)
    {
      AssignTagToTask(taskId, tagId, taskTagRepository, tagRepository);
    }
  }
  public void AssignTagToTasks(string tagId, IEnumerable<string> taskIds, ITaskTagRepository taskTagRepository, ITagRepository tagRepository)
  {
    foreach (string taskId in taskIds)
    {
      AssignTagToTask(taskId, tagId, taskTagRepository, tagRepository);
    }
  }
  public void UnassignTagFromTask(string taskId, string tagId, ITaskTagRepository taskTagRepository)
  {
    TaskTag taskTag = taskTagRepository.GetTaskTagsByTaskId(taskId).FirstOrDefault(t => t.TagId == tagId);

    if (taskTag == null)
      return; //The task doesn't have this tag

    taskTagRepository.DeleteTaskTagById(taskTag.Id);
  }
  public void UnassignAllTagsFromTask(string taskId, ITaskTagRepository taskTagRepository)
  {
    IEnumerable<TaskTag> taskTags = taskTagRepository.GetTaskTagsByTaskId(taskId);
    if (taskTags == null)
      return;

    taskTagRepository.DeleteTaskTagByIds(taskTags.Select(t => t.Id));
  }
  public void UnassignAllTagsFromTasks(IEnumerable<string> taskIds, ITaskTagRepository taskTagRepository)
  {
    IEnumerable<TaskTag> taskTags = taskTagRepository.GetTaskTagsByTaskIds(taskIds);
    if (taskTags == null)
      return;

    taskTagRepository.DeleteTaskTagByIds(taskTags.Select(t => t.Id));
  }
}
