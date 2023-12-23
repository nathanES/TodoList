using System.Globalization;
using System.Xml.Linq;
using TodoList.Domain.Entities;
using TodoList.Domain.Enum;
using TodoList.Domain.Interfaces;
using TodoList.Infrastructure.Repositories;

namespace TodoList.Infrastructure.UnitTest
{
  [TestClass]
  public class TagCRUD
  {
    [TestMethod]
    [DataRow("AddTag", "AddTagParent", "Description 1", "#000000")]
    public void AddTag(string name, string parentName, string description, string color )
    {
      //Arrange
      ITagRepository tagRepository = new TagRepositoryJson();
      Tag tagParent = new Tag.TagBuilder()
          .SetName(parentName)
          .Build();
      tagRepository.AddTag(tagParent);
      Assert.IsTrue(tagRepository.GetAllTags().Any(t => t.Id == tagParent.Id));

      Tag tag = new Tag.TagBuilder()
          .SetName(name)
          .SetDescription(description)
          .SetColor(new Domain.ValueObjects.Color(color))
          .SetParentTagIds(new List<string>() { tagParent.Id})
          .Build();
      //Act
      tagRepository.AddTag(tag);
      //Assert
      Assert.IsTrue(tagRepository.GetAllTags().Any(t => t.Id == tag.Id));

    }

    [TestMethod]
    [DataRow("DeleteTag", "Description", "#000000")]
    public void DeleteTag(string name, string description, string color)
    {
      //Arrange
      ITagRepository tagRepository = new TagRepositoryJson();

      Tag tag = new Tag.TagBuilder()
          .SetName(name)
          .SetDescription(description)
          .SetColor(new Domain.ValueObjects.Color(color))
          .Build();

      tagRepository.AddTag(tag);
      //Act
      tagRepository.DeleteTag(tag);
      //Assert
      Assert.IsFalse(tagRepository.GetAllTags().Any(t => t.Id == tag.Id));
    }

    [TestMethod]
    [DataRow("UpdateTag", "UpdateTag2", "Description", "#000000")]
    public void UpdateTag(string name, string updatedName, string description, string color)
    {
      //Arrange
      ITagRepository tagRepository = new TagRepositoryJson();

      Tag tag = new Tag.TagBuilder()
          .SetName(name)
          .SetDescription(description)
          .SetColor(new Domain.ValueObjects.Color(color))
          .Build();

      tagRepository.AddTag(tag);

      tag.UpdateName(updatedName);
      //Act
      tagRepository.UpdateTag(tag);
      //Assert
      Assert.IsTrue(tagRepository.GetAllTags().Any(t => t.Name == updatedName));
    }

    [TestMethod]
    [DataRow("GetTagById", "Description", "#000000")]
    public void GetTagById(string name, string description, string color)
    {
      //Arrange
      ITagRepository tagRepository = new TagRepositoryJson();

      Tag tag = new Tag.TagBuilder()
          .SetName(name)
          .SetDescription(description)
          .SetColor(new Domain.ValueObjects.Color(color))
          .Build();
      tagRepository.AddTag(tag);
      //Act
      var tagFound = tagRepository.GetTagById(tag.Id);
      //Assert
      TagCompare(tag, tagFound);
    }

    [TestMethod]
    [DataRow("GetAllTags", "Description", "#000000")]
    public void GetAllTags(string name, string description, string color)
    {
      //Arrange
      ITagRepository tagRepository = new TagRepositoryJson();

      Tag tag = new Tag.TagBuilder()
          .SetName(name)
          .SetDescription(description)
          .SetColor(new Domain.ValueObjects.Color(color))
          .Build();
      tagRepository.AddTag(tag);
      //Act
      var tags = tagRepository.GetAllTags();
      //Assert  
      Assert.IsTrue(tagRepository.GetAllTags().Any());
    }

    public static void TagCompare(Tag tag, Tag tag2)
    {
      Assert.AreEqual(tag.Id, tag2.Id);
      Assert.AreEqual(tag.Description, tag2.Description);
      Assert.AreEqual(tag.Color, tag2.Color);
      Assert.AreEqual(tag.Name, tag2.Name);
      foreach (var parentTagId in tag.ParentTagIds)
      {
        Assert.IsTrue(tag2.ParentTagIds.Any(t => t == parentTagId));
      }
      foreach (var parentTagId in tag2.ParentTagIds)
      {
        Assert.IsTrue(tag.ParentTagIds.Any(t => t == parentTagId));
      }
    }
  }
}
