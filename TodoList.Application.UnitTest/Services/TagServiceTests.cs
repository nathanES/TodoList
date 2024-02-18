using Moq;
using TodoList.Application.DTOs;
using TodoList.Application.Services;
using TodoList.Domain.Entities;
using TodoList.Domain.Enum;
using TodoList.Domain.Interfaces.Logger;
using TodoList.Domain.Interfaces.Repositories;
using TodoList.Infrastructure.Loggers;
using TodoList.Infrastructure.Repositories;
using Color = TodoList.Domain.ValueObjects.Color;

namespace TodoList.Application.UnitTest.Services;

[TestClass]
public class TagServiceTests
{
    //TODO faire le rework
    private ITagRepository? _tagRepository;
    private ITaskTagRepository? _taskTagRepository;
    private TaskRepositoryJson? _taskRepository;
    private TagService? _tagService;
    private TaskService? _taskService;
    private ILogger? _logger;

    [TestInitialize]
    public void TagServiceInitialize()
    {
        _logger = new LoggerCustom(new Mock<ILogDestination>().Object, LogLevel.Trace);
        _tagRepository = new TagRepositoryJson(_logger);
        _taskTagRepository = new TaskTagRepositoryJson(_logger);
        _taskRepository = new TaskRepositoryJson(_logger);
        _tagService = new(_tagRepository, _logger);
        _taskService = new(_taskRepository, _logger);

    }

    [TestMethod]
    [DataRow("TagName", "Description 1", "#000000")]
    public void TagService_AddTagWithAllProperties_ShouldAddTag(string tagName, string tagDescription, string tagColor)
    {
        //Arrange
        TagDto tag = new()
        {
            Id = Guid.NewGuid(),
            Name = tagName,
            Description = tagDescription,
            Color = new(tagColor),
            ParentTagIds = new HashSet<Guid>() { Guid.NewGuid() }
        };

        //Act
        _tagService.AddTag(tag);

        //Assert
        TagDto tagFound = _tagService.GetTagById(tag.Id);
        Assert.AreEqual(tag.Id, tagFound.Id);
        Assert.AreEqual(tag.Name, tagFound.Name);
        Assert.AreEqual(tag.Description, tagFound.Description);
        Assert.AreEqual(tag.Color, tagFound.Color);
        Assert.IsTrue(tag.ParentTagIds.SetEquals(tagFound.ParentTagIds));
    }
    [TestMethod]
    [DataRow("Tag 1")]
    public void TagService_AddTag_ShouldAddTag(string tagName)
    {
        //Arrange
        TagDto tag = new()
        {
            Id = Guid.NewGuid(),
            Name = tagName
        };
        //Act
        _tagService.AddTag(tag);
        //Assert
        TagDto tagFound = _tagService.GetTagById(tag.Id);
        Assert.AreEqual(tag.Id, tagFound.Id);
        Assert.AreEqual(tag.Name, tagFound.Name);
    }
    [TestMethod]
    [DataRow("description")]
    public void TagService_AddTagWithDescription_ShouldAddTag(string tagDescription)
    {
        TagDto tag = new()
        {
            Id = Guid.NewGuid(),
            Name = nameof(TagService_AddTagWithDescription_ShouldAddTag),
            Description = tagDescription
        };

        _tagService.AddTag(tag);

        TagDto tagFound = _tagService.GetTagById(tag.Id);

        Assert.AreEqual(tag.Id, tagFound.Id);
        Assert.AreEqual(tag.Name, tagFound.Name);
        Assert.AreEqual(tag.Description, tagFound.Description);
    }
    [TestMethod]
    [DataRow("#998877")]
    public void TagService_AddTagWithColor_ShouldAddTag(string tagColor)
    {
        //Arrange
        TagDto tag = new()
        {
            Id = Guid.NewGuid(),
            Name = nameof(TagService_AddTagWithColor_ShouldAddTag),
            Color = new Color(tagColor)
        };
        //Act
        _tagService.AddTag(tag);

        //Assert
        TagDto tagFound = _tagService.GetTagById(tag.Id);
        Assert.AreEqual(tag.Id, tagFound.Id);
        Assert.AreEqual(tag.Name, tagFound.Name);
        Assert.AreEqual(tag.Color, tagFound.Color);
    }
    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void TagService_AddTagWithoutName_ShouldArgumentNullException()
    {
        TagDto tag = new()
        {
            Id = Guid.NewGuid(),
        };
        _tagService.AddTag(tag);
    }

    [TestMethod]
    public void TagService_GetAllTag_ShouldGetAllTag()
    {
        TagDto tag = new()
        {
            Id = Guid.NewGuid(),
            Name = nameof(TagService_GetAllTag_ShouldGetAllTag)
        };
        TagDto tag2 = new()
        {
            Id = Guid.NewGuid(),
            Name = nameof(TagService_GetAllTag_ShouldGetAllTag) + "2"
        };
        _tagService.AddTag(tag);
        _tagService.AddTag(tag2);

        IEnumerable<TagDto> tagDtos = _tagService.GetAllTags();

        Assert.IsTrue(tagDtos.Any(x => x.Id == tag.Id));
        Assert.IsTrue(tagDtos.Any(x => x.Id == tag2.Id));
    }
    [TestMethod]
    public void TagService_GetTagById_ShouldGetTag()
    {
        TagDto tag = new()
        {
            Id = Guid.NewGuid(),
            Name = nameof(TagService_GetTagById_ShouldGetTag)
        };
        _tagService.AddTag(tag);

        TagDto tagDto = _tagService.GetTagById(tag.Id);

        Assert.AreEqual(tag.Id, tagDto.Id);
        Assert.AreEqual(tag.Name, tagDto.Name);
    }
    [TestMethod]
    public void TagService_GetTagByIdWhenNotFound_ShouldGetDefaultTag()
    {
        TagDto tagFound = _tagService.GetTagById(Guid.NewGuid());
        TagDto tagDefault = (TagDto)Tag.Default;
        Assert.AreEqual(tagDefault.Id, tagFound.Id);
        Assert.AreEqual(tagDefault.Name, tagFound.Name);
        Assert.AreEqual(tagDefault.Description, tagFound.Description);
        Assert.AreEqual(tagDefault.Color, tagFound.Color);
        Assert.IsTrue(tagDefault.ParentTagIds.SetEquals(tagFound.ParentTagIds));
    }
    [TestMethod]
    public void TagService_GetTagByIdWhenEmptyId_ShouldGetDefaultTag()
    {
        TagDto tagFound = _tagService.GetTagById(Guid.Empty);

        TagDto tagDefault = (TagDto)Tag.Default;
        Assert.AreEqual(tagDefault.Id, tagFound.Id);
        Assert.AreEqual(tagDefault.Name, tagFound.Name);
        Assert.AreEqual(tagDefault.Description, tagFound.Description);
        Assert.IsTrue(tagDefault.Color.Equals(tagFound.Color));
        Assert.AreEqual(tagDefault.ParentTagIds, tagFound.ParentTagIds);
    }

    [TestMethod]
    [DataRow("Tag 1", "Description 1", "#000000")]
    public void TagService_UpdateTagWithAllParametersWhenNoParameters_ShouldUpdateAllParameters(string newTagName, string newTagDescription, string newTagColor)
    {

        TagDto tag = new()
        {
            Id = Guid.NewGuid(),
            Name = nameof(TagService_UpdateTagWithAllParametersWhenNoParameters_ShouldUpdateAllParameters)
        };
        _tagService.AddTag(tag);

        TagDto newTag = new()
        {
            Id = tag.Id,
            Name = newTagName,
            Description = newTagDescription,
            Color = new Color(newTagColor)
        };
        _tagService.UpdateTag(newTag);

        TagDto tagFound = _tagService.GetTagById(tag.Id);

        Assert.AreEqual(newTag.Id, tagFound.Id);
        Assert.AreEqual(newTag.Name, tagFound.Name);
        Assert.AreEqual(newTag.Description, tagFound.Description);
        Assert.AreEqual(newTag.Color, tagFound.Color);
    }
    [TestMethod]
    [DataRow("Tag 1", "Description 1", "#000000")]
    public void TagService_UpdateTagWithAllParameters_ShouldUpdateAllParameters(string newTagName, string newTagDescription, string newTagColor)
    {

        TagDto tag = new()
        {
            Id = Guid.NewGuid(),
            Name = nameof(TagService_UpdateTagWithAllParameters_ShouldUpdateAllParameters),
            Description = "Description 1",
            Color = new Color("#000000")
        };
        _tagService.AddTag(tag);

        TagDto newTag = new()
        {
            Id = tag.Id,
            Name = newTagName,
            Description = newTagDescription,
            Color = new Color(newTagColor)
        };
        _tagService.UpdateTag(newTag);

        TagDto tagFound = _tagService.GetTagById(tag.Id);

        Assert.AreEqual(newTag.Id, tagFound.Id);
        Assert.AreEqual(newTag.Name, tagFound.Name);
        Assert.AreEqual(newTag.Description, tagFound.Description);
        Assert.AreEqual(newTag.Color, tagFound.Color);
    }
    [TestMethod]
    public void TagService_DeleteTagById_ShouldDeleteTagById()
    {
        TagDto tag = new()
        {
            Id = Guid.NewGuid(),
            Name = nameof(TagService_DeleteTagById_ShouldDeleteTagById),
        };
        _tagService.AddTag(tag);

        _tagService.DeleteTagById(tag.Id, _taskTagRepository);

        TagDto tagFound = _tagService.GetTagById(tag.Id);
        TagDto tagDefault = (TagDto)Tag.Default;
        Assert.AreEqual(tagDefault.Id, tagFound.Id);
        Assert.AreEqual(tagDefault.Name, tagFound.Name);
        Assert.AreEqual(tagDefault.Description, tagFound.Description);
        Assert.AreEqual(tagDefault.Color, tagFound.Color);
    }
    [TestMethod]
    public void TagService_DeleteTagByIdWheNotExisting_ShouldDoNothing()
    {
        Guid tagId = Guid.NewGuid();
        _tagService.DeleteTagById(tagId, _taskTagRepository);

        TagDto tagFound = _tagService.GetTagById(tagId);
        TagDto tagDefault = (TagDto)Tag.Default;
        Assert.AreEqual(tagDefault.Id, tagFound.Id);
        Assert.AreEqual(tagDefault.Name, tagFound.Name);
        Assert.AreEqual(tagDefault.Description, tagFound.Description);
        Assert.AreEqual(tagDefault.Color, tagFound.Color);
    }

    [TestMethod]
    public void TagService_DeleteTagsByIds_ShouldDeleteTagsByIds()
    {
        TagDto tag = new()
        {
            Id = Guid.NewGuid(),
            Name = nameof(TagService_DeleteTagById_ShouldDeleteTagById),
        };
        TagDto tag2 = new()
        {
            Id = Guid.NewGuid(),
            Name = nameof(TagService_DeleteTagById_ShouldDeleteTagById) + "2",
        };
        _tagService.AddTag(tag);
        _tagService.AddTag(tag2);

        _tagService.DeleteTagByIds(new List<Guid>() { tag.Id, tag2.Id }, _taskTagRepository);

        TagDto tagDefault = (TagDto)Tag.Default;
        TagDto tagFound = _tagService.GetTagById(tag.Id);
        Assert.AreEqual(tagDefault.Id, tagFound.Id);
        Assert.AreEqual(tagDefault.Name, tagFound.Name);
        Assert.AreEqual(tagDefault.Description, tagFound.Description);
        Assert.AreEqual(tagDefault.Color, tagFound.Color);
        TagDto tagFound2 = _tagService.GetTagById(tag.Id);
        Assert.AreEqual(tagDefault.Id, tagFound2.Id);
        Assert.AreEqual(tagDefault.Name, tagFound2.Name);
        Assert.AreEqual(tagDefault.Description, tagFound2.Description);
        Assert.AreEqual(tagDefault.Color, tagFound2.Color);
    }
    [TestMethod]
    public void TagService_DeleteTagsByIdsWheNotExisting_ShouldDoNothing()
    {
        Guid tagId = Guid.NewGuid();
        Guid tagId2 = Guid.NewGuid();

        _tagService.DeleteTagByIds(new List<Guid>() { tagId, tagId2 }, _taskTagRepository);

        TagDto tagDefault = (TagDto)Tag.Default;
        TagDto tagFound = _tagService.GetTagById(tagId);
        Assert.AreEqual(tagDefault.Id, tagFound.Id);
        Assert.AreEqual(tagDefault.Name, tagFound.Name);
        Assert.AreEqual(tagDefault.Description, tagFound.Description);
        Assert.AreEqual(tagDefault.Color, tagFound.Color);
        TagDto tagFound2 = _tagService.GetTagById(tagId);
        Assert.AreEqual(tagDefault.Id, tagFound2.Id);
        Assert.AreEqual(tagDefault.Name, tagFound2.Name);
        Assert.AreEqual(tagDefault.Description, tagFound2.Description);
        Assert.AreEqual(tagDefault.Color, tagFound2.Color);
    }
    [TestMethod]
    public void TagService_AssignTaskToTag_ShouldAssignTaskToTag()
    {
        //Arrange
        TagDto tag = new()
        {
            Id = Guid.NewGuid(),
            Name = nameof(TagService_AssignTaskToTag_ShouldAssignTaskToTag),
        };
        _tagService.AddTag(tag);
        TaskDto task = new()
        {
            Id = Guid.NewGuid(),
            Name = nameof(TagService_AssignTaskToTag_ShouldAssignTaskToTag),
        };
        _taskService.AddTask(task);

        //Act
        _tagService.AssignTaskToTag(task.Id, tag.Id, _taskTagRepository, _taskRepository);

        //Assert
        IEnumerable<TaskTag> taskTags = _taskTagRepository.GetTaskTagsByTagId(tag.Id);
        Assert.IsTrue(taskTags.Any(x => x.TaskId == task.Id));
    }
    [TestMethod]
    public void TagService_AssignTasksToTag_ShouldAssignTasksToTag()
    {
        //Arrange
        TagDto tag = new()
        {
            Id = Guid.NewGuid(),
            Name = nameof(TagService_AssignTaskToTag_ShouldAssignTaskToTag),
        };
        _tagService.AddTag(tag);
        TaskDto task = new()
        {
            Id = Guid.NewGuid(),
            Name = nameof(TagService_AssignTaskToTag_ShouldAssignTaskToTag),
        };
        TaskDto task2 = new()
        {
            Id = Guid.NewGuid(),
            Name = nameof(TagService_AssignTaskToTag_ShouldAssignTaskToTag) + "2",
        };
        _taskService.AddTask(task);
        _taskService.AddTask(task2);

        //Act
        _tagService.AssignTasksToTag(new List<Guid>() { task.Id, task2.Id }, tag.Id, _taskTagRepository, _taskRepository);

        //Assert
        IEnumerable<TaskTag> taskTags = _taskTagRepository.GetTaskTagsByTagId(tag.Id);
        Assert.IsTrue(taskTags.Any(x => x.TaskId == task.Id));
        Assert.IsTrue(taskTags.Any(x => x.TaskId == task2.Id));

    }
    //Todo continuer a faire les tests
}
//public void AssignTaskToTags(Guid taskId, IEnumerable<Guid> tagIds, ITaskTagRepository taskTagRepository, ITaskRepository taskRepository)
//public void UnassignTagFromTask(Guid tagId, Guid taskId, ITaskTagRepository taskTagRepository)
//public void UnassignAllTaskFromTag(Guid tagId, ITaskTagRepository taskTagRepository)
//public void UnassignAllTaskFromTags(IEnumerable<Guid> tagIds, ITaskTagRepository taskTagRepository)
