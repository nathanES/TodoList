using Newtonsoft.Json;
using TodoList.Domain.Enum;

namespace TodoList.Domain.Entities;

public class Task
{
  [JsonProperty("Id")]
  public string Id { get; } = Guid.NewGuid().ToString();
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
  [JsonProperty("Priority")]
  public Priority Priority { get; private set; } = Priority.Medium;
  
  private DateTime deadLine = DateTime.MaxValue;
  [JsonProperty("DeadLine")]
  public DateTime DeadLine
  {
    get { return deadLine; }
    private set
    {
      //TODO : Peut poser problème de tester ici lorsqu'on récupère les donneés du fichier json
      if (value < DateTime.Now)
        throw new ArgumentException("DeadLine must be in the future");

      deadLine = value;
    }
  }
  [JsonIgnore]
  public TimeSpan TimeLeftBeforeDeadLine
  {
    get
    {
      if (DeadLine == DateTime.MaxValue)
        return TimeSpan.MaxValue;
      return DeadLine - DateTime.UtcNow;
    }
  }
  [JsonProperty("CreationTime")]
  public DateTime CreationTime { get; } = DateTime.UtcNow;
  [JsonProperty("IsCompleted")]
  public bool IsCompleted { get; private set; } = false;

  [JsonConstructor]
  private Task(string Id, string Name, string Description, Priority Priority, DateTime DeadLine, DateTime CreationTime, bool IsCompleted)
  {
    this.Id = Id;
    this.Name = Name;
    this.Description = Description;
    this.Priority = Priority;
    this.DeadLine = DeadLine;
    this.CreationTime = CreationTime;
    this.IsCompleted = IsCompleted;
  }
  private Task(string name)
  {
    Name = name;
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
    DeadLine = deadLine;
  }
  public void Complete()
  {
    IsCompleted = true;
  }

  //TODO : A revoir, je ne suis pas sur que ce soit une bonne idée
  public static Task Empty => new Task(string.Empty);

  public class TaskBuilder
  {
    private string name;
    private string description;
    private Priority priority = Priority.Medium;
    private DateTime deadLine = DateTime.MaxValue;

    public TaskBuilder SetName(string name)
    {
      this.name = name;
      return this;
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
      this.deadLine = deadLine;
      return this;
    }
    public Task Build()
    {
      return new Task(name)
      { 
        DeadLine = deadLine,
        Description = description,
        Priority = priority
      };
    }
  }
}
