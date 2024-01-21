using TodoList.Application.DTOs;
using TodoList.Application.Services;
using TodoList.Domain.Entities;
using TodoList.Domain.Interfaces;
using TodoList.Domain.Interfaces.Repositories;
using TodoList.Infrastructure;
using TodoList.Infrastructure.Repositories;

namespace TodoList.Application.UnitTest.Services;

[TestClass]
public class TagServiceTest
{
    private ITagRepository? tagRepository;
    private ITaskTagRepository? taskTagRepository;
    private ILogger? logger;
    [TestInitialize]
    public void TagServiceInitialize()
    {
        logger = new LoggerCustom();
        tagRepository = new TagRepositoryJson(logger);
        taskTagRepository = new TaskTagRepositoryJson(logger);
    }

    [TestMethod]
    [DataRow("", "Tag 1", "Description 1", "#000000")]
    public void AddTag_WithAllProperties_WithoutParentTags(string id, string name, string description, string color)
    {
        string idToInsert = !string.IsNullOrEmpty(id) ? id : Guid.NewGuid().ToString();
        TagService tagService = new(tagRepository, logger);
        TagDto tagDtoInsert = new()
        {
            Id = idToInsert,
            Name = name,
            Description = description,
            Color = color
        };

        tagService.AddTag(tagDtoInsert);

        TagDto tagDto = tagService.GetTagById(tagDtoInsert.Id);

        Assert.IsNotNull(tagDto);
        Assert.AreEqual(idToInsert, tagDto.Id);
        Assert.AreEqual(name, tagDto.Name);
        Assert.AreEqual(description, tagDto.Description);
        Assert.AreEqual(color, tagDto.Color);
    }

    [TestMethod]
    [DataRow("Tag 1")]
    public void AddTag_WithNameAndId(string name)
    {
        string idToInsert = Guid.NewGuid().ToString();
        TagService tagService = new(tagRepository, logger);
        TagDto tagDtoInsert = new()
        {
            Id = idToInsert,
            Name = name,
        };

        tagService.AddTag(tagDtoInsert);

        TagDto tagDto = tagService.GetTagById(tagDtoInsert.Id);

        Assert.IsNotNull(tagDto);
        Assert.AreEqual(idToInsert, tagDto.Id);
        Assert.AreEqual(name, tagDto.Name);
    }
    [TestMethod]
    [DataRow("")]
    public void AddTag_WithId_Exception(string id) //Il manque le nom
    {
        string idToInsert = !string.IsNullOrEmpty(id) ? id : Guid.NewGuid().ToString();
        TagService tagService = new(tagRepository, logger);
        TagDto tagDtoInsert = new()
        {
            Id = idToInsert,
        };

        _ = Assert.ThrowsException<ArgumentNullException>(()=>tagService.AddTag(tagDtoInsert));

    }
    [TestMethod]
    [DataRow("Tag 1", "description")]
    public void AddTag_WithNameAndDescription(string name, string description)
    {
        string idToInsert = Guid.NewGuid().ToString();
        TagService tagService = new(tagRepository, logger);
        TagDto tagDtoInsert = new()
        {
            Id = idToInsert,
            Name = name,
            Description = description
        };

        tagService.AddTag(tagDtoInsert);

        TagDto tagDto = tagService.GetTagById(tagDtoInsert.Id);

        Assert.IsNotNull(tagDto);
        Assert.AreEqual(idToInsert, tagDto.Id);
        Assert.AreEqual(name, tagDto.Name);
        Assert.AreEqual(description, tagDto.Description);
    }
    [TestMethod]
    [DataRow("Tag 1", "#000000")]
    public void AddTag_WithNameAndColor(string name, string color)
    {
        string idToInsert = Guid.NewGuid().ToString();
        TagService tagService = new(tagRepository, logger);
        TagDto tagDtoInsert = new()
        {
            Id = idToInsert,
            Name = name,
            Color = color
        };

        tagService.AddTag(tagDtoInsert);

        TagDto tagDto = tagService.GetTagById(tagDtoInsert.Id);

        Assert.IsNotNull(tagDto);
        Assert.AreEqual(idToInsert, tagDto.Id);
        Assert.AreEqual(name, tagDto.Name);
        Assert.AreEqual(color, tagDto.Color);
    }
    [TestMethod]
    [DataRow("Description")]
    public void AddTag_WithoutName_Exception(string description)
    {
        string idToInsert = Guid.NewGuid().ToString();
        TagService tagService = new(tagRepository, logger);
        TagDto tagDtoInsert = new()
        {
            Id = idToInsert,
            Description = description
        };
        _ = Assert.ThrowsException<ArgumentNullException>(() => tagService.AddTag(tagDtoInsert));
    }

    [TestMethod]
    [DataRow("Tag 1")]
    public void GetAllTags(string tagName)
    {
        TagService tagService = new TagService(tagRepository, logger); 
        tagService.AddTag(new TagDto { Id = Guid.NewGuid().ToString(), Name = tagName });

        IEnumerable<TagDto> tagDtos = tagService.GetAllTags();

        Assert.IsNotNull(tagDtos);
        Assert.IsTrue(tagDtos.Any());
    }
    [TestMethod]
    [DataRow("")]
    public void GetTagById(string id)
    {
        string idToInsert = !string.IsNullOrEmpty(id) ? id : Guid.NewGuid().ToString();
        string nameToInsert = "Tag 1";
        TagService tagService = new(tagRepository, logger);
        TagDto tagDtoInsert = new() { Id = idToInsert, Name = nameToInsert };
        tagService.AddTag(tagDtoInsert);

        TagDto tagDto = tagService.GetTagById(tagDtoInsert.Id);

        Assert.IsNotNull(tagDto);
        Assert.AreEqual(idToInsert, tagDto.Id);
        Assert.AreEqual(nameToInsert, tagDto.Name);
    }
    [TestMethod]
    public void GetTagById_NotFound()
    {
        TagService tagService = new(tagRepository, logger);

        TagDto tagDto = tagService.GetTagById(Guid.NewGuid().ToString());
        var tagDtoEmpty = (TagDto)Tag.Empty;
        Assert.AreEqual(tagDtoEmpty.Id, tagDto.Id);
        Assert.AreEqual(tagDtoEmpty.Name,tagDto.Name);
        Assert.AreEqual(tagDtoEmpty.Description,tagDto.Description);
        Assert.AreEqual(tagDtoEmpty.Color,tagDto.Color);
        Assert.AreEqual(tagDtoEmpty.ParentTagIds,tagDto.ParentTagIds);
    }
    [TestMethod]
    public void GetTagById_NullId()
    {
        TagService tagService = new(tagRepository, logger);

        TagDto tagDto = tagService.GetTagById(null);

        var tagDtoEmpty = (TagDto)Tag.Empty;
        Assert.AreEqual(tagDtoEmpty.Id, tagDto.Id);
        Assert.AreEqual(tagDtoEmpty.Name, tagDto.Name);
        Assert.AreEqual(tagDtoEmpty.Description, tagDto.Description);
        Assert.AreEqual(tagDtoEmpty.Color, tagDto.Color);
        Assert.AreEqual(tagDtoEmpty.ParentTagIds, tagDto.ParentTagIds);

    }
    [TestMethod]
    public void GetTagById_EmptyId()
    {
        TagService tagService = new(tagRepository, logger);

        TagDto tagDto = tagService.GetTagById("");

        var tagDtoEmpty = (TagDto)Tag.Empty;
        Assert.AreEqual(tagDtoEmpty.Id, tagDto.Id);
        Assert.AreEqual(tagDtoEmpty.Name, tagDto.Name);
        Assert.AreEqual(tagDtoEmpty.Description, tagDto.Description);
        Assert.AreEqual(tagDtoEmpty.Color, tagDto.Color);
        Assert.AreEqual(tagDtoEmpty.ParentTagIds, tagDto.ParentTagIds);
    }

    [TestMethod]
    [DataRow("", "Tag 1", "Description 1", "#000000", "Tag 2", "Description 2", "#FFFFFF")]
    public void UpdateTag_WithFullParameters(string id, string name, string description, string color, string name2, string description2, string color2)
    {
        string idToInsert1 = !String.IsNullOrWhiteSpace(id) ? id : Guid.NewGuid().ToString();
        TagService tagService = new(tagRepository, logger);
        TagDto tagDtoInsert = new()
        {
            Id = idToInsert1,
            Name = name,
            Description = description,
            Color = color
        };
        tagService.AddTag(tagDtoInsert);

        TagDto tagDtoUpdate = new()
        {
            Id = idToInsert1,
            Name = name2,
            Description = description2,
            Color = color2
        };
        tagService.UpdateTag(tagDtoUpdate);

        TagDto tagDto = tagService.GetTagById(tagDtoInsert.Id);

        Assert.IsNotNull(tagDto);
        Assert.AreEqual(idToInsert1, tagDto.Id);
        Assert.AreEqual(name2, tagDto.Name);
        Assert.AreEqual(description2, tagDto.Description);
        Assert.AreEqual(color2, tagDto.Color);
    }

    //TODO : a continuer avec les différentes méthodes.

    //namespace TodoList.Application.Services;
    //public class TagService
    //{
    //  private readonly ITagRepository tagRepository;

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
