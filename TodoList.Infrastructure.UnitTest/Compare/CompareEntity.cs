using TodoList.Domain.Entities;

namespace TodoList.Infrastructure.UnitTest.Compare;
internal static class CompareEntity
{
    internal static void TagCompare(Tag tag, Tag tag2)
    {
        Assert.AreEqual(tag.Id, tag2.Id);
        Assert.AreEqual(tag.Description, tag2.Description);
        Assert.AreEqual(tag.Color, tag2.Color);
        Assert.AreEqual(tag.Name, tag2.Name);
        foreach (Guid parentTagId in tag.ParentTagIds)
        {
            Assert.IsTrue(tag2.ParentTagIds.Any(t => t == parentTagId));
        }
        foreach (Guid parentTagId in tag2.ParentTagIds)
        {
            Assert.IsTrue(tag.ParentTagIds.Any(t => t == parentTagId));
        }
    }
}
