using Newtonsoft.Json;
using TodoList.Domain.Entities;
using TodoList.Domain.Exceptions;
using TodoList.Domain.Interfaces.Logger;
using TodoList.Domain.Interfaces.Repositories;

namespace TodoList.Infrastructure.Repositories;

public class TagRepositoryJson : ITagRepository
{
    private readonly string _tagsFilePath = $@"{Settings.JsonDataFilePathBase}Tags.json";
    private List<Tag> _cache;
    private readonly object _fileLock = new();
    private readonly ILogger _logger;

    public TagRepositoryJson(ILogger logger)
    {
        _logger = logger;
        LoadCache();
    }
    private void LoadCache()
    {
        try
        {
            if (!File.Exists(_tagsFilePath))
            {
                _cache = new List<Tag>();
                return;
            }

            string json = File.ReadAllText(_tagsFilePath);
            _cache = JsonConvert.DeserializeObject<List<Tag>>(json) ?? new List<Tag>();
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
                File.WriteAllText(_tagsFilePath, json);
            }
        }
        catch (Exception e)
        {
            _logger.LogException(e, "WriteToFile Impossible !", LogLevel.Error);
            throw;
        }
    }

    public bool AddTag(Tag tag)
    {
        if (_cache.Any(t => t.Id == tag.Id))
        {
            _logger.LogCritical("AddTag : DuplicateKey : {0}", tag.Id);
            throw new DuplicateKeyException($"Duplicate {nameof(Tag.Id)}, Value : {tag.Id}");
        }
        _cache.Add(tag);
        WriteToFile();
        return true;
    }

    public bool DeleteTagById(Guid tagId)
    {
        int tagIndexToDelete = _cache.FindIndex(t => t.Id == tagId);

        if (tagIndexToDelete == -1)
        {
            _logger.LogWarning("DeleteTagById : Tag not found : {0}", tagId);
            return false;
        }

        _cache.RemoveAt(tagIndexToDelete);
        WriteToFile();
        return true;
    }

    public bool DeleteTagByIds(IEnumerable<Guid> tagIds)
    {
        bool result = true;
        foreach (Guid tagId in tagIds)
        {
            int tagIndexToDelete = _cache.FindIndex(t => t.Id == tagId);

            if (tagIndexToDelete == -1)
            {
                _logger.LogWarning("DeleteTagByIds : Tag not found : {0}", tagId);
                result = false;
                continue;
            }
            _cache.RemoveAt(tagIndexToDelete);
        }
        WriteToFile();
        return result;
    }

    public IEnumerable<Tag> GetAllTags()
    {
        return _cache;
    }

    public Tag GetTagById(Guid id)
    {
        if (id == null || id == Guid.Empty)
        {
            _logger.LogInformation("GetTagById : TagId is null or empty");
            return Tag.Default;
        }
        return _cache.Find(t => t.Id == id) ?? Tag.Default;
    }

    public bool UpdateTag(Tag tag)
    {
        int tagIndexToUpdate = _cache.FindIndex(t => t.Id == tag.Id);

        if (tagIndexToUpdate == -1)
        {
            _logger.LogWarning("UpdateTag : Tag not found : {0}", tag.Id);
            return false;
        }

        _cache[tagIndexToUpdate] = tag;
        WriteToFile();
        return true;
    }
}
