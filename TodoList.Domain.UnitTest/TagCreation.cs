using TodoList.Domain.Entities;
using TodoList.Domain.ValueObjects;

namespace TodoList.Domain.UnitTest;

[TestClass]
public class TagCreation
{
    [TestMethod]
    [DataRow("TagNameWithGivenName")]
    public void ConstructorTag_WithGivenName_ShouldCreateTag(string tagName)
    {
        Tag tag = new Tag.TagBuilder(tagName)
            .Build();

        Assert.IsNotNull(tag.Id);
        Assert.AreEqual(tagName, tag.Name);
        Assert.IsNull(tag.Description);
        Assert.AreEqual(Color.Default, tag.Color);
        Assert.IsFalse(tag.ParentTagIds.Any());

    }
    [TestMethod]
    [DataRow("Description 1")]
    public void ConstructorTag_WithGivenDescription_ShouldCreateTag(string tagDescription)
    {
        string tagName = nameof(ConstructorTag_WithGivenDescription_ShouldCreateTag);

        Tag tag = new Tag.TagBuilder(tagName)
            .SetDescription(tagDescription)
            .Build();

        Assert.IsNotNull(tag.Id);
        Assert.AreEqual(tagName, tag.Name);
        Assert.AreEqual(tagDescription, tag.Description);
        Assert.AreEqual(Color.Default, tag.Color);
        Assert.IsFalse(tag.ParentTagIds.Any());
    }
    [TestMethod]
    [DataRow("#000000")]
    public void ConstructorTag_WithGivenColor_ShouldCreateTag(string tagColor)
    {
        string tagName = nameof(ConstructorTag_WithGivenColor_ShouldCreateTag);
        Color color = new(tagColor);

        Tag tag = new Tag.TagBuilder(tagName)
            .SetColor(color)
            .Build();

        Assert.IsNotNull(tag.Id);
        Assert.AreEqual(tagName, tag.Name);
        Assert.IsNull(tag.Description);
        Assert.AreEqual(color, tag.Color);
        Assert.IsFalse(tag.ParentTagIds.Any());
    }
    [TestMethod]
    public void ConstructorTag_WithGivenParentTag_ShouldCreateTag()
    {
        string tagName = nameof(ConstructorTag_WithGivenParentTag_ShouldCreateTag);
        Guid parentTagID = Guid.NewGuid();

        Tag tag = new Tag.TagBuilder(tagName)
            .SetParentTagIds(new List<Guid>() { parentTagID })
            .Build();

        Assert.IsNotNull(tag.Id);
        Assert.AreEqual(tagName, tag.Name);
        Assert.IsNull(tag.Description);
        Assert.AreEqual(Color.Default, tag.Color);
        Assert.IsTrue(tag.ParentTagIds.Contains(parentTagID));
    }
    [TestMethod]
    [DataRow("TagNameWithGivenAllProperties", "Description 1", "#000000")]
    public void ConstructorTag_WithGivenAllProperties_ShouldCreateTag(string tagName, string tagDescription, string tagColor)
    {
        Color color1 = new(tagColor);
        Guid parentTagID = Guid.NewGuid();

        Tag tag = new Tag.TagBuilder(tagName)
            .SetDescription(tagDescription)
            .SetColor(color1)
            .SetParentTagIds(new List<Guid>() { parentTagID })
            .Build();

        Assert.IsNotNull(tag.Id);
        Assert.AreEqual(tagName, tag.Name);
        Assert.AreEqual(tagDescription, tag.Description);
        Assert.AreEqual(color1, tag.Color);
        Assert.IsTrue(tag.ParentTagIds.Contains(parentTagID));

    }
    [TestMethod]
    public void ConstructorTag_WithOneGivenParentTag_ShouldCreateTag()
    {
        string tagName = nameof(ConstructorTag_WithOneGivenParentTag_ShouldCreateTag);
        Guid parentTagID = Guid.NewGuid();

        Tag tag = new Tag.TagBuilder(tagName)
            .SetParentTagIds(new List<Guid>() { parentTagID, parentTagID })
            .Build();

        Assert.IsNotNull(tag.Id);
        Assert.AreEqual(tagName, tag.Name);
        Assert.IsNull(tag.Description);
        Assert.AreEqual(Color.Default, tag.Color);
        Assert.IsTrue(tag.ParentTagIds.Contains(parentTagID));
        Assert.IsTrue(tag.ParentTagIds.Where(x => x == parentTagID).Count() == 1);

    }
    [TestMethod]
    public void ConstructorTag_WithNullName_ShouldNotCreateTag_ArgumentNullException()
    {
        _ = Assert.ThrowsException<ArgumentNullException>(() => new Tag.TagBuilder(null)
                          .Build());
    }
    [TestMethod]
    public void ConstructorTag_WithEmptyName_ShouldNotCreateTag_ArgumentNullException()
    {
        _ = Assert.ThrowsException<ArgumentException>(() => new Tag.TagBuilder(string.Empty)
                          .Build());
    }
}
