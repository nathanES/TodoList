using TodoList.Domain.Entities;
using TodoList.Domain.ValueObjects;

namespace TodoList.Application.DTOs;

public class TagDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public Color Color { get; set; }
    public HashSet<Guid> ParentTagIds { get; set; } = new();

    public TagDto()
    {
        ParentTagIds = new();
    }

    public static explicit operator Tag(TagDto tagDto)
    {
        if (tagDto == null)
            throw new ArgumentNullException(nameof(tagDto), $"L'objet {nameof(TagDto)} ne doit pas être null.");

        Tag.TagBuilder builder = new(tagDto.Name);
        builder = builder.SetDescription(tagDto.Description);
        if (tagDto.Id != Guid.Empty)
            builder = builder.SetId(tagDto.Id);
        if (tagDto.Color != null)
            builder = builder.SetColor(tagDto.Color);
        if (tagDto.ParentTagIds != null)
            builder = builder.SetParentTagIds(tagDto.ParentTagIds);
        return builder.Build();
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
