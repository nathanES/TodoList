using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoList.Application.DTOs;
using TodoList.Domain.Entities;
using TodoList.Domain.Interfaces;

namespace TodoList.Application.Services;
public class TagService
{
  private readonly ITagRepository tagRepository;

  private readonly ITaskTagRepository taskTagRepository;

  public TagService(ITagRepository tagRepository, ITaskTagRepository taskTagRepository)
    {
    this.tagRepository = tagRepository;
    this.taskTagRepository = taskTagRepository;
  }
  public IEnumerable<TagDto> GetAllTags()
  {
    return tagRepository.GetAllTags().Select(t => (TagDto)t);
  }
  public TagDto GetTagById(string tagId)
  {
    return (TagDto)tagRepository.GetTagById(tagId);
  }
  public void AddTag(TagDto tagDto)
  {
    Tag tag = (Tag)tagDto;
    tagRepository.AddTag(tag);
  }
  public void UpdateTag(TagDto tagDto)
  {
    Tag tag = (Tag)tagDto;
    tagRepository.UpdateTag(tag);
  }
  public void DeleteTagById(string tagId)
  {
    UnassignAllTaskFromTag(tagId);
    tagRepository.DeleteTagById(tagId);
  }
  public void DeleteTagByIds(IEnumerable<string> tagIds)
  {
    foreach (string tagId in tagIds)
    {
     UnassignAllTaskFromTag(tagId);
    }
    tagRepository.DeleteTagByIds(tagIds);
  }
  private void UnassignAllTaskFromTag(string tagId)
  {
    var taskTags = taskTagRepository.GetTaskTagsByTagId(tagId);

    if (!taskTags.Any())
      return; //The tag does not have task

    taskTagRepository.DeleteTaskTagByIds(taskTags.Select(tt => tt.Id));
  }
  public void UnassignAllTaskFromTags(IEnumerable<string> tagIds)
  {
    IEnumerable<TaskTag> taskTags = taskTagRepository.GetTaskTagsByTagIds(tagIds);
    if (taskTags == null)
      return; //The tag does not have task

    taskTagRepository.DeleteTaskTagByIds(taskTags.Select(t => t.Id));
  }
}
