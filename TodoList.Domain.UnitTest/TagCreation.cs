using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoList.Domain.Entities;

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
      Assert.AreEqual(tag.Color, "#000000");
      Assert.IsNull(tag.TagParent);
      Assert.IsTrue(tag.AssociatedTask.Count == 0);
    }
  }
}
