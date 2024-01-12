using Newtonsoft.Json;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using TodoList.Domain.Entities;
using TodoList.Domain.Exceptions;
using TodoList.Domain.Interfaces;

namespace TodoList.Infrastructure.Repositories
{
  public class TagRepositoryJson : ITagRepository
  {
    private readonly string _tagsFilePath = $@"{Settings.JsonDataFilePathBase}tags.json";
    private List<Tag> cache;
    private readonly object fileLock = new object();
    public TagRepositoryJson()
    {
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
          Console.WriteLine("LoadCache Impossible !"); //TODO mettre un logger à la place
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
        Console.WriteLine("WriteToFile Impossible !"); //TODO mettre un logger à la place
        throw;
      }
    }

    public void AddTag(Tag tag)
    {
        if (cache.Any(t => t.Id == tag.Id))
            throw new DuplicateKeyException($"Duplicate {nameof(Tag.Id)}, Value : {tag.Id}");
      cache.Add(tag);
      WriteToFile();
    }

    public void DeleteTagById(string tagId)
    {
      int tagIndexToDelete = cache.FindIndex(t => t.Id == tagId);

      if(tagIndexToDelete == -1)
        return;
      
      cache.RemoveAt(tagIndexToDelete);
      WriteToFile();
    }

    public void DeleteTagByIds(IEnumerable<string> tagIds)
    {
      foreach (string tagId in tagIds)
      {
        int tagIndexToDelete = cache.FindIndex(t => t.Id == tagId);

        if (tagIndexToDelete == -1)
          continue;

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
      return cache.Find(t => t.Id == id);// ??Tag.Empty;
    }

    public void UpdateTag(Tag tag)
    {
      int tagIndexToUpdate = cache.FindIndex(t=>t.Id == tag.Id);

      if (tagIndexToUpdate == -1)
        return;
      
      cache[tagIndexToUpdate] = tag;
      WriteToFile();
    }
  }
}
