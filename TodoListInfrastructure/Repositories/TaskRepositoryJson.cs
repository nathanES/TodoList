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
    private readonly string _taskFilePath = $@"{Settings.JsonDataFilePathBase}tasks.json";

    public void AddTask(Task task)
    {
      List<Task> tasks = GetAllTasks().ToList();
      tasks.Add(task);
      WriteToFile(tasks);
    }

    public void DeleteTaskById(string id)
    {
      var tasks = GetAllTasks().ToList();
      int taskIndexToDelete = tasks.FindIndex(t => t.Id == id);

      if (taskIndexToDelete == -1)
        return;

      tasks.RemoveAt(taskIndexToDelete);
      WriteToFile(tasks);
    }
    public void DeleteTaskByIds(IEnumerable<string> taskIds)
    {
      List<Task> tasks = GetAllTasks().ToList();
      foreach (string taskId in taskIds)
      {
        int taskIndexToDelete = tasks.FindIndex(t => t.Id == taskId);

        if (taskIndexToDelete == -1)
          continue;

        tasks.RemoveAt(taskIndexToDelete);
      }
      WriteToFile(tasks);
    }

    public IEnumerable<Task> GetAllTasks()
    {
      if (!File.Exists(_taskFilePath))
        return new List<Task>();

      string json = File.ReadAllText(_taskFilePath);
      return JsonConvert.DeserializeObject<List<Task>>(json) ?? new List<Task>();
    }

    public Task GetTaskById(string id)
    {
      if (!File.Exists(_taskFilePath))
        return null;

      List<Task> tasks = GetAllTasks().ToList();
      return tasks.Find(t => t.Id == id);
    }

    public void UpdateTask(Task task)
    {
      List<Task> tasks = GetAllTasks().ToList();
      int taskIndexToUpdate = tasks.FindIndex(t => t.Id == task.Id);

      if (taskIndexToUpdate == -1)
        return;
      
      tasks[taskIndexToUpdate] = task;
      WriteToFile(tasks);
    }

    private void WriteToFile(List<Task> tasks)
    {
      string json = JsonConvert.SerializeObject(tasks, Formatting.Indented);
      File.WriteAllText(_taskFilePath, json);
    }
  }
}
