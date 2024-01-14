using Newtonsoft.Json;
using TodoList.Domain.ValueObjects;

namespace TodoList.Domain.Entities;

public class Tag
{

    private string id;
    [JsonProperty("Id")]
    public string Id
    {
        get
        {
            return id;
        }

        set
        {
            if (!Guid.TryParse(value, out Guid _))
                throw new ArgumentException($"{nameof(Id)} must be a valid Guid");
            id = value;
        }
    }

    private string name;
    [JsonProperty("Name")]
    public string Name
    {
        get
        {
            return name;
        }

        private set
        {
            ArgumentException.ThrowIfNullOrEmpty(argument: value, nameof(name));
            name = value;
        }
    }
    [JsonProperty("Description")]
    public string? Description { get; private set; }
    [JsonProperty("Color")]
    public Color Color { get; private set; } = new Color("#000000");

    private List<string> parentTagIds = new();
    [JsonProperty("ParentTagIds")]
    public List<string> ParentTagIds
    {
        get
        {
            return parentTagIds;
        }

        set
        {
            foreach (string parentTagId in value)
            {
                if (!Guid.TryParse(parentTagId, out Guid _))
                    throw new ArgumentException($"{nameof(parentTagId)} must be a valid Guid");
            }
            parentTagIds = value;
        }
    }
    //TODO faire la même chose pour Task et modifier surement le constructeur pour faire aussi pareil que Tag
    public static Tag Empty = new("00000000-0000-0000-0000-000000000000", "___Empty")
    {
        Description = string.Empty,
        Color = new Color("#000000"),
        ParentTagIds = new List<string>()
    };

    private Tag(string id, string name)
    {
        Id = id;
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
        if (!Guid.TryParse(parentTagId, out _))
            throw new ArgumentException($"{nameof(parentTagId)} must be a valid Guid");

        ParentTagIds.Add(parentTagId);
    }

    public class TagBuilder
    {
        private readonly string id;
        private readonly string name;
        private string description;
        private Color color = new("#000000");
        private List<string> parentTagIds = new();
        public TagBuilder(string id, string name)
        {
            this.id = id;
            this.name = name;
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
            return new Tag(id, name)
            {
                Description = description,
                Color = color,
                ParentTagIds = parentTagIds
            };
        }
    }
}
