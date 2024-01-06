using TodoList.Domain.Entities;
using TodoList.Domain.ValueObjects;

namespace TodoList.Application.DTOs;

public class TagDto
{
  public string Id { get; set; }
  public string Name { get; set; }
  public string Description { get; set; }
  public string Color { get; set; }
  public List<string> ParentTagIds { get; set; }

  public TagDto() => ParentTagIds = new List<string>();
  //TODO : peut-être faire une bibliotheque de conversion au lieu de faire des explicit operator
  public static explicit operator Tag(TagDto tagDto)
  {
    return new Tag.TagBuilder()
        .SetId(tagDto.Id)
        .SetName(tagDto.Name)
        .SetDescription(tagDto.Description)
        .SetColor(new Color(tagDto.Color))
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
      Color = tag.Color?.ColorValue,
      ParentTagIds = tag.ParentTagIds
    };
  }
}
