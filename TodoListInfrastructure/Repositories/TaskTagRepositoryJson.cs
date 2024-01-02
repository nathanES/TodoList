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
    private List<TaskTag> cache;
    private readonly object fileLock = new object();

    public TaskTagRepositoryJson()
    {
      LoadCache();
    }
    private void LoadCache()
    {
      try
      {
        if (!File.Exists(_taskTagFilePath))
        {
          cache = new List<TaskTag>();
          return;
        }

        string json = File.ReadAllText(_taskTagFilePath);
        cache = JsonConvert.DeserializeObject<List<TaskTag>>(json) ?? new List<TaskTag>();

      }
      catch (Exception e)
      {
        Console.WriteLine("LoadCache Impossible"); //TODO mettre un logger à la place
        throw;
      }
    }
    private void WriteToFile()
    {
      try
      {
        lock (fileLock)
        {
          string json = JsonConvert.SerializeObject(cache, Formatting.Indented);
          File.WriteAllText(_taskTagFilePath, json);
        }
      }
      catch (Exception e)
      {
        Console.WriteLine("WriteToFile Impossible"); //TODO mettre un logger à la place
        throw;
      }
    }

    public void AddTaskTag(TaskTag taskTag)
    {
      cache.Add(taskTag);
      WriteToFile();
    }

    public void DeleteTaskTagById(string taskTagId)
    {
      DeleteTaskTagByIdInCache(taskTagId);
      WriteToFile();
    }
    public void DeleteTaskTagByIds(IEnumerable<string> taskTagIds)
    {
      foreach (string taskTagId in taskTagIds)
      {
        DeleteTaskTagByIdInCache(taskTagId);
      }
      WriteToFile();
    }
    private void DeleteTaskTagByIdInCache(string taskTagId)
    {
      int taskTagIndexToDelete = cache.FindIndex(t => t.Id == taskTagId);
      if (taskTagIndexToDelete == -1)
        return;
      cache.RemoveAt(taskTagIndexToDelete);
    }

    public IEnumerable<TaskTag> GetAllTaskTags()
    {
      return cache;
    }

    public TaskTag GetTaskTagById(string id)
    {
      return cache.Find(t => t.Id == id);//?? TaskTag.Empty;
    }

    public IEnumerable<TaskTag> GetTaskTagsByTagId(string tagId)
    {
      return cache.FindAll(t => t.TagId == tagId) ?? new List<TaskTag>();
    }

    public IEnumerable<TaskTag> GetTaskTagsByTaskId(string taskId)
    {
      return cache.FindAll(t => t.TaskId == taskId) ?? new List<TaskTag>();
    }
    public IEnumerable<TaskTag> GetTaskTagsByTaskIds(IEnumerable<string> taskIds)
    {
      foreach (string taskId in taskIds)
      {
        foreach (TaskTag taskTag in GetTaskTagsByTaskId(taskId))
        {
          yield return taskTag;
        }
      }
    }

    public bool IsRelationExists(string taskId, string tagId)
    {
      return cache.Exists(t => t.TaskId == taskId && t.TagId == tagId);
    }

    public void UpdateTaskTag(TaskTag taskTag)
    {
      int taskTagIndexToUpdate = cache.FindIndex(t => t.Id == taskTag.Id);
      if (taskTagIndexToUpdate == -1)
        return;

      cache[taskTagIndexToUpdate] = taskTag;
      WriteToFile();
    }
  }
}
