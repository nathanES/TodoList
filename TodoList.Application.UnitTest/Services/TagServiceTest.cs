using TodoList.Application.DTOs;
using TodoList.Application.Services;
using TodoList.Domain.Interfaces;
using TodoList.Infrastructure.Repositories;

namespace TodoList.Application.UnitTest.Services;

[TestClass]
public class TagServiceTest
{
  //TODO faire les tests
  private ITagRepository? tagRepository;
  private ITaskTagRepository? taskTagRepository;
  [TestInitialize]
  public void TagServiceInitialize()
  {
    tagRepository = new TagRepositoryJson();
    taskTagRepository = new TaskTagRepositoryJson();
  }

  [TestMethod]
  public void GetAllTags()
  {
    TagService tagService = new(tagRepository);
    tagService.AddTag(new TagDto { Name = "Tag1" });

    IEnumerable<TagDto> tagDtos = tagService.GetAllTags();
    //TODO : faire les assertions
    Assert.IsNotNull(tagDtos);
    Assert.IsTrue(tagDtos.Any());
  }

  //namespace TodoList.Application.Services;
  //public class TagService
  //{
  //  private readonly ITagRepository tagRepository;

  //  public TagService(ITagRepository tagRepository) => this.tagRepository = tagRepository;//this.taskTagRepository = taskTagRepository;
  //  public IEnumerable<TagDto> GetAllTags() => tagRepository.GetAllTags().Select(t => (TagDto)t);
  //  public TagDto GetTagById(string tagId) => (TagDto)tagRepository.GetTagById(tagId);
  //  public void AddTag(TagDto tagDto)
  //  {
  //    Tag tag = (Tag)tagDto;
  //    tagRepository.AddTag(tag);
  //  }
  //  public void UpdateTag(TagDto tagDto)
  //  {
  //    Tag tag = (Tag)tagDto;
  //    tagRepository.UpdateTag(tag);
  //  }
  //  public void DeleteTagById(string tagId, ITaskTagRepository taskTagRepository)
  //  {
  //    UnassignAllTaskFromTag(tagId, taskTagRepository);
  //    tagRepository.DeleteTagById(tagId);
  //  }
  //  public void DeleteTagByIds(IEnumerable<string> tagIds, ITaskTagRepository taskTagRepository)
  //  {
  //    foreach (string tagId in tagIds)
  //    {
  //      UnassignAllTaskFromTag(tagId, taskTagRepository);
  //    }
  //    tagRepository.DeleteTagByIds(tagIds);
  //  }
  //  private void UnassignAllTaskFromTag(string tagId, ITaskTagRepository taskTagRepository)
  //  {
  //    IEnumerable<TaskTag> taskTags = taskTagRepository.GetTaskTagsByTagId(tagId);

  //    if (!taskTags.Any())
  //      return; //The tag does not have task

  //    taskTagRepository.DeleteTaskTagByIds(taskTags.Select(tt => tt.Id));
  //  }
  //  public void UnassignAllTaskFromTags(IEnumerable<string> tagIds, ITaskTagRepository taskTagRepository)
  //  {
  //    IEnumerable<TaskTag> taskTags = taskTagRepository.GetTaskTagsByTagIds(tagIds);
  //    if (taskTags == null)
  //      return; //The tag does not have task

  //    taskTagRepository.DeleteTaskTagByIds(taskTags.Select(t => t.Id));
  //  }

}
