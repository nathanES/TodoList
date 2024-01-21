using Newtonsoft.Json;
using TodoList.Domain.Entities;
using TodoList.Domain.Exceptions;
using TodoList.Domain.Interfaces;
using TodoList.Domain.Interfaces.Repositories;

namespace TodoList.Infrastructure.Repositories;

public class TaskTagRepositoryJson : ITaskTagRepository
{
    private readonly string _taskTagFilePath = $@"{Settings.JsonDataFilePathBase}taskTags.json";
    private List<TaskTag> cache;
    private readonly object fileLock = new();
    private readonly ILogger logger;

    public TaskTagRepositoryJson(ILogger logger)
    {
        this.logger = logger;
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
                File.WriteAllText(_taskTagFilePath, json);
            }
        }
        catch (Exception e)
        {
            logger.LogException(e, "WriteToFile Impossible !", LogLevel.Error);
            throw;
        }
    }

    public bool AddTaskTag(TaskTag taskTag)
    {
        if (cache.Any(t => t.Id == taskTag.Id))
        {
            logger.LogCritical("AddTaskTag : DuplicateKey : {0}", taskTag.Id);
            throw new DuplicateKeyException($"Duplicate {nameof(TaskTag.Id)}, Value : {taskTag.Id}");
        }
        cache.Add(taskTag);
        WriteToFile();
        return true;
    }

    public bool DeleteTaskTagById(string taskTagId)
    {
        if (DeleteTaskTagByIdInCache(taskTagId) == false)
        {
            logger.LogWarning("DeleteTaskTagById : TaskTag Deletion Impossible : {0}", taskTagId);
            return false;
        }
        WriteToFile();
        return true;
    }
    public bool DeleteTaskTagByIds(IEnumerable<string> taskTagIds)
    {
        bool result = true;
        foreach (string taskTagId in taskTagIds)
        {
            if (DeleteTaskTagByIdInCache(taskTagId) == false)
            {
                logger.LogWarning("DeleteTaskTagByIds : TaskTag Deletion Impossible : {0}", taskTagId);
                result = false;
            }
        }
        WriteToFile();
        return result;
    }
    private bool DeleteTaskTagByIdInCache(string taskTagId)
    {
        int taskTagIndexToDelete = cache.FindIndex(t => t.Id == taskTagId);
        if (taskTagIndexToDelete == -1)
        {
            logger.LogWarning("DeleteTaskTagByIdInCache : TaskTag not found : {0}", taskTagId);
            return false;
        }
        cache.RemoveAt(taskTagIndexToDelete);
        return true;
    }

    public IEnumerable<TaskTag> GetAllTaskTags()
    {
        return cache;
    }

    public TaskTag GetTaskTagById(string id)
    {
        return cache.Find(t => t.Id == id) ?? TaskTag.Empty;
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
    public IEnumerable<TaskTag> GetTaskTagsByTagIds(IEnumerable<string> tagIds)
    {
        foreach (string tagId in tagIds)
        {
            foreach (TaskTag taskTag in GetTaskTagsByTagId(tagId))
            {
                yield return taskTag;
            }
        }
    }

    public bool IsRelationExists(string taskId, string tagId)
    {
        return cache.Exists(t => t.TaskId == taskId && t.TagId == tagId);
    }

    public bool UpdateTaskTag(TaskTag taskTag)
    {
        int taskTagIndexToUpdate = cache.FindIndex(t => t.Id == taskTag.Id);
        if (taskTagIndexToUpdate == -1)
        {
            logger.LogWarning("UpdateTaskTag : TaskTag not found : {0}", taskTag.Id);
            return false;
        }
        cache[taskTagIndexToUpdate] = taskTag;
        WriteToFile();
        return true;
    }
}
