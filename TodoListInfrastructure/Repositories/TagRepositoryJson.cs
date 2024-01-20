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

    public void AddTag(Tag tag)
    {
        if (cache.Any(t => t.Id == tag.Id))
        {
            logger.LogCritical("AddTag : DuplicateKey : {0}", tag.Id);
            throw new DuplicateKeyException($"Duplicate {nameof(Tag.Id)}, Value : {tag.Id}");
        }
        cache.Add(tag);
        WriteToFile();
    }

    public void DeleteTagById(string tagId)
    {
        int tagIndexToDelete = cache.FindIndex(t => t.Id == tagId);

        if (tagIndexToDelete == -1)
        {
            logger.LogWarning(_tagsFilePath, "DeleteTagById : Tag not found : {0}", tagId);
            return;
        }

        cache.RemoveAt(tagIndexToDelete);
        WriteToFile();
    }

    public void DeleteTagByIds(IEnumerable<string> tagIds)
    {
        foreach (string tagId in tagIds)
        {
            int tagIndexToDelete = cache.FindIndex(t => t.Id == tagId);

            if (tagIndexToDelete == -1)
            {
                logger.LogWarning(_tagsFilePath, "DeleteTagByIds : Tag not found : {0}", tagId);
                continue;
            }
            cache.RemoveAt(tagIndexToDelete);
        }
        WriteToFile();
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
        Tag tag = cache.Find(t => t.Id == id);
        if (tag == null)
        {
            logger.LogInformation("GetTagById : Tag not found : {0}", id);
            return Tag.Empty;
        }
        return tag;
    }

    public void UpdateTag(Tag tag)
    {
        int tagIndexToUpdate = cache.FindIndex(t => t.Id == tag.Id);

        if (tagIndexToUpdate == -1)
        {
            logger.LogWarning(_tagsFilePath, "UpdateTag : Tag not found : {0}", tag.Id);
            return;
        }

        cache[tagIndexToUpdate] = tag;
        WriteToFile();
    }
}
