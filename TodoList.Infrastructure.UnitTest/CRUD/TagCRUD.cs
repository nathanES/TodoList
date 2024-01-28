using Moq;
using TodoList.Domain.Entities;
using TodoList.Domain.Interfaces.Logger;
using TodoList.Domain.Interfaces.Repositories;
using TodoList.Infrastructure.Loggers;
using TodoList.Infrastructure.Repositories;

namespace TodoList.Infrastructure.UnitTest.CRUD;

[TestClass]
public class TagCRUD
{
    private ITagRepository? tagRepository;
    private ILogger? logger;
    private readonly Mock<ILogDestination> logDestination = new();

    [TestInitialize]
    public void TagCRUDInitialize()
    {
        logger = new LoggerCustom(logDestination.Object);
        tagRepository = new TagRepositoryJson(logger);
    }

    //TODO : rajouter des cas non passant
    [TestMethod]
    [DataRow("AddTag", "AddTagParent", "Description 1", "#000000")]
    public void AddTag(string name, string parentName, string description, string color)
    {
        //Arrange
        Tag tagParent = new Tag.TagBuilder(Guid.NewGuid().ToString(), parentName)
            .Build();
        _ = tagRepository.AddTag(tagParent);
        Assert.IsTrue(tagRepository.GetAllTags().Any(t => t.Id == tagParent.Id));

        Tag tag = new Tag.TagBuilder(Guid.NewGuid().ToString(), name)
            .SetDescription(description)
            .SetColor(new Domain.ValueObjects.Color(color))
            .SetParentTagIds(new List<string>() { tagParent.Id })
            .Build();
        //Act
        bool addResult = tagRepository.AddTag(tag);
        //Assert
        Assert.IsTrue(addResult);
        Assert.IsTrue(tagRepository.GetAllTags().Any(t => t.Id == tag.Id));

    }

    [TestMethod]
    [DataRow("DeleteTag", "Description", "#000000")]
    public void DeleteTag(string name, string description, string color)
    {
        //Arrange

        Tag tag = new Tag.TagBuilder(Guid.NewGuid().ToString(), name)
            .SetDescription(description)
            .SetColor(new Domain.ValueObjects.Color(color))
            .Build();

        _ = tagRepository.AddTag(tag);
        //Act
        bool deleteResult = tagRepository.DeleteTagById(tag.Id);
        //Assert
        Assert.IsTrue(deleteResult);
        Assert.IsFalse(tagRepository.GetAllTags().Any(t => t.Id == tag.Id));
    }

    [TestMethod]
    [DataRow("DeleteTags", "Description", "#000000")]
    public void DeleteTags(string name, string description, string color)
    {
        //Arrange
        Tag tag = new Tag.TagBuilder(Guid.NewGuid().ToString(), name)
            .SetDescription(description)
            .SetColor(new Domain.ValueObjects.Color(color))
            .Build();

        _ = tagRepository.AddTag(tag);

        Tag tag2 = new Tag.TagBuilder(Guid.NewGuid().ToString(), name)
          .SetDescription(description)
          .SetColor(new Domain.ValueObjects.Color(color))
          .Build();

        _ = tagRepository.AddTag(tag2);

        //Act
        bool deleteResult = tagRepository.DeleteTagByIds(new List<string>() { tag.Id, tag2.Id });
        //Assert
        Assert.IsTrue(deleteResult);
        Assert.IsFalse(tagRepository.GetAllTags().Any(t => t.Id == tag.Id));
        Assert.IsFalse(tagRepository.GetAllTags().Any(t => t.Id == tag2.Id));
    }

    [TestMethod]
    [DataRow("UpdateTag", "UpdateTag2", "Description", "#000000")]
    public void UpdateTag(string name, string updatedName, string description, string color)
    {
        //Arrange
        Tag tag = new Tag.TagBuilder(Guid.NewGuid().ToString(), name)
            .SetDescription(description)
            .SetColor(new Domain.ValueObjects.Color(color))
            .Build();

        _ = tagRepository.AddTag(tag);

        tag.UpdateName(updatedName);
        //Act
        bool updateResult = tagRepository.UpdateTag(tag);
        //Assert
        Assert.IsTrue(updateResult);
        Assert.IsTrue(tagRepository.GetAllTags().Any(t => t.Name == updatedName));
    }
    [TestMethod]
    [DataRow("UpdateTagNotExisted", "UpdateTagNotExisted2", "Description", "#000000")]
    public void UpdateTag_NotExisting(string name, string updatedName, string description, string color)
    {
        //Arrange
        Tag tag = new Tag.TagBuilder(Guid.NewGuid().ToString(), name)
            .SetDescription(description)
            .SetColor(new Domain.ValueObjects.Color(color))
            .Build();

        tag.UpdateName(updatedName);
        //Act
        bool updateResult = tagRepository.UpdateTag(tag);
        //Assert
        Assert.IsFalse(updateResult);
    }
    [TestMethod]
    [DataRow("GetTagById", "Description", "#000000")]
    public void GetTagById(string name, string description, string color)
    {
        //Arrange
        Tag tag = new Tag.TagBuilder(Guid.NewGuid().ToString(), name)
            .SetDescription(description)
            .SetColor(new Domain.ValueObjects.Color(color))
            .Build();
        _ = tagRepository.AddTag(tag);
        //Act
        Tag tagFound = tagRepository.GetTagById(tag.Id);
        //Assert
        TagCompare(tagFound, tag);
    }
    [TestMethod]
    [DataRow("GetTagById", "Description", "#000000")]
    public void GetTagById_NotFound(string name, string description, string color)
    {
        //Arrange
        Tag tag = new Tag.TagBuilder(Guid.NewGuid().ToString(), name)
            .SetDescription(description)
            .SetColor(new Domain.ValueObjects.Color(color))
            .Build();
        //Act
        Tag tagFound = tagRepository.GetTagById(tag.Id);
        //Assert
        TagCompare(tagFound, Tag.Empty);
    }

    [TestMethod]
    [DataRow("GetAllTags", "Description", "#000000")]
    public void GetAllTags(string name, string description, string color)
    {
        //Arrange
        Tag tag = new Tag.TagBuilder(Guid.NewGuid().ToString(), name)
            .SetDescription(description)
            .SetColor(new Domain.ValueObjects.Color(color))
            .Build();
        _ = tagRepository.AddTag(tag);
        //Act
        _ = tagRepository.GetAllTags();
        //Assert  
        Assert.IsTrue(tagRepository.GetAllTags().Any());
    }

    public static void TagCompare(Tag tag, Tag tag2)
    {
        Assert.AreEqual(tag.Id, tag2.Id);
        Assert.AreEqual(tag.Description, tag2.Description);
        Assert.AreEqual(tag.Color, tag2.Color);
        Assert.AreEqual(tag.Name, tag2.Name);
        foreach (string parentTagId in tag.ParentTagIds)
        {
            Assert.IsTrue(tag2.ParentTagIds.Any(t => t == parentTagId));
        }
        foreach (string parentTagId in tag2.ParentTagIds)
        {
            Assert.IsTrue(tag.ParentTagIds.Any(t => t == parentTagId));
        }
    }
}
