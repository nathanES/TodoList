using TodoList.Domain.Entities;
using TodoList.Domain.ValueObjects;

namespace TodoList.Application.DTOs;

public class TagDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public Color Color { get; set; }
    public List<Guid> ParentTagIds { get; set; }

    public TagDto()
    {
        ParentTagIds = new List<Guid>();
    }

    public static explicit operator Tag(TagDto tagDto)
    {
        return new Tag.TagBuilder(tagDto.Name)
            .SetId(tagDto.Id)
            .SetDescription(tagDto.Description)
            .SetColor(tagDto.Color)
            .SetParentTagIds(tagDto.ParentTagIds)
            .Build();
    }

    public static explicit operator TagDto(Tag tag)
    {
        return new TagDto()
        {
            Id = tag.Id,
            Name = tag.Name,
            Description = tag.Description,
            Color = tag.Color,
            ParentTagIds = tag.ParentTagIds
        };
    }
}
