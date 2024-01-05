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
  public class TaskRepositoryJson : ITaskRepository
  {
    private readonly string _taskFilePath = $@"{Settings.JsonDataFilePathBaseSurface}tasks.json";
    private List<Task> cache;
    private readonly object fileLock = new object();
    public TaskRepositoryJson()
    {
      LoadCache();
    }
    private void LoadCache()
    {
      try
      {
        if (!File.Exists(_taskFilePath))
        {
          cache = new List<Task>();
          return;
        }

        string json = File.ReadAllText(_taskFilePath);
        cache = JsonConvert.DeserializeObject<List<Task>>(json) ?? new List<Task>();
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
          File.WriteAllText(_taskFilePath, json);
        }
      }
      catch (Exception e)
      {
        Console.WriteLine("WriteToFile Impossible"); //TODO mettre un logger à la place

        throw;
      }
    }

    public void AddTask(Task task)
    {
      cache.Add(task);
      WriteToFile();
    }

    public void DeleteTaskById(string id)
    {

      int taskIndexToDelete = cache.FindIndex(t => t.Id == id);

      if (taskIndexToDelete == -1)
        return;

      cache.RemoveAt(taskIndexToDelete);
      WriteToFile();
    }
    public void DeleteTaskByIds(IEnumerable<string> taskIds)
    {
      foreach (string taskId in taskIds)
      {
        int taskIndexToDelete = cache.FindIndex(t => t.Id == taskId);

        if (taskIndexToDelete == -1)
          continue;

        cache.RemoveAt(taskIndexToDelete);
      }
      WriteToFile();
    }

    public IEnumerable<Task> GetAllTasks()
    {
      return cache;
    }

    public Task GetTaskById(string id)
    {
      return cache.Find(t => t.Id == id);
    }

    public void UpdateTask(Task task)
    { 
      int taskIndexToUpdate = cache.FindIndex(t => t.Id == task.Id);

      if (taskIndexToUpdate == -1)
        return;

      cache[taskIndexToUpdate] = task;
      WriteToFile();
    }
  }
}
