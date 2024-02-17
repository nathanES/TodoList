using Moq;
using TodoList.Domain.Entities;
using TodoList.Domain.Exceptions;
using TodoList.Domain.Interfaces.Logger;
using TodoList.Domain.Interfaces.Repositories;
using TodoList.Infrastructure.Loggers;
using TodoList.Infrastructure.Repositories;
using TodoList.Infrastructure.UnitTest.Compare;

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
    public void TagCRUD_AddTag_ShouldAddTag()
    {
        string tagName = nameof(TagCRUD_AddTag_ShouldAddTag);
        Tag tag = new Tag.TagBuilder(tagName)
            .Build();

        //Act
        bool addResult = _tagRepository.AddTag(tag);
        //Assert
        Assert.IsTrue(addResult);
        Assert.IsTrue(_tagRepository.GetAllTags().Any(t => t.Id == tag.Id));
    }
    [TestMethod]
    [DataRow("AddTagWithAllParameters", "Description", "#000000")]
    public void TagCRUD_AddTagWithAllParameters_ShouldAddTag(string tagName, string tagDescription, string tagColor)
    {
        //Arrange
        Color tagColorValue = new(tagColor);
        Guid parentTagID = Guid.NewGuid();

        Tag tag = new Tag.TagBuilder(tagName)
                .SetDescription(tagDescription)
                .SetColor(tagColorValue)
                .SetParentTagIds(new List<Guid>() { parentTagID })
                .Build();

        //Act
        bool addResult = _tagRepository.AddTag(tag);
        //Assert
        Tag tagResult = _tagRepository.GetTagById(tag.Id);
        Assert.IsTrue(addResult);
        Assert.IsTrue(_tagRepository.GetAllTags().Any(t => t.Id == tag.Id));
        Assert.AreEqual(tagName, tagResult.Name);
        Assert.AreEqual(tagDescription, tagResult.Description);
        Assert.AreEqual(tagColorValue, tagResult.Color);
        Assert.IsTrue(tagResult.ParentTagIds.Any(t => t == parentTagID));
    }
    [TestMethod]
    [ExpectedException(typeof(DuplicateKeyException))]
    public void TagCRUD_AddTagAlreadyExisting_DuplicateKeyException()
    {
        //Arrange
        string tagName = nameof(TagCRUD_AddTagAlreadyExisting_DuplicateKeyException);
        Tag tag = new Tag.TagBuilder(tagName)
            .Build();

        bool addResult = _tagRepository.AddTag(tag); //First Add
        //Act
        _ = _tagRepository.AddTag(tag); //Second Add with same id
    }

    [TestMethod]
    public void TagCRUD_DeleteTag_ShouldDeleteTag()
    {
        //Arrange
        string tagName = nameof(TagCRUD_DeleteTag_ShouldDeleteTag);
        Tag tag = new Tag.TagBuilder(tagName)
            .Build();

        _ = _tagRepository.AddTag(tag);
        //Act
        bool deleteResult = _tagRepository.DeleteTagById(tag.Id);
        //Assert
        Assert.IsTrue(deleteResult);
        Assert.IsFalse(_tagRepository.GetAllTags().Any(t => t.Id == tag.Id));
    }
    [TestMethod]
    public void TagCRUD_DeleteTags_ShouldDeleteTags()
    {
        //Arrange
        string tagName = nameof(TagCRUD_DeleteTags_ShouldDeleteTags);
        Tag tag = new Tag.TagBuilder(tagName)
            .Build();

        string tagName2 = nameof(TagCRUD_DeleteTags_ShouldDeleteTags) + "2";
        Tag tag2 = new Tag.TagBuilder(tagName2)
          .Build();

        _ = _tagRepository.AddTag(tag);
        _ = _tagRepository.AddTag(tag2);

        //Act
        bool deleteResult = _tagRepository.DeleteTagByIds(new List<Guid>() { tag.Id, tag2.Id });
        //Assert
        Assert.IsTrue(deleteResult);
        Assert.IsFalse(_tagRepository.GetAllTags().Any(t => t.Id == tag.Id));
        Assert.IsFalse(_tagRepository.GetAllTags().Any(t => t.Id == tag2.Id));
    }
    [TestMethod]
    public void TagCRUD_DeleteTagWhenNotExisting_ShouldReturnFalse()
    {
        //Act
        bool deleteResult = _tagRepository.DeleteTagById(Guid.NewGuid());
        //Assert
        Assert.IsFalse(deleteResult);
    }
    [TestMethod]
    public void TagCRUD_DeleteTagsWhenNotExisting_ShouldReturnFalse()
    {
        //Arrange
        bool deleteResult = _tagRepository.DeleteTagByIds(new List<Guid>() { Guid.NewGuid(), Guid.NewGuid(), });
        //Assert
        Assert.IsFalse(deleteResult);
    }

    [TestMethod]
    [DataRow("UpdateTag2", "Description", "#008760")]
    public void TagCRUD_UpdateTag_ShouldReturnFalse(string newTagName, string newTagDescription, string newTagColor)
    {
        //Arrange
        string tagName = nameof(TagCRUD_UpdateTag_ShouldReturnFalse);

        Tag tag = new Tag.TagBuilder(tagName)
            .SetDescription("First Description")
            .SetColor(new Color("#098734"))
            .Build();

        _ = _tagRepository.AddTag(tag);

        Color newTagColorValue = new(newTagColor);
        Guid newParentTagID = Guid.NewGuid();
        tag.UpdateName(newTagName);
        tag.UpdateDescription(newTagDescription);
        tag.UpdateColor(newTagColorValue);
        tag.UpdateParentTagIds(new List<Guid>() { newParentTagID });
        //Act
        bool updateResult = _tagRepository.UpdateTag(tag);
        //Assert
        Tag tagResult = _tagRepository.GetTagById(tag.Id);
        Assert.IsTrue(updateResult);
        Assert.AreEqual(newTagName, tagResult.Name);
        Assert.AreEqual(newTagDescription, tagResult.Description);
        Assert.AreEqual(newTagColorValue, tagResult.Color);
        Assert.IsTrue(tagResult.ParentTagIds.Any(t => t == newParentTagID));
    }
    [TestMethod]
    public void TagCRUD_UpdateTagWhenNotExisting_ShouldReturnFalse()
    {
        //Arrange
        string tagName = nameof(TagCRUD_UpdateTagWhenNotExisting_ShouldReturnFalse);
        Tag tag = new Tag.TagBuilder(tagName)
            .SetDescription("description Not existing")
            .Build();

        //Act
        bool updateResult = _tagRepository.UpdateTag(tag);
        //Assert
        Assert.IsFalse(updateResult);
        Assert.AreEqual(Tag.Default, _tagRepository.GetTagById(tag.Id));
    }

    [TestMethod]
    public void TagCRUD_GetTagByIdWhenFound_ShouldGetTag()
    {
        //Arrange
        string tagName = nameof(TagCRUD_GetTagByIdWhenFound_ShouldGetTag);
        Tag tag = new Tag.TagBuilder(tagName)
            .Build();
        _ = _tagRepository.AddTag(tag);
        //Act
        Tag tagFound = _tagRepository.GetTagById(tag.Id);
        //Assert
        CompareEntity.TagCompare(tagFound, tag);
    }
    [TestMethod]
    public void TagCRUD_GetTagByIdWhenFound_ShouldGetTagDefault()
    {
        //Arrange
        string tagName = nameof(TagCRUD_GetTagByIdWhenFound_ShouldGetTagDefault);
        Tag tag = new Tag.TagBuilder(tagName)
            .Build();
        //Act
        Tag tagFound = _tagRepository.GetTagById(tag.Id);
        //Assert
        CompareEntity.TagCompare(tagFound, Tag.Default);
    }
    //TODO continuer le rework
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

    [TestMethod]
    [DataRow("UpdateTag", "UpdateTag2", "Description", "#000000")]
    public void UpdateTagName_ShouldUpdateTagName(string name, string updatedName, string description, string color)
    {
        //Arrange
        Tag tag = new Tag.TagBuilder(name)
            .SetDescription(description)
            .SetColor(new Color(color))
            .Build();

        _ = _tagRepository.AddTag(tag);

        //Act
        bool updateResult = _tagRepository.UpdateTagName(tag.Id, updatedName);
        //Assert
        Assert.IsTrue(updateResult);
        Assert.IsTrue(_tagRepository.GetAllTags().Any(t => t.Name == updatedName));
    }
    [TestMethod]
    [DataRow("UpdateTagNotFound")]
    public void UpdateTagName_ShouldNotUpdateTagName_NotFound(string updatedName)
    {
        //Arrange
        Guid id = Guid.NewGuid();

        //Act
        bool updateResult = _tagRepository.UpdateTagName(id, updatedName);
        //Assert
        Assert.IsFalse(updateResult);
        Assert.IsFalse(_tagRepository.GetAllTags().Any(t => t.Name == updatedName));
    }
    [TestMethod]
    [DataRow("UpdateTag", "UpdateTag2", "Description", "#000000")]
    public void UpdateTagDescription_ShouldUpdateTagDescription(string name, string updatedDescription, string description, string color)
    {
        //Arrange
        Tag tag = new Tag.TagBuilder(name)
            .SetDescription(description)
            .SetColor(new Color(color))
            .Build();

        _ = _tagRepository.AddTag(tag);

        //Act
        bool updateResult = _tagRepository.UpdateTagDescription(tag.Id, updatedDescription);
        //Assert
        Assert.IsTrue(updateResult);
        Assert.IsTrue(_tagRepository.GetAllTags().Any(t => t.Description == updatedDescription));
    }
    [TestMethod]
    [DataRow("UpdateTag")]
    public void UpdateTagDescription_ShouldNotUpdateTagDescription_NotFound(string updatedDescription)
    {
        //Arrange
        Guid id = Guid.NewGuid();

        //Act
        bool updateResult = _tagRepository.UpdateTagDescription(id, updatedDescription);
        //Assert
        Assert.IsFalse(updateResult);
        Assert.IsFalse(_tagRepository.GetAllTags().Any(t => t.Description == updatedDescription));
    }

    [TestMethod]
    [DataRow("UpdateTag", "Description", "#000000", "#000000")]
    public void UpdateTagColor_ShouldUpdateTagColor(string name, string description, string color, string updatedColor)
    {
        //Arrange
        Tag tag = new Tag.TagBuilder(name)
            .SetDescription(description)
            .SetColor(new Color(color))
            .Build();

        _ = _tagRepository.AddTag(tag);

        //Act
        bool updateResult = _tagRepository.UpdateTagColor(tag.Id, new Color(updatedColor));
        //Assert
        Assert.IsTrue(updateResult);
        Assert.IsTrue(_tagRepository.GetAllTags().Any(t => t.Color.Equals(new Color(updatedColor))));
    }
    [TestMethod]
    [DataRow("#987873")]
    public void UpdateTagColor_ShouldNotUpdateTagColor_NotFound(string updatedColor)
    {
        //Arrange
        Guid id = Guid.NewGuid();

        //Act
        bool updateResult = _tagRepository.UpdateTagColor(id, new Color(updatedColor));
        //Assert
        Assert.IsFalse(updateResult);
        Assert.IsFalse(_tagRepository.GetAllTags().Where(t => t.Color.Equals(new Color(updatedColor))).Any());
    }

    [TestMethod]
    [DataRow("UpdateTag", "Description", "#000000")]
    public void UpdateTagParentTag_ShouldUpdateTagParentTag(string name, string description, string color)
    {
        //Arrange
        Tag tagParent1 = new Tag.TagBuilder("ParentTag1").Build();
        Tag tagParent2 = new Tag.TagBuilder("ParentTag2").Build();
        Tag tagParent3 = new Tag.TagBuilder("ParentTag3").Build();
        List<Guid> parentTagIds = new() { tagParent1.Id, tagParent2.Id };
        Tag tag = new Tag.TagBuilder(name)
            .SetDescription(description)
            .SetColor(new Color(color))
            .SetParentTagIds(parentTagIds)
            .Build();

        _ = _tagRepository.AddTag(tag);

        parentTagIds.Add(tagParent3.Id);

        //Act
        bool updateResult = _tagRepository.UpdateTagParentTag(tag.Id, parentTagIds);

        //Assert
        Assert.IsTrue(updateResult);
        Assert.IsTrue(_tagRepository.GetAllTags().Any(t => t.ParentTagIds.SetEquals(new HashSet<Guid>(parentTagIds))));
    }
    [TestMethod]
    public void UpdateTagParentTag_ShouldNotUpdateTagParentTag_NotFound()
    {
        //Arrange
        Tag tagParent1 = new Tag.TagBuilder("ParentTag1").Build();
        Tag tagParent2 = new Tag.TagBuilder("ParentTag2").Build();
        Tag tagParent3 = new Tag.TagBuilder("ParentTag3").Build();
        List<Guid> parentTagIds = new() { tagParent1.Id, tagParent2.Id, tagParent3.Id };
        Guid id = Guid.NewGuid();
        //Act
        bool updateResult = _tagRepository.UpdateTagParentTag(id, parentTagIds);

        //Assert
        Assert.IsFalse(updateResult);
        Assert.IsFalse(_tagRepository.GetAllTags().Any(t => t.ParentTagIds.Equals(new HashSet<Guid>(parentTagIds))));
    }
    [TestMethod]
    [DataRow("UpdateTag", "Description", "#000000")]
    public void AddTagParentTag_ShouldAddTagParentTag(string name, string description, string color)
    {
        //Arrange
        Tag tagParent1 = new Tag.TagBuilder("ParentTag1").Build();
        Tag tagParent2 = new Tag.TagBuilder("ParentTag2").Build();
        List<Guid> parentTagIds = new() { tagParent1.Id, tagParent2.Id };
        Tag tag = new Tag.TagBuilder(name)
            .SetDescription(description)
            .SetColor(new Color(color))
            .SetParentTagIds(parentTagIds)
            .Build();

        _ = _tagRepository.AddTag(tag);

        Tag tagParent3 = new Tag.TagBuilder("ParentTag3").Build();

        //Act
        bool updateResult = _tagRepository.AddTagParentTag(tag.Id, tagParent3.Id);

        //Assert
        Assert.IsTrue(updateResult);
        Assert.IsTrue(_tagRepository.GetAllTags().Select(t => t.ParentTagIds.Select(t => t == tagParent1.Id)).Any());
        Assert.IsTrue(_tagRepository.GetAllTags().Select(t => t.ParentTagIds.Select(t => t == tagParent2.Id)).Any());
        Assert.IsTrue(_tagRepository.GetAllTags().Select(t => t.ParentTagIds.Select(t => t == tagParent3.Id)).Any());
    }
    [TestMethod]
    public void AddTagParentTag_ShouldNotAddTagParentTag_NotFound()
    {
        //Arrange
        Tag tagParent1 = new Tag.TagBuilder("ParentTag1").Build();
        Tag tagParent2 = new Tag.TagBuilder("ParentTag2").Build();
        Tag tagParent3 = new Tag.TagBuilder("ParentTag3").Build();
        List<Guid> parentTagIds = new() { tagParent1.Id, tagParent2.Id };
        Guid id = Guid.NewGuid();

        //Act
        bool updateResult = _tagRepository.AddTagParentTag(id, tagParent3.Id);

        //Assert
        Assert.IsFalse(updateResult);
        Assert.IsFalse(_tagRepository.GetAllTags().Where(t => t.ParentTagIds.Contains(tagParent3.Id)).Any());
    }

    [TestMethod]
    [DataRow("UpdateTag", "Description", "#000000")]
    public void RemoveTagParentTag_ShouldRemoveTagParentTag(string name, string description, string color)
    {
        //Arrange
        Tag tagParent1 = new Tag.TagBuilder("ParentTag1").Build();
        Tag tagParent2 = new Tag.TagBuilder("ParentTag2").Build();
        List<Guid> parentTagIds = new() { tagParent1.Id, tagParent2.Id };
        Tag tag = new Tag.TagBuilder(name)
            .SetDescription(description)
            .SetColor(new Color(color))
            .SetParentTagIds(parentTagIds)
            .Build();

        _ = _tagRepository.AddTag(tag);

        //Act
        bool updateResult = _tagRepository.RemoveTagParentTag(tag.Id, tagParent2.Id);

        //Assert
        Assert.IsTrue(updateResult);
        Assert.IsTrue(_tagRepository.GetAllTags().Where(t => t.ParentTagIds.Contains(tagParent1.Id)).Any());
        Assert.IsFalse(_tagRepository.GetAllTags().Where(t => t.ParentTagIds.Contains(tagParent2.Id)).Any());
    }
    [TestMethod]
    public void RemoveTagParentTag_ShouldNotRemoveTagParentTag_NotFound()
    {
        //Arrange
        Tag tagParent1 = new Tag.TagBuilder("ParentTag1").Build();
        Tag tagParent2 = new Tag.TagBuilder("ParentTag2").Build();
        List<Guid> parentTagIds = new() { tagParent1.Id, tagParent2.Id };
        Guid id = Guid.NewGuid();

        //Act
        bool updateResult = _tagRepository.RemoveTagParentTag(id, tagParent2.Id);

        //Assert
        Assert.IsFalse(updateResult);
        Assert.IsFalse(_tagRepository.GetAllTags().Where(t => t.ParentTagIds.Contains(tagParent2.Id)).Any());
    }
}
