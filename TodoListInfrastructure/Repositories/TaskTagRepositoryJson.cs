using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoList.Domain.Entities;
using TodoList.Domain.Interfaces;

namespace TodoList.Infrastructure.Repositories
{
  public class TaskTagRepositoryJson : ITaskTagRepository
  {
    private readonly string _taskTagFilePath = $@"{Settings.JsonDataFilePathBase}taskTags.json";

    public void AddTaskTag(TaskTag taskTag)
    {
      List<TaskTag> taskTags = GetAllTaskTags().ToList();
      taskTags.Add(taskTag);
      WriteToFile(taskTags);
    }

    public void DeleteTaskTag(TaskTag taskTag)
    {
      List<TaskTag> taskTags = GetAllTaskTags().ToList();
      int taskTagIndexToDelete = taskTags.FindIndex(t=>t.Id == taskTag.Id);
      
      if(taskTagIndexToDelete == -1)
        return;
      
      taskTags.RemoveAt(taskTagIndexToDelete);
      WriteToFile(taskTags);
    }

    public IEnumerable<TaskTag> GetAllTaskTags()
    {
      if(!File.Exists(_taskTagFilePath))
        return new List<TaskTag>();
      
      string json = File.ReadAllText(_taskTagFilePath);
      return JsonConvert.DeserializeObject<IEnumerable<TaskTag>>(json)?? new List<TaskTag>();
    }

    public TaskTag GetTaskTagById(string id)
    {
      if (!File.Exists(_taskTagFilePath))
        return null;//TaskTag.Empty;

        List<TaskTag> taskTags = GetAllTaskTags().ToList();
      return taskTags.Find(t => t.Id == id);//?? TaskTag.Empty;
    }

    public IEnumerable<TaskTag> GetTaskTagsByTagId(string tagId)
    {
      if(!File.Exists(_taskTagFilePath))
        return new List<TaskTag>();

      List<TaskTag> taskTags = GetAllTaskTags().ToList();
      return taskTags.FindAll(t=>t.TagId == tagId)?? new List<TaskTag>();
    }

    public IEnumerable<TaskTag> GetTaskTagsByTaskId(string taskId)
    {
    if(!File.Exists(_taskTagFilePath))
        return new List<TaskTag>();

      List<TaskTag> taskTags = GetAllTaskTags().ToList();
      return taskTags.FindAll(t=>t.TaskId == taskId)?? new List<TaskTag>();
    }

    public bool IsRelationExists(string taskId, string tagId)
    {
     if(!File.Exists(_taskTagFilePath))
        return false;

      List<TaskTag> taskTags = GetAllTaskTags().ToList();
      return taskTags.Exists(t=>t.TaskId == taskId && t.TagId == tagId);
    }

    public void UpdateTaskTag(TaskTag taskTag)
    {
      List<TaskTag> taskTags = GetAllTaskTags().ToList();
      int taskTagIndexToUpdate = taskTags.FindIndex(t=>t.Id == taskTag.Id);
      if(taskTagIndexToUpdate == -1)
        return;

      taskTags[taskTagIndexToUpdate] = taskTag;
      WriteToFile(taskTags);
    }

    private void WriteToFile(List<TaskTag> taskTags)
    {
      string json = JsonConvert.SerializeObject(taskTags, Formatting.Indented);
      File.WriteAllText(_taskTagFilePath, json);
    }
  }
}
