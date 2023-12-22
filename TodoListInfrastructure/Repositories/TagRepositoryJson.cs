using Newtonsoft.Json;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using TodoList.Domain.Entities;
using TodoList.Domain.Interfaces;

namespace TodoList.Infrastructure.Repositories
{
  public class TagRepositoryJson : ITagRepository
  {
    private readonly string _tagsFilePath = $@"{Settings.JsonDataFilePathBase}tags.json";

    public void AddTag(Tag tag)
    {
      List<Tag> tags = GetAllTags().ToList();
      tags.Add(tag);
      WriteToFile(tags);
    }

    public void DeleteTag(Tag tag)
    {
      List<Tag> tags = GetAllTags().ToList();
      int tagIndexToDelete = tags.FindIndex(t => t.Id == tag.Id);

      if(tagIndexToDelete == -1)
        return;
      
      tags.RemoveAt(tagIndexToDelete);
      WriteToFile(tags);
    }

    public IEnumerable<Tag> GetAllTags()
    {
      if(!File.Exists(_tagsFilePath))
        return new List<Tag>();

      string json = File.ReadAllText(_tagsFilePath);
      return JsonConvert.DeserializeObject<IEnumerable<Tag>>(json) ?? new List<Tag>();
    }

    public Tag GetTagById(string id)
    {
      if (!File.Exists(_tagsFilePath))
        return null;//Tag.Empty;

      List<Tag> tags = GetAllTags().ToList();
      return tags.Find(t => t.Id == id);// ??Tag.Empty;
    }

    public void UpdateTag(Tag tag)
    {
      List<Tag> tags = GetAllTags().ToList();
      int tagIndexToUpdate = tags.FindIndex(t=>t.Id == tag.Id);

      if (tagIndexToUpdate == -1)
        return;
      
      tags[tagIndexToUpdate] = tag;
      WriteToFile(tags);
    }

    private void WriteToFile(List<Tag> tags)
    {
      string json = JsonConvert.SerializeObject(tags, Formatting.Indented);
      File.WriteAllText(_tagsFilePath, json);
    }

    //private void WriteToFile<T>(T tags) where T : IEnumerable<Tag> 
    //{
    //  string json = JsonConvert.SerializeObject(tags, Formatting.Indented);
    //  File.WriteAllText(_tagsFilePath, json);
    //}
  }
}
