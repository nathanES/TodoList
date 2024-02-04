using Newtonsoft.Json;
using TodoList.Domain.ValueObjects;

namespace TodoList.Domain.Entities;

public class Tag
{

    [JsonIgnore]
    public Guid Id { get; private set; }
    [JsonProperty("Id")]
    public string IdString
    {
        get
        {
            return Id.ToString();
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
    public Color Color { get; private set; } = Color.Default;
    [JsonProperty("ParentTagIds")]
    public List<Guid> ParentTagIds { get; private set; } = new();
    public static Tag Default = new(Guid.Parse("00000000-0000-0000-0000-000000000000"), "___Default")
    {
        Description = string.Empty,
        Color = Color.Default,
        ParentTagIds = new List<Guid>()
    };

    private Tag(string name)
    {
        Id = Guid.NewGuid();
        Name = name;
    }
    private Tag(Guid id, string name)
    {
        Id = id;
        Name = name;
    }
    [JsonConstructor]
    private Tag(string id, string name, string description, Color color, List<string> parentTagIds)
    {
        if (!Guid.TryParse(id, out Guid idFormated))
            throw new ArgumentException("Id must be a valid Guid");
        Id = idFormated;
        Name = name;
        Description = description;
        Color = color;
        ParentTagIds = parentTagIds.Select(Guid.Parse).ToList();
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

    public void UpdateTagParent(List<Guid> parentTagIds)
    {
        ParentTagIds = parentTagIds;
    }

    public void AddTagParent(Guid parentTagId)
    {
        ParentTagIds.Add(parentTagId);
    }

    public class TagBuilder
    {
        private Guid _id = Guid.Empty;
        private readonly string _name;
        private string _description;
        private Color _color = Color.Default;
        private List<Guid> _parentTagIds = new();
        public TagBuilder(string name)
        {
            _name = name;
        }
        public TagBuilder SetId(Guid id)
        {
            _id = id;
            return this;
        }
        public TagBuilder SetDescription(string description)
        {
            _description = description;
            return this;
        }

        public TagBuilder SetColor(Color color)
        {
            _color = color;
            return this;
        }

        public TagBuilder SetParentTagIds(List<Guid> parentTagIds)
        {
            _parentTagIds = parentTagIds;
            return this;
        }

        public Tag Build()
        {
            if (_id == Guid.Empty)
            {
                return new Tag(_name)
                {
                    Description = _description,
                    Color = _color,
                    ParentTagIds = _parentTagIds
                };
            }
            return new Tag(_id, _name)
            {
                Description = _description,
                Color = _color,
                ParentTagIds = _parentTagIds
            };

        }
    }
}
