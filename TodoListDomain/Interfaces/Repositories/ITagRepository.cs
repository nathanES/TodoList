using TodoList.Domain.Entities;

namespace TodoList.Domain.Interfaces.Repositories;

public interface ITagRepository
{
    IEnumerable<Tag> GetAllTags();
    Tag GetTagById(string id);
    bool AddTag(Tag tag);
    bool UpdateTag(Tag tag);
    bool DeleteTagById(string tagId);
    bool DeleteTagByIds(IEnumerable<string> tagIds);
}
