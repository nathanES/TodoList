using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoList.Application.Interfaces;
using TodoList.Domain.Entities;

namespace TodoList.Application.Services
{
  public class TaskService
  {
    private readonly ITaskRepository taskRepository;
    private readonly ITagRepository tagRepository;
    private readonly ITaskTagRepository taskTagRepository;

    public TaskService(ITaskRepository taskRepository, ITagRepository tagRepository,  ITaskTagRepository taskTagRepository)
        {
      this.taskRepository = taskRepository;
      this.tagRepository = tagRepository;
      this.taskTagRepository = taskTagRepository;
    }
  public void AssignTagToTask(string taskId, string tagId)
  {
    Task task = taskRepository.GetTaskById(taskId);
    Tag tag = tagRepository.GetTagById(tagId);
    
    if(task == null || tag == null)
      throw new Exception("Task or Tag not found");

      //TODO : Check if task already has tag
    TaskTag taskTag = new TaskTag
    {
      Task = task,
      Tag = tag
    };
    taskTagRepository.AddTaskTag(taskTag);
  }
  }
}
