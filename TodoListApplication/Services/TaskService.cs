using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoList.Domain.Interfaces;
using TodoList.Domain.Entities;
using TodoList.Application.DTOs;

namespace TodoList.Application.Services
{
//TODO Faire les différents services pour les Tags et les TaskTags,
//  voir si pour les taskTags on peut pas passer par le service des tags et tasks
  public class TaskService
  {
    private readonly ITaskRepository taskRepository;
    private readonly ITagRepository tagRepository;
    private readonly ITaskTagRepository taskTagRepository;

    public TaskService(ITaskRepository taskRepository, ITagRepository tagRepository, ITaskTagRepository taskTagRepository)
    {
      this.taskRepository = taskRepository;
      this.tagRepository = tagRepository;//TODO : cf en dessous
      this.taskTagRepository = taskTagRepository;//TODO : peut-être voir pour les mettre en paramètre des
                                                 //méthodes et non pas dans le constructeur car toutes les méthodes n'en ont pas besoin

    }

    public IEnumerable<TaskDto> GetAllTasks()
    {
      return taskRepository.GetAllTasks().Select(t => (TaskDto)t);
    }
    public TaskDto GetTaskById(string taskId)
    {
      return (TaskDto)taskRepository.GetTaskById(taskId);
    }
    public IEnumerable<TaskDto> GetTasksByTagId(string tagId)
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
    public void DeleteTaskById(string taskId)
    {
      UnassignAllTagsFromTask(taskId);
      taskRepository.DeleteTaskById(taskId);
    }
    public void DeleteTaskByIds(IEnumerable<string> taskIds)
    {
      UnassignAllTagsFromTasks(taskIds);
      taskRepository.DeleteTaskByIds(taskIds);
    }
    public void AssignTagToTask(string taskId, string tagId)
    {
      if (taskTagRepository.GetTaskTagsByTaskId(taskId).Any(t => t.TagId == tagId))
        return; //The task already got this tag

      Task task = taskRepository.GetTaskById(taskId);
      Tag tag = tagRepository.GetTagById(tagId);

      if (task == null || tag == null)
        throw new Exception("Task or Tag not found"); //TODO mettre une exception personnalisée

      TaskTag taskTag = new TaskTag(task, tag);
      taskTagRepository.AddTaskTag(taskTag);
    }
    public void UnassignTagFromTask(string taskId, string tagId)
    {
      TaskTag taskTag = taskTagRepository.GetTaskTagsByTaskId(taskId).FirstOrDefault(t => t.TagId == tagId);

      if (taskTag == null)
        return; //The task doesn't have this tag

      taskTagRepository.DeleteTaskTagById(taskTag.Id);
    }
    public void UnassignAllTagsFromTask(string taskId)
    {
      IEnumerable<TaskTag> taskTags = taskTagRepository.GetTaskTagsByTaskId(taskId);
      if (taskTags == null)
        return;

      taskTagRepository.DeleteTaskTagByIds(taskTags.Select(t => t.Id));
    }
    public void UnassignAllTagsFromTasks(IEnumerable<string> taskIds)
    {
      IEnumerable<TaskTag> taskTags = taskTagRepository.GetTaskTagsByTaskIds(taskIds);
      if (taskTags == null)
        return;

      taskTagRepository.DeleteTaskTagByIds(taskTags.Select(t => t.Id));
    }
  }
}
