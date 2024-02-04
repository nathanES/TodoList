using TodoList.Domain.Entities;
using TodoList.Domain.ValueObjects;

namespace TodoList.Domain.UnitTest;

[TestClass]
public class TagUpdate
{
    [TestMethod]
    [DataRow("Tag 1")]
    public void UpdateName_ShouldUpdateTagName(string name)
    {
        Tag tag = new Tag.TagBuilder(name)
            .Build();

        string newName = "New Name";
        tag.UpdateName(newName);

        Assert.AreEqual(tag.Name, newName);
    }
    [TestMethod]
    [DataRow("Description 1", "Tag 1")]
    public void UpdateDescription_ShouldUpdateTagDescription(string description, string name)
    {
        Tag tag = new Tag.TagBuilder(name)
            .Build();

        tag.UpdateDescription(description);

        Assert.AreEqual(tag.Description, description);
    }
    [TestMethod]
    [DataRow("#000000", "Tag 1")]
    public void UpdateColor_ShouldUpdateTagColor(string color, string name)
    {
        Color color1 = new(color);
        Tag tag = new Tag.TagBuilder(name)
            .Build();

        tag.UpdateColor(color1);

        Assert.AreEqual(color1, tag.Color);
    }
    [TestMethod]
    [DataRow("Tag Parent")]
    public void UpdateTagParent_ShouldUpdateTagParent(string parentName)
    {
        Tag tagParent = new Tag.TagBuilder(parentName)
        .Build();

        Tag tag = new Tag.TagBuilder("Tag 1")
            .Build();

        tag.UpdateTagParent(new List<Guid>() { tagParent.Id });

        Assert.IsTrue(tag.ParentTagIds.Contains(tagParent.Id));
    }
    [TestMethod]
    [DataRow("Tag Parent", "#000000", "Description 1", "New name")]
    public void UpdateAllProperties_ShouldUpdateAllProperties(string parentName, string color, string description, string newname)
    {
        Tag tagParent = new Tag.TagBuilder(parentName)
        .Build();
        Color color1 = new(color);

        Tag tag = new Tag.TagBuilder("Tag1")
            .Build();

        tag.UpdateTagParent(new List<Guid>() { tagParent.Id });
        tag.UpdateColor(color1);
        tag.UpdateDescription(description);
        tag.UpdateName(newname);

        Assert.IsTrue(tag.ParentTagIds.Contains(tagParent.Id));
        Assert.AreEqual(color1, tag.Color);
        Assert.AreEqual(tag.Description, description);
        Assert.AreEqual(newname, tag.Name);
    }
}
