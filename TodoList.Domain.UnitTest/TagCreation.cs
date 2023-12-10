using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoList.Domain.Entities;
using TodoList.Domain.ValueObjects;

namespace TodoList.Domain.UnitTest
{
  
  [TestClass]
  public class TagCreation
  {
    [TestMethod]
    [DataRow("Tag 1")]
    public void Constructor_ShouldCreateTag_WithGivenName(string name)
    {
      Tag tag = new Tag.TagBuilder()
          .SetName(name)
          .Build();

      Assert.AreEqual(tag.Name, name);
      Assert.IsNotNull(tag.Id);
      Assert.IsNull(tag.Description);
      Assert.AreEqual(tag.Color, new Color("#000000"));
      Assert.IsNull(tag.TagParent);
    }

    [TestMethod]
    [DataRow("Tag 1", "Description 1")]
    public void Constructor_ShouldCreateTag_WithDescription(string name, string description)
    {
      Tag tag = new Tag.TagBuilder()
          .SetName(name)
          .SetDescription(description)
          .Build();

      Assert.AreEqual(tag.Name, name);
      Assert.IsNotNull(tag.Id);
      Assert.AreEqual(tag.Description, description);
      Assert.AreEqual(tag.Color, new Color("#000000"));
      Assert.IsNull(tag.TagParent);
    }
    [TestMethod]
    [DataRow("Tag 1", "#000000")]
    public void Constructor_ShouldCreateTag_WithColor(string name, string color)
    {
    Color color1 = new Color(color);
      Tag tag = new Tag.TagBuilder()
          .SetName(name)
          .SetColor(color1)
          .Build();

      Assert.AreEqual(tag.Name, name);
      Assert.IsNotNull(tag.Id);
      Assert.IsNull(tag.Description);
      Assert.AreEqual(tag.Color, color1);
      Assert.IsNull(tag.TagParent);
    }
    [TestMethod]
    [DataRow("Tag 1", "Tag 2")]
    public void Constructor_ShouldCreateTag_WithParent(string name, string parentName)
    {
      Tag tagParent = new Tag.TagBuilder()
          .SetName(parentName)
          .Build();
      Tag tag = new Tag.TagBuilder()
          .SetName(name)
          .SetTagParent(tagParent)
          .Build();

      Assert.AreEqual(tag.Name, name);
      Assert.IsNotNull(tag.Id);
      Assert.IsNull(tag.Description);
      Assert.AreEqual(tag.Color, new Color("#000000"));
      Assert.AreEqual(tag.TagParent, tagParent);
    }
    [TestMethod]
    [DataRow("Tag 1", "Tag 2", "Description 1", "#000000")]
    public void Constructor_ShouldCreateTag_WithAllProperties(string name, string parentName, string description, string color)
    {
    Color color1 = new Color(color);
      Tag tagParent = new Tag.TagBuilder()
          .SetName(parentName)
          .Build();
      Tag tag = new Tag.TagBuilder()
          .SetName(name)
          .SetDescription(description)
          .SetColor(color1)
          .SetTagParent(tagParent)
          .Build();

      Assert.AreEqual(tag.Name, name);
      Assert.IsNotNull(tag.Id);
      Assert.AreEqual(tag.Description, description);
      Assert.AreEqual(tag.Color, color1);
      Assert.AreEqual(tag.TagParent, tagParent);
    }
  }
}
