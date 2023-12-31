﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoList.Domain.Entities;
using TodoList.Domain.ValueObjects;

namespace TodoList.Domain.UnitTest
{
  [TestClass]
  public class TagUpdate
  {
    [TestMethod]
    [DataRow("Tag 1")]
    public void UpdateName_ShouldUpdateTagName(string name)
    {
      Tag tag = new Tag.TagBuilder()
          .SetName(name)
          .Build();

      string newName = "New Name";
      tag.UpdateName(newName);

      Assert.AreEqual(tag.Name, newName);
    }
    [TestMethod]
    [DataRow("Description 1")]
    public void UpdateDescription_ShouldUpdateTagDescription(string description)
    {
      Tag tag = new Tag.TagBuilder()
          .SetName("Tag 1")
          .Build();

      tag.UpdateDescription(description);

      Assert.AreEqual(tag.Description, description);
    }
    [TestMethod]
    [DataRow("#000000")]
    public void UpdateColor_ShouldUpdateTagColor(string color)
    {
      Color color1 = new Color(color);
      Tag tag = new Tag.TagBuilder()
          .SetName("Tag 1")
          .Build();

      tag.UpdateColor(color1);

      Assert.AreEqual(color1, tag.Color);
    }
    [TestMethod]
    [DataRow("Tag Parent")]
    public void UpdateTagParent_ShouldUpdateTagParent(string parentName)
    {
      Tag tagParent = new Tag.TagBuilder()
      .SetName(parentName)
      .Build();

      Tag tag = new Tag.TagBuilder()
          .SetName("Tag 1")
          .Build();

      tag.UpdateTagParent(new List<string>() { tagParent.Id});

      Assert.IsTrue(tag.ParentTagIds.Contains(tagParent.Id ));
    }
    [TestMethod]
    [DataRow("Tag Parent", "#000000", "Description 1", "New name")]
    public void UpdateAllProperties_ShouldUpdateAllProperties(string parentName, string color, string description, string newname)
    {
      Tag tagParent = new Tag.TagBuilder()
      .SetName(parentName)
      .Build();
      Color color1 = new Color(color);

      Tag tag = new Tag.TagBuilder()
          .SetName("Tag 1")
          .Build();

      tag.UpdateTagParent(new List<string>() { tagParent.Id });
      tag.UpdateColor(color1);
      tag.UpdateDescription(description);
      tag.UpdateName(newname);

      Assert.IsTrue(tag.ParentTagIds.Contains(tagParent.Id));
      Assert.AreEqual(color1, tag.Color);
      Assert.AreEqual(tag.Description, description);
      Assert.AreEqual(newname, tag.Name);
    }
  }
}
