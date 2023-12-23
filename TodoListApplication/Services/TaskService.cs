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
  public class TaskService
  {
    private readonly ITaskRepository taskRepository;
    private readonly ITagRepository tagRepository;
    private readonly ITaskTagRepository taskTagRepository;

    public TaskService(ITaskRepository taskRepository, ITagRepository tagRepository, ITaskTagRepository taskTagRepository)
    {
      this.taskRepository = taskRepository;
      this.tagRepository = tagRepository;
      this.taskTagRepository = taskTagRepository;
    }

    public IEnumerable<TaskDto> GetAllTasks()
    {
      return taskRepository.GetAllTasks().Select(t => (TaskDto)t);
    }
    public TaskDto GetTaskById(string taskId)
    {
      return (TaskDto)taskRepository.GetTaskById(taskId);
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
      taskRepository.DeleteTaskById(taskId);
    }
    public void AssignTagToTask(string taskId, string tagId)
    {
      if (taskTagRepository.GetTaskTagsByTaskId(taskId).Any(t => t.TagId == tagId))
        return; //The task already got this tag

      Task task = taskRepository.GetTaskById(taskId);
      Tag tag = tagRepository.GetTagById(tagId);

      if (task == null || tag == null)
        throw new Exception("Task or Tag not found");

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
      if(taskTags == null)
        return;

      taskTagRepository.DeleteTaskTagByIds(taskTags.Select(t=>t.Id));
    }


    //TODO ajouter les autres méthodes du repository
  }
}
