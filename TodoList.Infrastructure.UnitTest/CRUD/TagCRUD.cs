using Moq;
using TodoList.Domain.Entities;
using TodoList.Domain.Exceptions;
using TodoList.Domain.Interfaces.Logger;
using TodoList.Domain.Interfaces.Repositories;
using TodoList.Infrastructure.Loggers;
using TodoList.Infrastructure.Repositories;

namespace TodoList.Infrastructure.UnitTest.CRUD;

[TestClass]
public class TagCRUD
{
    private ITagRepository? _tagRepository;
    private ILogger? _logger;
    private readonly Mock<ILogDestination> _logDestination = new();

    [TestInitialize]
    public void TagCRUDInitialize()
    {
        _logger = new LoggerCustom(_logDestination.Object);
        _tagRepository = new TagRepositoryJson(_logger);
    }

    [TestMethod]
    [DataRow("AddTag", "AddTagParent", "Description 1", "#000000")]
    public void AddTag(string name, string parentName, string description, string color)
    {
        //Arrange
        Tag tagParent = new Tag.TagBuilder(parentName)
            .Build();
        _ = _tagRepository.AddTag(tagParent);
        Assert.IsTrue(_tagRepository.GetAllTags().Any(t => t.Id == tagParent.Id));

        Tag tag = new Tag.TagBuilder(name)
            .SetDescription(description)
            .SetColor(new Color(color))
            .SetParentTagIds(new List<Guid>() { tagParent.Id })
            .Build();
        //Act
        bool addResult = _tagRepository.AddTag(tag);
        //Assert
        Assert.IsTrue(addResult);
        Assert.IsTrue(_tagRepository.GetAllTags().Any(t => t.Id == tag.Id));
    }
    [TestMethod]
    [DataRow("AddTagDuplicateKey", "AddTagParent", "Description 1", "#000000")]
    public void AddTag_DuplicateKey(string name, string parentName, string description, string color)
    {
        //Arrange
        Tag tag = new Tag.TagBuilder(name)
            .SetDescription(description)
            .SetColor(new Color(color))
            .Build();
        bool addResult = _tagRepository.AddTag(tag);
        Assert.IsTrue(addResult);
        Assert.IsTrue(_tagRepository.GetAllTags().Any(t => t.Id == tag.Id));
        //Act and Assert
        _ = Assert.ThrowsException<DuplicateKeyException>(() => _tagRepository.AddTag(tag));
    }

    [TestMethod]
    [DataRow("DeleteTag", "Description", "#000000")]
    public void DeleteTag(string name, string description, string color)
    {
        //Arrange

        Tag tag = new Tag.TagBuilder(name)
            .SetDescription(description)
            .SetColor(new Color(color))
            .Build();

        _ = _tagRepository.AddTag(tag);
        //Act
        bool deleteResult = _tagRepository.DeleteTagById(tag.Id);
        //Assert
        Assert.IsTrue(deleteResult);
        Assert.IsFalse(_tagRepository.GetAllTags().Any(t => t.Id == tag.Id));
    }
    [TestMethod]
    [DataRow("DeleteTags", "Description", "#000000")]
    public void DeleteTags(string name, string description, string color)
    {
        //Arrange
        Tag tag = new Tag.TagBuilder(name)
            .SetDescription(description)
            .SetColor(new Color(color))
            .Build();

        _ = _tagRepository.AddTag(tag);

        Tag tag2 = new Tag.TagBuilder(name)
          .SetDescription(description)
          .SetColor(new Color(color))
          .Build();

        _ = _tagRepository.AddTag(tag2);

        //Act
        bool deleteResult = _tagRepository.DeleteTagByIds(new List<Guid>() { tag.Id, tag2.Id });
        //Assert
        Assert.IsTrue(deleteResult);
        Assert.IsFalse(_tagRepository.GetAllTags().Any(t => t.Id == tag.Id));
        Assert.IsFalse(_tagRepository.GetAllTags().Any(t => t.Id == tag2.Id));
    }
    [TestMethod]
    public void DeleteTag_NotFound()
    {
        //Act
        bool deleteResult = _tagRepository.DeleteTagById(Guid.NewGuid());
        //Assert
        Assert.IsFalse(deleteResult);
    }
    [TestMethod]
    public void DeleteTags_NotFound()
    {
        IEnumerable<Guid> guids = new List<Guid>() { Guid.NewGuid(), Guid.NewGuid(), };
        //Arrange
        bool deleteResult = _tagRepository.DeleteTagByIds(guids);
        //Assert
        Assert.IsFalse(deleteResult);
    }

    [TestMethod]
    [DataRow("UpdateTag", "UpdateTag2", "Description", "#000000")]
    public void UpdateTag(string name, string updatedName, string description, string color)
    {
        //Arrange
        Tag tag = new Tag.TagBuilder(name)
            .SetDescription(description)
            .SetColor(new Color(color))
            .Build();

        _ = _tagRepository.AddTag(tag);

        tag.UpdateName(updatedName);
        //Act
        bool updateResult = _tagRepository.UpdateTag(tag);
        //Assert
        Assert.IsTrue(updateResult);
        Assert.IsTrue(_tagRepository.GetAllTags().Any(t => t.Name == updatedName));
    }
    [TestMethod]
    [DataRow("UpdateTagNotFound", "Description", "#000000")]
    public void UpdateTag_NotFound(string name, string description, string color)
    {
        //Arrange
        Tag tag = new Tag.TagBuilder(name)
            .SetDescription(description)
            .SetColor(new Color(color))
            .Build();

        //Act
        bool updateResult = _tagRepository.UpdateTag(tag);
        //Assert
        Assert.IsFalse(updateResult);
        Assert.IsFalse(_tagRepository.GetAllTags().Any(t => t.Name == name));

    }

    [TestMethod]
    [DataRow("GetTagById", "Description", "#000000")]
    public void GetTagById(string name, string description, string color)
    {
        //Arrange
        Tag tag = new Tag.TagBuilder(name)
            .SetDescription(description)
            .SetColor(new Color(color))
            .Build();
        _ = _tagRepository.AddTag(tag);
        //Act
        Tag tagFound = _tagRepository.GetTagById(tag.Id);
        //Assert
        TagCompare(tagFound, tag);
    }
    [TestMethod]
    [DataRow("GetTagById", "Description", "#000000")]
    public void GetTagById_NotFound(string name, string description, string color)
    {
        //Arrange
        Tag tag = new Tag.TagBuilder(name)
            .SetDescription(description)
            .SetColor(new Color(color))
            .Build();
        //Act
        Tag tagFound = _tagRepository.GetTagById(tag.Id);
        //Assert
        TagCompare(tagFound, Tag.Default);
    }

    [TestMethod]
    [DataRow("GetAllTags", "Description", "#000000")]
    public void GetAllTags(string name, string description, string color)
    {
        //Arrange
        Tag tag = new Tag.TagBuilder(name)
            .SetDescription(description)
            .SetColor(new Color(color))
            .Build();
        _ = _tagRepository.AddTag(tag);
        //Act
        IEnumerable<Tag> tags = _tagRepository.GetAllTags();
        //Assert  
        Assert.IsTrue(tags.Any());
    }

    public static void TagCompare(Tag tag, Tag tag2)
    {
        Assert.AreEqual(tag.Id, tag2.Id);
        Assert.AreEqual(tag.Description, tag2.Description);
        Assert.AreEqual(tag.Color, tag2.Color);
        Assert.AreEqual(tag.Name, tag2.Name);
        foreach (Guid parentTagId in tag.ParentTagIds)
        {
            Assert.IsTrue(tag2.ParentTagIds.Any(t => t == parentTagId));
        }
        foreach (Guid parentTagId in tag2.ParentTagIds)
        {
            Assert.IsTrue(tag.ParentTagIds.Any(t => t == parentTagId));
        }
    }
}
