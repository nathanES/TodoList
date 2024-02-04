using TodoList.Domain.Entities;

namespace TodoList.Domain.Interfaces.Repositories;

public interface ITagRepository
{
    IEnumerable<Tag> GetAllTags();
    Tag GetTagById(Guid id);
    bool AddTag(Tag tag);
    bool UpdateTag(Tag tag);
    bool DeleteTagById(Guid tagId);
    bool DeleteTagByIds(IEnumerable<Guid> tagIds);
}
