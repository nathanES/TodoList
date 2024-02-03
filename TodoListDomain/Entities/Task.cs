using Newtonsoft.Json;
using TodoList.Domain.Enum;

namespace TodoList.Domain.Entities;

public class Task
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
                throw new ArgumentException("Id must be a valid Guid");
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
            ArgumentException.ThrowIfNullOrEmpty(value, nameof(name));
            name = value;
        }
    }

    [JsonProperty("Description")]
    public string? Description { get; private set; }
    [JsonProperty("Priority")]
    public Priority Priority { get; private set; } = Priority.Medium;
    [JsonProperty("DeadLine")]
    public DateTime DeadLine { get; private set; } = DateTime.MaxValue;
    [JsonIgnore]
    public TimeSpan TimeLeftBeforeDeadLine
    {
        get
        {
            return DeadLine == DateTime.MaxValue ? TimeSpan.MaxValue : DeadLine - DateTime.UtcNow;
        }
    }

    [JsonProperty("CreationTime")]
    public DateTime CreationTime { get; private set; } = DateTime.UtcNow;
    [JsonProperty("IsCompleted")]
    public bool IsCompleted { get; private set; } = false;
    public static Task Default = new("00000000-0000-0000-0000-000000000000", "___Default")
    {
        Description = string.Empty,
        Priority = Priority.Medium,
        DeadLine = DateTime.MinValue,
        CreationTime = DateTime.MinValue,
        IsCompleted = false
    };
    private Task(string id, string name)
    {
        Id = id;
        Name = name;
    }
    [JsonConstructor]
    private Task(string id, string name, string description, Priority priority, DateTime deadLine, DateTime creationTime, bool isCompleted)
    {
        Id = id;
        Name = name;
        Description = description;
        Priority = priority;
        DeadLine = deadLine;
        CreationTime = creationTime;
        IsCompleted = isCompleted;
    }

    public void UpdateName(string name)
    {
        Name = name;
    }

    public void UpdateDescription(string description)
    {
        Description = description;
    }

    public void UpdatePriority(Priority priority)
    {
        Priority = priority;
    }

    public void UpdateDeadLine(DateTime deadLine)
    {
        //Le contrôle est fait ici pour éviter les erreurs lors de la récupération d'anciennes taches via le Json
        if (deadLine < DateTime.Now)
            throw new ArgumentException("DeadLine must be in the future");
        DeadLine = deadLine;
    }
    public void Complete()
    {
        IsCompleted = true;
    }

    public class TaskBuilder
    {
        private readonly string id;
        private readonly string name;
        private string description;
        private Priority priority = Priority.Medium;
        private DateTime deadLine = DateTime.MaxValue;
        private DateTime creationTime = DateTime.MinValue;
        private bool isCompleted = false;

        public TaskBuilder(string id, string name)
        {
            this.id = id;
            this.name = name;
        }
        public TaskBuilder SetDescription(string description)
        {
            this.description = description;
            return this;
        }
        public TaskBuilder SetPriority(Priority priority)
        {
            this.priority = priority;
            return this;
        }
        public TaskBuilder SetDeadLine(DateTime deadLine)
        {
            //Le contrôle est fait ici pour éviter les erreurs lors de la récupération d'anciennes taches via le Json
            if (deadLine < DateTime.Now)
                throw new ArgumentException("DeadLine must be in the future");

            this.deadLine = deadLine;
            return this;
        }
        public TaskBuilder SetCreationTime(DateTime creationTime)
        {
            this.creationTime = creationTime;
            return this;
        }
        public TaskBuilder SetIsCompleted(bool isCompleted)
        {
            this.isCompleted = isCompleted;
            return this;
        }
        public Task Build()
        {
            return new Task(id, name)
            {
                DeadLine = deadLine,
                Description = description,
                Priority = priority,
                CreationTime = creationTime,
                IsCompleted = isCompleted
            };
        }
    }
}
