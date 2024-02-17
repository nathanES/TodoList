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

    private string _name;
    [JsonProperty("Name")]
    public string Name
    {
        get
        {
            return _name;
        }

        private set
        {
            ArgumentNullException.ThrowIfNullOrEmpty(argument: value, nameof(_name));
            _name = value;
        }
    }
    [JsonProperty("Description")]
    public string? Description { get; private set; }

    [JsonIgnore]
    private Color _color = Color.Default;

    [JsonProperty("Color")]
    public Color Color
    {
        get { return _color; }
        set
        {
            ArgumentNullException.ThrowIfNull(value, nameof(Color));
            _color = value;
        }
    }

    [JsonProperty("ParentTagIds")]
    public HashSet<Guid> ParentTagIds { get; private set; } = new(); //HashSet to avoid duplicate
    public static Tag Default = new(Guid.Parse("00000000-0000-0000-0000-000000000000"), "___Default")
    {
        Description = string.Empty,
        Color = Color.Default,
        ParentTagIds = new HashSet<Guid>()
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
        ParentTagIds = new HashSet<Guid>(parentTagIds.Select(Guid.Parse));
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

    public void UpdateParentTagIds(IEnumerable<Guid> parentTagIds)
    {
        ParentTagIds = new HashSet<Guid>(parentTagIds);
    }

    public void AddTagParent(Guid parentTagId)
    {
        if (ParentTagIds.Contains(parentTagId))
            return;
        _ = ParentTagIds.Add(parentTagId);
    }
    public bool RemoveTagParent(Guid parentTagId)
    {
        if (!ParentTagIds.Contains(parentTagId))
            return false;
        return ParentTagIds.Remove(parentTagId);
    }
    public class TagBuilder
    {
        private Guid _id = Guid.Empty;
        private readonly string _name;
        private string _description;
        private Color _color = Color.Default;
        private HashSet<Guid> _parentTagIds = new();
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

        public TagBuilder SetParentTagIds(IEnumerable<Guid> parentTagIds)
        {
            _parentTagIds = new HashSet<Guid>(parentTagIds);
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
