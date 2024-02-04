using Newtonsoft.Json;
using TodoList.Domain.Exceptions;
using TodoList.Domain.Interfaces.Logger;
using TodoList.Domain.Interfaces.Repositories;

namespace TodoList.Infrastructure.Repositories;

public class TaskRepositoryJson : ITaskRepository
{
    private readonly string _taskFilePath = $@"{Settings.JsonDataFilePathBase}Tasks.json";
    private List<Task> _cache;
    private readonly object _fileLock = new();
    private readonly ILogger _logger;

    public TaskRepositoryJson(ILogger logger)
    {
        _logger = logger;
        LoadCache();
    }
    private void LoadCache()
    {
        try
        {
            if (!File.Exists(_taskFilePath))
            {
                _cache = new List<Task>();
                return;
            }

            string json = File.ReadAllText(_taskFilePath);
            _cache = JsonConvert.DeserializeObject<List<Task>>(json) ?? new List<Task>();
        }
        catch (Exception e)
        {
            _logger.LogException(e, "LoadCache Impossible !", LogLevel.Error);
            throw;
        }
    }
    private void WriteToFile()
    {
        try
        {
            lock (_fileLock)
            {
                string json = JsonConvert.SerializeObject(_cache, Formatting.Indented);
                File.WriteAllText(_taskFilePath, json);
            }
        }
        catch (Exception e)
        {
            _logger.LogException(e, "WriteToFile Impossible !", LogLevel.Error);
            throw;
        }
    }

    public bool AddTask(Task task)
    {
        if (_cache.Any(t => t.Id == task.Id))
        {
            _logger.LogCritical("AddTask : DuplicateKey : {0}", task.Id);
            throw new DuplicateKeyException($"Duplicate {nameof(Task.Id)}, Value : {task.Id}");
        }
        _cache.Add(task);
        WriteToFile();
        return true;
    }

    public bool DeleteTaskById(Guid taskId)
    {
        int taskIndexToDelete = _cache.FindIndex(t => t.Id == taskId);
        if (taskIndexToDelete == -1)
        {
            _logger.LogWarning("DeleteTaskById : Task not found : {0}", taskId);
            return false;
        }
        _cache.RemoveAt(taskIndexToDelete);
        WriteToFile();
        return true;
    }

    public bool DeleteTaskByIds(IEnumerable<Guid> taskIds)
    {
        bool result = true;

        foreach (Guid taskId in taskIds)
        {
            int taskIndexToDelete = _cache.FindIndex(t => t.Id == taskId);

            if (taskIndexToDelete == -1)
            {
                _logger.LogWarning("DeleteTagByIds : Tag not found : {0}", taskId);
                result = false;
                continue;
            }

            _cache.RemoveAt(taskIndexToDelete);
        }
        WriteToFile();
        return result;
    }

    public IEnumerable<Task> GetAllTasks()
    {
        return _cache;
    }

    public Task GetTaskById(Guid id)
    {
        return _cache.Find(t => t.Id == id) ?? Task.Default;
    }

    public bool UpdateTask(Task task)
    {
        int taskIndexToUpdate = _cache.FindIndex(t => t.Id == task.Id);

        if (taskIndexToUpdate == -1)
        {
            _logger.LogWarning("UpdateTask : Task not found : {0}", task.Id);
            return false;
        }

        _cache[taskIndexToUpdate] = task;
        WriteToFile();
        return true;
    }
}
