using TodoList.Domain.Entities;
using TodoList.Domain.ValueObjects;

namespace TodoList.Domain.Interfaces.Repositories;

public interface ITagRepository
{
    IEnumerable<Tag> GetAllTags();
    Tag GetTagById(Guid id);
    bool AddTag(Tag tag);
    bool UpdateTag(Tag tag);
    bool UpdateTagName(Guid tagId, string newName);
    bool UpdateTagDescription(Guid tagId, string newDescription);
    bool UpdateTagColor(Guid tagId, Color newColor);
    bool UpdateTagParent(Guid tagId, List<Guid> newParentTagIds);
    bool AddTagParent(Guid tagId, Guid parentTagId);
    bool RemoveTagParent(Guid tagId, Guid parentTagId);
    bool DeleteTagById(Guid tagId);
    bool DeleteTagByIds(IEnumerable<Guid> tagIds);
}
