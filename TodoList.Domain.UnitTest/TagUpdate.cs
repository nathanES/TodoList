using TodoList.Domain.Entities;

namespace TodoList.Domain.UnitTest;

[TestClass]
public class TagUpdate
{
    [TestMethod]
    [DataRow("NewTagName")]
    public void UpdateTag_UpdateTagName_ShouldUpdateTagName(string newTagName)
    {
        string tagName = nameof(UpdateTag_UpdateTagName_ShouldUpdateTagName);

        Tag tag = new Tag.TagBuilder(tagName)
            .Build();

        tag.UpdateName(newTagName);

        Assert.AreEqual(newTagName, tag.Name);
    }

    [TestMethod]
    [DataRow("NewTagDescription")]
    public void UpdateTag_UpdateTagDescription_ShouldUpdateTagDescription(string newTagDescription)
    {
        string tagName = nameof(UpdateTag_UpdateTagDescription_ShouldUpdateTagDescription);

        Tag tag = new Tag.TagBuilder(tagName)
            .SetDescription("FirstTagDescription")
            .Build();

        tag.UpdateDescription(newTagDescription);

        Assert.AreEqual(newTagDescription, tag.Description);
    }

    [TestMethod]
    [DataRow("NewTagDescription")]
    public void UpdateTag_UpdateTagDescriptionWhenNoTagDescription_ShouldUpdateTagDescription(string newTagDescription)
    {
        string tagName = nameof(UpdateTag_UpdateTagDescriptionWhenNoTagDescription_ShouldUpdateTagDescription);

        Tag tag = new Tag.TagBuilder(tagName)
            .Build();

        tag.UpdateDescription(newTagDescription);

        Assert.AreEqual(newTagDescription, tag.Description);
    }

    [TestMethod]
    [DataRow("#000000")]
    public void UpdateTag_UpdateTagColor_ShouldUpdateTagColor(string newTagColor)
    {
        string tagName = nameof(UpdateTag_UpdateTagColor_ShouldUpdateTagColor);
        ValueObjects.Color color = new(newTagColor);

        Tag tag = new Tag.TagBuilder(tagName)
            .SetColor(new ValueObjects.Color("#070809"))
            .Build();

        tag.UpdateColor(color);

        Assert.AreEqual(color, tag.Color);
    }
    [TestMethod]
    [DataRow("#099880")]
    public void UpdateTag_UpdateTagColorWhenNoTagColor_ShouldUpdateTagColor(string newTagColor)
    {
        string tagName = nameof(UpdateTag_UpdateTagColorWhenNoTagColor_ShouldUpdateTagColor);
        ValueObjects.Color color = new(newTagColor);

        Tag tag = new Tag.TagBuilder(tagName)
            .Build();

        tag.UpdateColor(color);

        Assert.AreEqual(color, tag.Color);
    }

    [TestMethod]
    public void UpdateTag_UpdateTagParent_ShouldUpdateTagParent()
    {
        string tagName = nameof(UpdateTag_UpdateTagParent_ShouldUpdateTagParent);
        Guid newTagParentId = Guid.NewGuid();

        Tag tag = new Tag.TagBuilder(tagName)
            .SetParentTagIds(new List<Guid>() { Guid.NewGuid() })
            .Build();
        tag.UpdateParentTagIds(new List<Guid>() { newTagParentId });

        Assert.IsTrue(tag.ParentTagIds.Contains(newTagParentId));
        Assert.IsTrue(tag.ParentTagIds.Count() == 1);
    }
    [TestMethod]
    public void UpdateTag_UpdateTagParentWhenNoTagParent_ShouldUpdateTagParent()
    {
        string tagName = nameof(UpdateTag_UpdateTagParentWhenNoTagParent_ShouldUpdateTagParent);
        Guid newTagParentId = Guid.NewGuid();

        Tag tag = new Tag.TagBuilder(tagName)
            .Build();

        tag.UpdateParentTagIds(new List<Guid>() { newTagParentId });

        Assert.IsTrue(tag.ParentTagIds.Contains(newTagParentId));
        Assert.IsTrue(tag.ParentTagIds.Count() == 1);
    }

    [TestMethod]
    [DataRow("newTagName", "newTagDescription", "#099880")]
    public void UpdateTag_UpdateAllProperties_ShouldUpdateAllProperties(string newTagName, string newTagDescription, string newTagColor)
    {
        string tagName = nameof(UpdateTag_UpdateAllProperties_ShouldUpdateAllProperties);
        ValueObjects.Color newTagColorValue = new(newTagColor);
        Guid newTagParentId = Guid.NewGuid();

        Tag tag = new Tag.TagBuilder(tagName)
        .SetDescription("FirstTagDescription")
        .SetColor(new ValueObjects.Color("#012345"))
        .SetParentTagIds(new List<Guid>() { Guid.NewGuid() })
        .Build();

        tag.UpdateName(newTagName);
        tag.UpdateDescription(newTagDescription);
        tag.UpdateColor(newTagColorValue);
        tag.UpdateParentTagIds(new List<Guid>() { newTagParentId });

        Assert.AreEqual(newTagName, tag.Name);
        Assert.AreEqual(newTagDescription, tag.Description);
        Assert.AreEqual(newTagColorValue, tag.Color);
        Assert.IsTrue(tag.ParentTagIds.Count() == 1);
        Assert.IsTrue(tag.ParentTagIds.Contains(newTagParentId));
    }
    [TestMethod]
    [DataRow("newTagName", "newTagDescription", "#099880")]
    public void UpdateTag_UpdateAllPropertiesWhenNoProperties_ShouldUpdateAllProperties(string newTagName, string newTagDescription, string newTagColor)
    {
        string tagName = nameof(UpdateTag_UpdateAllPropertiesWhenNoProperties_ShouldUpdateAllProperties);
        ValueObjects.Color newTagColorValue = new(newTagColor);
        Guid newTagParentId = Guid.NewGuid();

        Tag tag = new Tag.TagBuilder(tagName)
        .Build();

        tag.UpdateName(newTagName);
        tag.UpdateDescription(newTagDescription);
        tag.UpdateColor(newTagColorValue);
        tag.UpdateParentTagIds(new List<Guid>() { newTagParentId });

        Assert.AreEqual(newTagName, tag.Name);
        Assert.AreEqual(newTagDescription, tag.Description);
        Assert.AreEqual(newTagColorValue, tag.Color);
        Assert.IsTrue(tag.ParentTagIds.Count() == 1);
        Assert.IsTrue(tag.ParentTagIds.Contains(newTagParentId));
    }

    [TestMethod]
    public void UpdateTag_AddTagParent_ShouldAddTagParent()
    {
        string tagName = nameof(UpdateTag_AddTagParent_ShouldAddTagParent);
        Guid newTagParentId = Guid.NewGuid();

        Tag tag = new Tag.TagBuilder(tagName)
            .SetParentTagIds(new List<Guid>() { Guid.NewGuid() })
            .Build();

        tag.AddTagParent(newTagParentId);

        Assert.IsTrue(tag.ParentTagIds.Count() == 2);
        Assert.IsTrue(tag.ParentTagIds.Contains(newTagParentId));
    }
    [TestMethod]
    public void UpdateTag_AddTagParentWhenNoTagParent_ShouldAddTagParent()
    {
        string tagName = nameof(UpdateTag_AddTagParentWhenNoTagParent_ShouldAddTagParent);
        Guid newTagParentId = Guid.NewGuid();

        Tag tag = new Tag.TagBuilder(tagName)
            .Build();

        tag.AddTagParent(newTagParentId);

        Assert.IsTrue(tag.ParentTagIds.Count() == 1);
        Assert.IsTrue(tag.ParentTagIds.Contains(newTagParentId));
    }
    [TestMethod]
    public void UpdateTag_AddTagParent_ShouldAddOneTagParent()
    {
        string tagName = nameof(UpdateTag_AddTagParent_ShouldAddOneTagParent);
        Guid newTagParentId = Guid.NewGuid();

        Tag tag = new Tag.TagBuilder(tagName)
            .Build();

        tag.AddTagParent(newTagParentId);
        tag.AddTagParent(newTagParentId);

        Assert.IsTrue(tag.ParentTagIds.Contains(newTagParentId));
        Assert.IsTrue(tag.ParentTagIds.Where(t => t == newTagParentId).Count() == 1);
    }
    [TestMethod]
    public void UpdateTag_RemoveTagParent_ShouldRemoveTagParent()
    {
        string tagName = nameof(UpdateTag_RemoveTagParent_ShouldRemoveTagParent);
        Guid tagParentIDToRemove = Guid.NewGuid();

        Tag tag = new Tag.TagBuilder(tagName)
            .SetParentTagIds(new List<Guid>() { Guid.NewGuid(), tagParentIDToRemove })
            .Build();

        bool result = tag.RemoveTagParent(tagParentIDToRemove);

        Assert.IsTrue(result);
        Assert.IsFalse(tag.ParentTagIds.Contains(tagParentIDToRemove));
        Assert.IsTrue(tag.ParentTagIds.Count() == 1);
    }

    [TestMethod]
    public void UpdateTag_RemoveTagParent_ShouldRemoveOneTagParent()
    {
        string tagName = nameof(UpdateTag_RemoveTagParent_ShouldRemoveOneTagParent);
        Guid tagParentIDToRemove = Guid.NewGuid();

        Tag tag = new Tag.TagBuilder(tagName)
        .SetParentTagIds(new List<Guid>() { Guid.NewGuid(), tagParentIDToRemove })
            .Build();

        bool result = tag.RemoveTagParent(tagParentIDToRemove);
        bool result2 = tag.RemoveTagParent(tagParentIDToRemove);

        Assert.IsTrue(result);
        Assert.IsFalse(result2);
        Assert.IsFalse(tag.ParentTagIds.Contains(tagParentIDToRemove));
        Assert.IsTrue(tag.ParentTagIds.Count() == 1);

    }
}
