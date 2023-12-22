using System.Globalization;
using System.Xml.Linq;
using TodoList.Domain.Entities;
using TodoList.Domain.Enum;
using TodoList.Domain.Helpers;
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
      Assert.AreEqual(tagFound.Id, tag.Id);
      Assert.IsTrue(ObjectHelper.AreObjectsEqual(tagFound, tag));
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
  }
}
