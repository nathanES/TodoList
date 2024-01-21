using Newtonsoft.Json;
using TodoList.Domain.Entities;
using TodoList.Domain.Exceptions;
using TodoList.Domain.Interfaces;
using TodoList.Domain.Interfaces.Repositories;

namespace TodoList.Infrastructure.Repositories;

public class TagRepositoryJson : ITagRepository
{
    private readonly string _tagsFilePath = $@"{Settings.JsonDataFilePathBase}tags.json";
    private List<Tag> cache;
    private readonly object fileLock = new();
    private readonly ILogger logger;

    public TagRepositoryJson(ILogger logger)
    {
        this.logger = logger;
        LoadCache();
    }
    private void LoadCache()
    {
        try
        {
            if (!File.Exists(_tagsFilePath))
            {
                cache = new List<Tag>();
                return;
            }

            string json = File.ReadAllText(_tagsFilePath);
            cache = JsonConvert.DeserializeObject<List<Tag>>(json) ?? new List<Tag>();
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
                File.WriteAllText(_tagsFilePath, json);
            }
        }
        catch (Exception e)
        {
            logger.LogException(e, "WriteToFile Impossible !", LogLevel.Error);
            throw;
        }
    }

    public bool AddTag(Tag tag)
    {
        if (cache.Any(t => t.Id == tag.Id))
        {
            logger.LogCritical("AddTag : DuplicateKey : {0}", tag.Id);
            throw new DuplicateKeyException($"Duplicate {nameof(Tag.Id)}, Value : {tag.Id}");
        }
        cache.Add(tag);
        WriteToFile();
        return true;
    }

    public bool DeleteTagById(string tagId)
    {
        int tagIndexToDelete = cache.FindIndex(t => t.Id == tagId);

        if (tagIndexToDelete == -1)
        {
            logger.LogWarning("DeleteTagById : Tag not found : {0}", tagId);
            return false;
        }

        cache.RemoveAt(tagIndexToDelete);
        WriteToFile();
        return true;
    }

    public bool DeleteTagByIds(IEnumerable<string> tagIds)
    {
        bool result = true;
        foreach (string tagId in tagIds)
        {
            int tagIndexToDelete = cache.FindIndex(t => t.Id == tagId);

            if (tagIndexToDelete == -1)
            {
                logger.LogWarning("DeleteTagByIds : Tag not found : {0}", tagId);
                result = false;
                continue;
            }
            cache.RemoveAt(tagIndexToDelete);
        }
        WriteToFile();
        return result;
    }

    public IEnumerable<Tag> GetAllTags()
    {
        return cache;
    }

    public Tag GetTagById(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            logger.LogInformation("GetTagById : TagId is null or empty");
            return Tag.Empty;
        }
        return cache.Find(t => t.Id == id) ?? Tag.Empty;
    }

    public bool UpdateTag(Tag tag)
    {
        int tagIndexToUpdate = cache.FindIndex(t => t.Id == tag.Id);

        if (tagIndexToUpdate == -1)
        {
            logger.LogWarning("UpdateTag : Tag not found : {0}", tag.Id);
            return false;
        }

        cache[tagIndexToUpdate] = tag;
        WriteToFile();
        return true;
    }
}
