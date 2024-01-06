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

      Assert.AreEqual(name, tag.Name);
      Assert.IsNotNull(tag.Id);
      Assert.IsNull(tag.Description);
      Assert.AreEqual(tag.Color, new Color("#000000"));
      Assert.IsFalse(tag.ParentTagIds.Any());

    }
    [TestMethod]
    [DataRow("Tag 1", "Description 1", "#000000")]
    public void Constructor_ShouldCreateTag_WithDescription(string name, string description, string color)
    {
      Tag tag = new Tag.TagBuilder()
          .SetName(name)
          .SetDescription(description)
          .Build();

      Assert.AreEqual(name, tag.Name);
      Assert.IsNotNull(tag.Id);
      Assert.AreEqual(description, tag.Description);
      Assert.AreEqual(tag.Color, new Color(color));
      Assert.IsFalse(tag.ParentTagIds.Any());
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

      Assert.AreEqual(name, tag.Name);
      Assert.IsNotNull(tag.Id);
      Assert.IsNull(tag.Description);
      Assert.AreEqual(color1, tag.Color);
      Assert.IsFalse(tag.ParentTagIds.Any());
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
          .SetParentTagIds(new List<string>() { tagParent.Id })
          .Build();

      Assert.AreEqual(name, tag.Name);
      Assert.IsNotNull(tag.Id);
      Assert.IsNull(tag.Description);
      Assert.AreEqual(new Color("#000000"), tag.Color);
      Assert.IsTrue(tag.ParentTagIds.Contains(tagParent.Id));
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
          .SetParentTagIds(new List<string>() { tagParent.Id})
          .Build();

      Assert.AreEqual(name, tag.Name);
      Assert.IsNotNull(tag.Id);
      Assert.AreEqual(description, tag.Description);
      Assert.AreEqual(color1, tag.Color);
      Assert.IsTrue( tag.ParentTagIds.Contains(tagParent.Id));
    }
    [TestMethod]
    public void Constructor_ShouldNotCreateTag_WithNullName()
    {
      Assert.ThrowsException<ArgumentNullException>(() => new Tag.TagBuilder()
               .SetName(null)
                        .Build());
    }
    [TestMethod]
    public void Constructor_ShouldNotCreateTag_WithNulNameButOtherProperties()
    {
      Assert.ThrowsException<ArgumentNullException>(() => new Tag.TagBuilder()
               .SetName(null)
               .SetDescription("Description 1")
               .SetColor(new Color("#000000"))
              .Build());
    }
  }
}
