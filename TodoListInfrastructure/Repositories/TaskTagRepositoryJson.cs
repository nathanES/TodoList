using Newtonsoft.Json;
using TodoList.Domain.Entities;
using TodoList.Domain.Exceptions;
using TodoList.Domain.Interfaces.Logger;
using TodoList.Domain.Interfaces.Repositories;

namespace TodoList.Infrastructure.Repositories;

public class TaskTagRepositoryJson : ITaskTagRepository
{
    private readonly string _taskTagFilePath = $@"{Settings.JsonDataFilePathBase}TaskTags.json";
    private List<TaskTag> _cache;
    private readonly object _fileLock = new();
    private readonly ILogger _logger;

    public TaskTagRepositoryJson(ILogger logger)
    {
        _logger = logger;
        LoadCache();
    }

    private void LoadCache()
    {
        try
        {
            if (!File.Exists(_taskTagFilePath))
            {//c'est ici que cela pose problème
                _cache = new List<TaskTag>();
                return;
            }

            string json = File.ReadAllText(_taskTagFilePath);
            _cache = JsonConvert.DeserializeObject<List<TaskTag>>(json) ?? new List<TaskTag>();

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
                File.WriteAllText(_taskTagFilePath, json);
            }
        }
        catch (Exception e)
        {
            _logger.LogException(e, "WriteToFile Impossible !", LogLevel.Error);
            throw;
        }
    }

    public bool AddTaskTag(TaskTag taskTag)
    {
        if (_cache.Any(t => t.Id == taskTag.Id))
        {
            _logger.LogCritical("AddTaskTag : DuplicateKey : {0}", taskTag.Id);
            throw new DuplicateKeyException($"Duplicate {nameof(TaskTag.Id)}, Value : {taskTag.Id}");
        }
        _cache.Add(taskTag);
        WriteToFile();
        return true;
    }

    public bool DeleteTaskTagById(Guid taskTagId)
    {
        if (DeleteTaskTagByIdInCache(taskTagId) == false)
        {
            _logger.LogWarning("DeleteTaskTagById : TaskTag Deletion Impossible : {0}", taskTagId);
            return false;
        }
        WriteToFile();
        return true;
    }
    public bool DeleteTaskTagByIds(IEnumerable<Guid> taskTagIds)
    {
        bool result = true;
        foreach (Guid taskTagId in taskTagIds)
        {
            if (DeleteTaskTagByIdInCache(taskTagId) == false)
            {
                _logger.LogWarning("DeleteTaskTagByIds : TaskTag Deletion Impossible : {0}", taskTagId);
                result = false;
            }
        }
        WriteToFile();
        return result;
    }
    private bool DeleteTaskTagByIdInCache(Guid taskTagId)
    {
        int taskTagIndexToDelete = _cache.FindIndex(t => t.Id == taskTagId);
        if (taskTagIndexToDelete == -1)
        {
            _logger.LogWarning("DeleteTaskTagByIdInCache : TaskTag not found : {0}", taskTagId);
            return false;
        }
        _cache.RemoveAt(taskTagIndexToDelete);
        return true;
    }

    public IEnumerable<TaskTag> GetAllTaskTags()
    {
        return _cache;
    }

    public TaskTag GetTaskTagById(Guid id)
    {
        return _cache.Find(t => t.Id == id) ?? TaskTag.Default;
    }

    public IEnumerable<TaskTag> GetTaskTagsByTagId(Guid tagId)
    {
        return _cache.FindAll(t => t.TagId == tagId) ?? new List<TaskTag>();
    }

    public IEnumerable<TaskTag> GetTaskTagsByTaskId(Guid taskId)
    {
        return _cache.FindAll(t => t.TaskId == taskId) ?? new List<TaskTag>();
    }

    public IEnumerable<TaskTag> GetTaskTagsByTaskIds(IEnumerable<Guid> taskIds)
    {
        foreach (Guid taskId in taskIds)
        {
            foreach (TaskTag taskTag in GetTaskTagsByTaskId(taskId))
            {
                yield return taskTag;
            }
        }
    }
    public IEnumerable<TaskTag> GetTaskTagsByTagIds(IEnumerable<Guid> tagIds)
    {
        foreach (Guid tagId in tagIds)
        {
            foreach (TaskTag taskTag in GetTaskTagsByTagId(tagId))
            {
                yield return taskTag;
            }
        }
    }

    public bool IsRelationExists(Guid taskId, Guid tagId)
    {
        return _cache.Exists(t => t.TaskId == taskId && t.TagId == tagId);
    }

    public bool UpdateTaskTag(TaskTag taskTag)
    {
        int taskTagIndexToUpdate = _cache.FindIndex(t => t.Id == taskTag.Id);
        if (taskTagIndexToUpdate == -1)
        {
            _logger.LogWarning("UpdateTaskTag : TaskTag not found : {0}", taskTag.Id);
            return false;
        }
        _cache[taskTagIndexToUpdate] = taskTag;
        WriteToFile();
        return true;
    }
}
