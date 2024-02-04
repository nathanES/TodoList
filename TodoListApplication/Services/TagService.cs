using TodoList.Application.DTOs;
using TodoList.Domain.Entities;
using TodoList.Domain.Interfaces.Logger;
using TodoList.Domain.Interfaces.Repositories;

namespace TodoList.Application.Services;
public class TagService
{
    private readonly ITagRepository _tagRepository;
    private readonly ILogger _logger;

    public TagService(ITagRepository tagRepository, ILogger logger)
    {
        _tagRepository = tagRepository;
        _logger = logger;
    }

    public IEnumerable<TagDto> GetAllTags()
    {
        return _tagRepository.GetAllTags().Select(t => (TagDto)t);
    }

    public TagDto GetTagById(Guid tagId)
    {
        return (TagDto)_tagRepository.GetTagById(tagId);
    }

    public void AddTag(TagDto tagDto)
    {
        Tag tag = (Tag)tagDto;
        _ = _tagRepository.AddTag(tag);
    }
    public void UpdateTag(TagDto tagDto)
    {
        Tag tag = (Tag)tagDto;
        _ = _tagRepository.UpdateTag(tag);
    }

    // Peut être mettre le tasktagService à la place de taskTagRepository, si cela se complexifie.
    public void DeleteTagById(Guid tagId, ITaskTagRepository taskTagRepository)
    {
        UnassignAllTaskFromTag(tagId, taskTagRepository);
        _ = _tagRepository.DeleteTagById(tagId);
    }
    public void DeleteTagByIds(IEnumerable<Guid> tagIds, ITaskTagRepository taskTagRepository)
    {
        foreach (Guid tagId in tagIds)
        {
            UnassignAllTaskFromTag(tagId, taskTagRepository);
        }
        _ = _tagRepository.DeleteTagByIds(tagIds);
    }
    private void UnassignAllTaskFromTag(Guid tagId, ITaskTagRepository taskTagRepository)
    {
        IEnumerable<TaskTag> taskTags = taskTagRepository.GetTaskTagsByTagId(tagId);

        if (!taskTags.Any())
            return; //The tag does not have task

        _ = taskTagRepository.DeleteTaskTagByIds(taskTags.Select(tt => tt.Id));
    }
    public void UnassignAllTaskFromTags(IEnumerable<Guid> tagIds, ITaskTagRepository taskTagRepository)
    {
        IEnumerable<TaskTag> taskTags = taskTagRepository.GetTaskTagsByTagIds(tagIds);
        if (taskTags == null)
            return; //The tag does not have task

        _ = taskTagRepository.DeleteTaskTagByIds(taskTags.Select(t => t.Id));
    }
}
