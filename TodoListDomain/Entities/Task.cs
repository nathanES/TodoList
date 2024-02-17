using Newtonsoft.Json;
using TodoList.Domain.Enum;
using TodoList.Domain.Exceptions;

namespace TodoList.Domain.Entities;

public class Task
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
            ArgumentException.ThrowIfNullOrEmpty(value, nameof(name));
            name = value;
        }
    }

    [JsonProperty("Description")]
    public string? Description { get; private set; }
    [JsonProperty("Priority")]
    public Priority Priority { get; private set; } = Priority.Medium;
    [JsonProperty("DeadLine")]
    public DateTime Deadline { get; private set; } = DateTime.MaxValue;
    [JsonIgnore]
    public TimeSpan TimeLeftBeforeDeadline
    {
        get
        {
            return Deadline == DateTime.MaxValue ? TimeSpan.MaxValue : Deadline - DateTime.UtcNow;
        }
    }

    [JsonProperty("CreationTime")]
    public DateTime CreationTime { get; private set; } = DateTime.UtcNow;
    [JsonProperty("IsCompleted")]
    public bool IsCompleted { get; private set; } = false;
    public static Task Default = new(Guid.Parse("00000000-0000-0000-0000-000000000000"), "___Default")
    {
        Description = string.Empty,
        Priority = Priority.Medium,
        Deadline = DateTime.MinValue,
        CreationTime = DateTime.MinValue,
        IsCompleted = false
    };
    private Task(string name)
    {
        Id = Guid.NewGuid();
        Name = name;
    }
    private Task(Guid id, string name)
    {
        Id = id;
        Name = name;
    }
    [JsonConstructor]
    private Task(string id, string name, string description, Priority priority, DateTime deadline, DateTime creationTime, bool isCompleted)
    {
        if (!Guid.TryParse(id, out Guid idFormated))
            throw new ArgumentException("Id must be a valid Guid");
        Id = idFormated;
        Name = name;
        Description = description;
        Priority = priority;
        Deadline = deadline;
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
    /// <summary>
    /// Modifie la date limite de la tâche. La date ne peut pas être dans le passé.
    /// </summary>
    /// <param name="value">La date limite à définir.</param>
    /// <exception cref="DeadlineInThePastException">Si la date limite est dans le passé.</exception>
    public void UpdateDeadLine(DateTime deadline)
    {
        //Le contrôle est fait ici pour éviter les erreurs lors de la récupération d'anciennes taches via le Json
        if (IsDeadlineInPast(deadline))
            throw new DeadlineInThePastException($"{nameof(Deadline)} must be in the future");
        Deadline = deadline;
    }
    public void Complete()
    {
        IsCompleted = true;
    }
    private static bool IsDeadlineInPast(DateTime deadLine)
    {
        return deadLine < DateTime.UtcNow;
    }
    public class TaskBuilder
    {
        private Guid _id = Guid.Empty;
        private readonly string _name;
        private string _description;
        private Priority _priority = Priority.Medium;
        private DateTime _deadline = DateTime.MaxValue;
        private DateTime _creationTime = DateTime.MinValue;
        private bool _isCompleted = false;

        public TaskBuilder(string name)
        {
            _name = name;
        }
        public TaskBuilder SetId(Guid id)
        {
            _id = id;
            return this;
        }
        public TaskBuilder SetDescription(string description)
        {
            _description = description;
            return this;
        }
        public TaskBuilder SetPriority(Priority priority)
        {
            _priority = priority;
            return this;
        }
        /// <summary>
        /// Définit la date limite de la tâche. La date ne peut pas être dans le passé.
        /// </summary>
        /// <param name="value">La date limite à définir.</param>
        /// <returns>Le builder pour chaînage.</returns>
        /// <exception cref="DeadlineInThePastException">Si la date limite est dans le passé.</exception>
        public TaskBuilder SetDeadLine(DateTime deadline)
        {
            if (IsDeadlineInPast(deadline))
                throw new DeadlineInThePastException("Deadline must be in the future");

            _deadline = deadline;
            return this;
        }
        public TaskBuilder SetCreationTime(DateTime creationTime)
        {
            _creationTime = creationTime;
            return this;
        }
        public TaskBuilder SetIsCompleted(bool isCompleted)
        {
            _isCompleted = isCompleted;
            return this;
        }
        public Task Build()
        {
            if (_id == Guid.Empty)
            {
                return new Task(_name)
                {
                    Description = _description,
                    Priority = _priority,
                    Deadline = _deadline,
                    CreationTime = _creationTime,
                    IsCompleted = _isCompleted
                };
            }

            return new Task(_id, _name)
            {
                Deadline = _deadline,
                Description = _description,
                Priority = _priority,
                CreationTime = _creationTime,
                IsCompleted = _isCompleted
            };
        }
    }
}
