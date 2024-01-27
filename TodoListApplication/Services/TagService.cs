using TodoList.Application.DTOs;
using TodoList.Domain.Entities;
using TodoList.Domain.Interfaces.Logger;
using TodoList.Domain.Interfaces.Repositories;

namespace TodoList.Application.Services;
public class TagService
{
    private readonly ITagRepository tagRepository;
    private readonly ILogger logger;

    public TagService(ITagRepository tagRepository, ILogger logger)
    {
        this.tagRepository = tagRepository;
        this.logger = logger;
    }

    public IEnumerable<TagDto> GetAllTags()
    {
        return tagRepository.GetAllTags().Select(t => (TagDto)t);
    }

    public TagDto GetTagById(string tagId)
    {
        return (TagDto)tagRepository.GetTagById(tagId);
    }

    public void AddTag(TagDto tagDto)
    {
        Tag tag = (Tag)tagDto;
        tagRepository.AddTag(tag);
    }
    public void UpdateTag(TagDto tagDto)
    {
        Tag tag = (Tag)tagDto;
        tagRepository.UpdateTag(tag);
    }

    // Peut être mettre le tasktagService à la place de taskTagRepository, si cela se complexifie.
    public void DeleteTagById(string tagId, ITaskTagRepository taskTagRepository)
    {
        UnassignAllTaskFromTag(tagId, taskTagRepository);
        tagRepository.DeleteTagById(tagId);
    }
    public void DeleteTagByIds(IEnumerable<string> tagIds, ITaskTagRepository taskTagRepository)
    {
        foreach (string tagId in tagIds)
        {
            UnassignAllTaskFromTag(tagId, taskTagRepository);
        }
        tagRepository.DeleteTagByIds(tagIds);
    }
    private void UnassignAllTaskFromTag(string tagId, ITaskTagRepository taskTagRepository)
    {
        IEnumerable<TaskTag> taskTags = taskTagRepository.GetTaskTagsByTagId(tagId);

        if (!taskTags.Any())
            return; //The tag does not have task

        taskTagRepository.DeleteTaskTagByIds(taskTags.Select(tt => tt.Id));
    }
    public void UnassignAllTaskFromTags(IEnumerable<string> tagIds, ITaskTagRepository taskTagRepository)
    {
        IEnumerable<TaskTag> taskTags = taskTagRepository.GetTaskTagsByTagIds(tagIds);
        if (taskTags == null)
            return; //The tag does not have task

        taskTagRepository.DeleteTaskTagByIds(taskTags.Select(t => t.Id));
    }
}
