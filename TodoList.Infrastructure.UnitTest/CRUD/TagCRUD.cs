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
        Assert.AreEqual(tag, tagFound);
    }
    [TestMethod]
    public void TagCRUD_GetTagByIdWhenNotFound_ShouldGetTagDefault()
    {
        //Arrange
        string tagName = nameof(TagCRUD_GetTagByIdWhenNotFound_ShouldGetTagDefault);
        Tag tag = new Tag.TagBuilder(tagName)
            .Build();
        //Act
        Tag tagFound = _tagRepository.GetTagById(tag.Id);
        //Assert
        Assert.AreEqual(Tag.Default, tagFound);
    }
    [TestMethod]
    public void TagCRUD_GetAllTag_ShouldGetAllTags()
    {
        //Arrange
        string tagName = nameof(TagCRUD_GetAllTag_ShouldGetAllTags);
        Tag tag = new Tag.TagBuilder(tagName)
            .Build();
        _ = _tagRepository.AddTag(tag);
        //Act
        IEnumerable<Tag> tags = _tagRepository.GetAllTags();
        //Assert  
        Assert.IsTrue(tags.Any());
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
        Tag tagResult = _tagRepository.GetTagById(tag.Id);
        Assert.IsTrue(addResult);
        Assert.AreNotEqual(Tag.Default, tagResult);
        Assert.AreEqual(tagName, tagResult.Name);
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
    public void TagCRUD_DeleteTagWhenNotFound_ShouldReturnFalse()
    {
        //Act
        bool deleteResult = _tagRepository.DeleteTagById(Guid.NewGuid());
        //Assert
        Assert.IsFalse(deleteResult);
    }
    [TestMethod]
    public void TagCRUD_DeleteTagsWhenNotFound_ShouldReturnFalse()
    {
        //Arrange
        bool deleteResult = _tagRepository.DeleteTagByIds(new List<Guid>() { Guid.NewGuid(), Guid.NewGuid(), });
        //Assert
        Assert.IsFalse(deleteResult);
    }

    [TestMethod]
    [DataRow("UpdateTag2", "Description", "#008760")]
    public void TagCRUD_UpdateTagAllProperties_ShouldUpdateAllProperties(string newTagName, string newTagDescription, string newTagColor)
    {
        //Arrange
        string tagName = nameof(TagCRUD_UpdateTagAllProperties_ShouldUpdateAllProperties);

        Guid parentTagID = Guid.NewGuid();
        Tag tag = new Tag.TagBuilder(tagName)
            .SetDescription("First Description")
            .SetColor(new Color("#098734"))
            .SetParentTagIds(new List<Guid>() { Guid.NewGuid() })
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
    [DataRow("UpdateTag2", "Description", "#008760")]
    public void TagCRUD_UpdateTagAllPropertiesWithNoProperties_ShouldUpdateAllProperties(string newTagName, string newTagDescription, string newTagColor)
    {
        //Arrange
        string tagName = nameof(TagCRUD_UpdateTagAllPropertiesWithNoProperties_ShouldUpdateAllProperties);

        Tag tag = new Tag.TagBuilder(tagName)
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
    public void TagCRUD_UpdateTagAllPropertiesWhenNotExisting_ShouldReturnFalse()
    {
        //Arrange
        string tagName = nameof(TagCRUD_UpdateTagAllPropertiesWhenNotExisting_ShouldReturnFalse);
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
    [DataRow("UpdateTag")]
    public void TagCRUD_UpdateTagName_ShouldUpdateTagName(string newTagName)
    {
        //Arrange
        string tagName = nameof(TagCRUD_UpdateTagName_ShouldUpdateTagName);
        Tag tag = new Tag.TagBuilder(tagName)
            .Build();

        _ = _tagRepository.AddTag(tag);

        //Act
        bool updateResult = _tagRepository.UpdateTagName(tag.Id, newTagName);
        //Assert
        Assert.IsTrue(updateResult);
        Assert.AreEqual(newTagName, _tagRepository.GetTagById(tag.Id).Name);
    }
    [TestMethod]
    [DataRow("UpdateTagNotFound")]
    public void TagCRUD_UpdateTagNameWhenNotFound_ShouldReturnFalse(string newTagName)
    {
        //Arrange
        Guid tagID = Guid.NewGuid();

        //Act
        bool updateResult = _tagRepository.UpdateTagName(tagID, newTagName);
        //Assert
        Assert.IsFalse(updateResult);
        Assert.AreEqual(Tag.Default, _tagRepository.GetTagById(tagID));
    }

    [TestMethod]
    [DataRow("Description")]
    public void TagCRUD_UpdateTagDescription_ShouldUpdateTagDescription(string newTagDescription)
    {
        //Arrange
        string tagName = nameof(TagCRUD_UpdateTagDescription_ShouldUpdateTagDescription);
        Tag tag = new Tag.TagBuilder(tagName)
            .SetDescription("FirstTagDescription")
            .Build();

        _ = _tagRepository.AddTag(tag);

        //Act
        bool updateResult = _tagRepository.UpdateTagDescription(tag.Id, newTagDescription);
        //Assert
        Assert.IsTrue(updateResult);
        Assert.AreEqual(newTagDescription, _tagRepository.GetTagById(tag.Id).Description);
    }
    [TestMethod]
    [DataRow("Description")]
    public void TagCRUD_UpdateTagDescriptionWithNoDescription_ShouldUpdateTagDescription(string newTagDescription)
    {
        //Arrange
        string tagName = nameof(TagCRUD_UpdateTagDescription_ShouldUpdateTagDescription);
        Tag tag = new Tag.TagBuilder(tagName)
            .Build();

        _ = _tagRepository.AddTag(tag);

        //Act
        bool updateResult = _tagRepository.UpdateTagDescription(tag.Id, newTagDescription);
        //Assert
        Assert.IsTrue(updateResult);
        Assert.AreEqual(newTagDescription, _tagRepository.GetTagById(tag.Id).Description);
    }
    [TestMethod]
    [DataRow("TagDescription")]
    public void TagCRUD_UpdateTagDescriptionWhenNotFound_ShouldReturnFalse(string newTagDescription)
    {
        //Arrange
        Guid tagID = Guid.NewGuid();

        //Act
        bool updateResult = _tagRepository.UpdateTagDescription(tagID, newTagDescription);

        //Assert
        Assert.IsFalse(updateResult);
        Assert.AreEqual(Tag.Default, _tagRepository.GetTagById(tagID));
    }

    [TestMethod]
    [DataRow("#987654")]
    public void TagCRUD_UpdateTagColor_ShouldUpdateTagColor(string newTagColor)
    {
        //Arrange
        string tagName = nameof(TagCRUD_UpdateTagColor_ShouldUpdateTagColor);
        Color newTagColorValue = new(newTagColor);
        Tag tag = new Tag.TagBuilder(tagName)
            .SetColor(new Color("#070809"))
            .Build();

        _ = _tagRepository.AddTag(tag);

        //Act
        bool updateResult = _tagRepository.UpdateTagColor(tag.Id, newTagColorValue);
        //Assert
        Assert.IsTrue(updateResult);
        Assert.AreEqual(newTagColorValue, _tagRepository.GetTagById(tag.Id).Color);
    }
    [TestMethod]
    [DataRow("#987654")]
    public void TagCRUD_UpdateTagColorWithNoColor_ShouldUpdateTagColor(string newTagColor)
    {
        //Arrange
        string tagName = nameof(TagCRUD_UpdateTagColor_ShouldUpdateTagColor);
        Color newTagColorValue = new(newTagColor);
        Tag tag = new Tag.TagBuilder(tagName)
            .Build();

        _ = _tagRepository.AddTag(tag);

        //Act
        bool updateResult = _tagRepository.UpdateTagColor(tag.Id, newTagColorValue);
        //Assert
        Assert.IsTrue(updateResult);
        Assert.AreEqual(newTagColorValue, _tagRepository.GetTagById(tag.Id).Color);
    }
    [TestMethod]
    [DataRow("#987873")]
    public void TagCRUD_UpdateTagColorWhenNotFound_ShouldReturnFalse(string newTagColor)
    {
        //Arrange
        Guid tagID = Guid.NewGuid();
        Color newTagColorValue = new(newTagColor);

        //Act
        bool updateResult = _tagRepository.UpdateTagColor(tagID, newTagColorValue);
        //Assert
        Assert.IsFalse(updateResult);
        Assert.AreEqual(Tag.Default, _tagRepository.GetTagById(tagID));
    }

    [TestMethod]
    public void TagCRUD_UpdateTagParentTag_ShouldUpdateTagParentTag()
    {
        //Arrange
        string tagName = nameof(TagCRUD_UpdateTagParentTag_ShouldUpdateTagParentTag);
        Guid tagParentID1 = Guid.NewGuid();
        Guid tagParentID2 = Guid.NewGuid();
        List<Guid> parentTagIds = new() { tagParentID1, tagParentID2 };
        Tag tag = new Tag.TagBuilder(tagName)
            .SetParentTagIds(parentTagIds)
            .Build();

        _ = _tagRepository.AddTag(tag);

        Guid tagParentID3 = Guid.NewGuid();
        parentTagIds.Add(tagParentID3);

        //Act
        bool updateResult = _tagRepository.UpdateTagParentTag(tag.Id, parentTagIds);

        //Assert
        Assert.IsTrue(updateResult);
        Assert.IsTrue(_tagRepository.GetTagById(tag.Id).ParentTagIds.SetEquals(new HashSet<Guid>(parentTagIds)));
    }
    [TestMethod]
    public void TagCRUD_UpdateTagParentTagWithNoTagParent_ShouldUpdateTagParentTag()
    {
        //Arrange
        string tagName = nameof(TagCRUD_UpdateTagParentTag_ShouldUpdateTagParentTag);
        Guid tagParentID1 = Guid.NewGuid();
        Guid tagParentID2 = Guid.NewGuid();
        List<Guid> parentTagIds = new() { tagParentID1, tagParentID2 };
        Tag tag = new Tag.TagBuilder(tagName)
            .Build();

        _ = _tagRepository.AddTag(tag);

        Guid tagParentID3 = Guid.NewGuid();
        parentTagIds.Add(tagParentID3);

        //Act
        bool updateResult = _tagRepository.UpdateTagParentTag(tag.Id, parentTagIds);

        //Assert
        Assert.IsTrue(updateResult);
        Assert.IsTrue(_tagRepository.GetTagById(tag.Id).ParentTagIds.SetEquals(new HashSet<Guid>(parentTagIds)));
    }
    [TestMethod]
    public void TagCRUD_UpdateTagParentTagWhenNotFound_ShouldReturnFalse()
    {
        Guid tagParentID1 = Guid.NewGuid();
        Guid tagParentID2 = Guid.NewGuid();
        List<Guid> parentTagIds = new() { tagParentID1, tagParentID2 };
        Guid tagID = Guid.NewGuid();
        //Act
        bool updateResult = _tagRepository.UpdateTagParentTag(tagID, parentTagIds);

        //Assert
        Assert.IsFalse(updateResult);
        Assert.AreEqual(Tag.Default, _tagRepository.GetTagById(tagID));
    }
    [TestMethod]
    public void TagCRUD_UpdateTagAddParentTag_ShouldUpdateAddParentTag()
    {
        //Arrange
        Guid tagParentID1 = Guid.NewGuid();
        Guid tagParentID2 = Guid.NewGuid();
        List<Guid> parentTagIds = new() { tagParentID1, tagParentID2 };

        string tagName = nameof(TagCRUD_UpdateTagAddParentTag_ShouldUpdateAddParentTag);
        Tag tag = new Tag.TagBuilder(tagName)
            .SetParentTagIds(parentTagIds)
            .Build();

        _ = _tagRepository.AddTag(tag);
        Guid tagParentID3 = Guid.NewGuid();

        //Act
        bool updateResult = _tagRepository.AddTagParentTag(tag.Id, tagParentID3);

        //Assert
        Tag tagResult = _tagRepository.GetTagById(tag.Id);
        Assert.IsTrue(updateResult);
        Assert.IsTrue(tagResult.ParentTagIds.Contains(tagParentID1));
        Assert.IsTrue(tagResult.ParentTagIds.Contains(tagParentID2));
        Assert.IsTrue(tagResult.ParentTagIds.Contains(tagParentID3));
    }
    [TestMethod]
    public void TagCRUD_UpdateTagAddParentTagWithNoTagParent_ShouldUpdateAddParentTag()
    {
        //Arrange
        string tagName = nameof(TagCRUD_UpdateTagAddParentTag_ShouldUpdateAddParentTag);
        Tag tag = new Tag.TagBuilder(tagName)
            .Build();

        _ = _tagRepository.AddTag(tag);
        Guid tagParentID1 = Guid.NewGuid();

        //Act
        bool updateResult = _tagRepository.AddTagParentTag(tag.Id, tagParentID1);

        //Assert
        Tag tagResult = _tagRepository.GetTagById(tag.Id);
        Assert.IsTrue(updateResult);
        Assert.IsTrue(tagResult.ParentTagIds.Contains(tagParentID1));
    }
    [TestMethod]
    public void TagCRUD_UpdateTagAddParentTagWhenNotFound_ShouldReturnFalse()
    {
        //Arrange
        Guid tagID = Guid.NewGuid();
        Guid tagParentID3 = Guid.NewGuid();

        //Act
        bool updateResult = _tagRepository.AddTagParentTag(tagID, tagParentID3);

        //Assert
        Assert.IsFalse(updateResult);
        Assert.AreEqual(Tag.Default, _tagRepository.GetTagById(tagID));
    }
    [TestMethod]
    public void TagCRUD_UpdateTagRemoveParentTag_ShouldUpdateRemoveParentTag()
    {
        //Arrange
        string tagName = nameof(TagCRUD_UpdateTagRemoveParentTag_ShouldUpdateRemoveParentTag);
        Guid tagParentID1 = Guid.NewGuid();
        Guid tagParentID2 = Guid.NewGuid();
        List<Guid> parentTagIds = new() { tagParentID1, tagParentID2 };
        Tag tag = new Tag.TagBuilder(tagName)
            .SetParentTagIds(parentTagIds)
            .Build();

        _ = _tagRepository.AddTag(tag);

        //Act
        bool updateResult = _tagRepository.RemoveTagParentTag(tag.Id, tagParentID2);

        //Assert
        Assert.IsTrue(updateResult);
        Assert.IsTrue(_tagRepository.GetTagById(tag.Id).ParentTagIds.Contains(tagParentID1));
        Assert.IsFalse(_tagRepository.GetTagById(tag.Id).ParentTagIds.Contains(tagParentID2));
    }
    [TestMethod]
    public void TagCRUD_UpdateTagRemoveParentTagWhenNotFound_ShouldReturnFalse()
    {
        Guid tagParentID1 = Guid.NewGuid();
        Guid tagParentID2 = Guid.NewGuid();
        _ = new List<Guid>() { tagParentID1, tagParentID2 };
        Guid id = Guid.NewGuid();

        //Act
        bool updateResult = _tagRepository.RemoveTagParentTag(id, tagParentID2);

        //Assert
        Assert.IsFalse(updateResult);
        Assert.AreEqual(Tag.Default, _tagRepository.GetTagById(id));
    }
    [TestMethod]
    public void TagCRUD_UpdateTagRemoveParentTagWhenNoTagParentForThisId_ShouldReturnFalse()
    {
        //Arrange
        string tagName = nameof(TagCRUD_UpdateTagRemoveParentTagWhenNoTagParentForThisId_ShouldReturnFalse);
        Guid tagParentID1 = Guid.NewGuid();
        Guid tagParentID2 = Guid.NewGuid();
        List<Guid> parentTagIds = new() { tagParentID1, tagParentID2 };
        Tag tag = new Tag.TagBuilder(tagName)
            .SetParentTagIds(parentTagIds)
            .Build();

        _ = _tagRepository.AddTag(tag);

        //Act
        bool updateResult = _tagRepository.RemoveTagParentTag(tag.Id, Guid.NewGuid());

        //Assert
        Assert.IsFalse(updateResult);
        Assert.IsTrue(_tagRepository.GetTagById(tag.Id).ParentTagIds.Contains(tagParentID1));
        Assert.IsTrue(_tagRepository.GetTagById(tag.Id).ParentTagIds.Contains(tagParentID2));
    }
}
