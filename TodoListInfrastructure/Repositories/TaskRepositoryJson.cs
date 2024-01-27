using Newtonsoft.Json;
using TodoList.Domain.Exceptions;
using TodoList.Domain.Interfaces.Logger;
using TodoList.Domain.Interfaces.Repositories;

namespace TodoList.Infrastructure.Repositories;

public class TaskRepositoryJson : ITaskRepository
{
    private readonly string _taskFilePath = $@"{Settings.JsonDataFilePathBase}tasks.json";
    private List<Task> cache;
    private readonly object fileLock = new();
    private readonly ILogger logger;

    public TaskRepositoryJson(ILogger logger)
    {
        this.logger = logger;
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
            logger.LogException(e, "LoadCache Impossible !", LogLevel.Error);
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
            logger.LogException(e, "WriteToFile Impossible !", LogLevel.Error);
            throw;
        }
    }

    public bool AddTask(Task task)
    {
        if (cache.Any(t => t.Id == task.Id))
        {
            logger.LogCritical("AddTask : DuplicateKey : {0}", task.Id);
            throw new DuplicateKeyException($"Duplicate {nameof(Task.Id)}, Value : {task.Id}");
        }
        cache.Add(task);
        WriteToFile();
        return true;
    }

    public bool DeleteTaskById(string taskId)
    {
        int taskIndexToDelete = cache.FindIndex(t => t.Id == taskId);
        if (taskIndexToDelete == -1)
        {
            logger.LogWarning("DeleteTaskById : Task not found : {0}", taskId);
            return false;
        }
        cache.RemoveAt(taskIndexToDelete);
        WriteToFile();
        return true;
    }

    public bool DeleteTaskByIds(IEnumerable<string> taskIds)
    {
        bool result = true;

        foreach (string taskId in taskIds)
        {
            int taskIndexToDelete = cache.FindIndex(t => t.Id == taskId);

            if (taskIndexToDelete == -1)
            {
                logger.LogWarning("DeleteTagByIds : Tag not found : {0}", taskId);
                result = false;
                continue;
            }

            cache.RemoveAt(taskIndexToDelete);
        }
        WriteToFile();
        return result;
    }

    public IEnumerable<Task> GetAllTasks()
    {
        return cache;
    }

    public Task GetTaskById(string id)
    {
        return cache.Find(t => t.Id == id) ?? Task.Empty;
    }

    public bool UpdateTask(Task task)
    {
        int taskIndexToUpdate = cache.FindIndex(t => t.Id == task.Id);

        if (taskIndexToUpdate == -1)
        {
            logger.LogWarning("UpdateTask : Task not found : {0}", task.Id);
            return false;
        }

        cache[taskIndexToUpdate] = task;
        WriteToFile();
        return true;
    }
}
