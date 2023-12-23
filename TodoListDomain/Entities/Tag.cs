using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoList.Domain.Enum;
using TodoList.Domain.ValueObjects;

namespace TodoList.Domain.Entities
{
  public class Tag
  {

    private string id = Guid.NewGuid().ToString();
    [JsonProperty("Id")]
    public string Id
    {
      get { return id; }
      set 
      { 
        if(!Guid.TryParse(value, out Guid _))
          throw new ArgumentException("Id must be a valid Guid");
        id = value;
      }
    }

    private string name;
    [JsonProperty("Name")]
    public string Name
    {
      get { return name; }
      private set 
      {
        ArgumentException.ThrowIfNullOrEmpty(value, nameof(name));
        name = value; 
      }
    }
    [JsonProperty("Description")]
    public string? Description { get; private set; }
    [JsonProperty("Color")]
    public Color Color { get; private set; } = new Color("#000000");
    [JsonProperty("ParentTagIds")]
    public List<string> ParentTagIds { get; private set; } = new List<string>();

    private Tag(string name)
    {
      Name = name;
    }

    [JsonConstructor]
    private Tag(string Id, string Name, string Description, Color Color, List<string> ParentTagIds)
    {
      this.Id = Id;
      this.Name = Name;
      this.Description = Description;
      this.Color = Color;
      this.ParentTagIds = ParentTagIds;
    }


    public void UpdateName(string name)
    {
      Name = name;
    }
    public void UpdateDescription(string description)
    {
      Description = description;
    }
    public void UpdateColor(Color color)
    {
      Color = color;
    }
    public void UpdateTagParent(List<string> parentTagIds)
    {
      ParentTagIds = parentTagIds;
    }
    public void AddTagParent(string parentTagId)
    {
      ParentTagIds.Add(parentTagId);
    }

    public class TagBuilder
    {
      private string id = Guid.NewGuid().ToString();
      private string name;
      private string description;
      private Color color = new Color("#000000");
      private List<string> parentTagIds = new List<string>();
      public TagBuilder SetId(string id)
      {
        this.id = id;
        return this;
      }
      public TagBuilder SetName(string name)
      {
        this.name = name;
        return this;
      }

      public TagBuilder SetDescription(string description)
      {
        this.description = description;
        return this;
      }

      public TagBuilder SetColor(Color color)
      {
        this.color = color;
        return this;
      }

      public TagBuilder SetParentTagIds(List<string> parentTagIds)
      {
        this.parentTagIds = parentTagIds;
        return this;
      }

      public Tag Build()
      {
        return new Tag(name)
        {
          Id = id,
          Description = description,
          Color = color,
          ParentTagIds = parentTagIds
        };
      }
    }
  }
}
