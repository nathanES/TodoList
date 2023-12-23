using Newtonsoft.Json;
using TodoList.Domain.Enum;

namespace TodoList.Domain.Entities;

public class Task
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
  [JsonProperty("Priority")]
  public Priority Priority { get; private set; } = Priority.Medium;
  
  private DateTime deadLine = DateTime.MaxValue;
  [JsonProperty("DeadLine")]
  public DateTime DeadLine
  {
    get { return deadLine; }
    private set
    {
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
  public DateTime CreationTime { get; private set; } = DateTime.UtcNow;
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
    private string id = Guid.NewGuid().ToString();
    private string name;
    private string description;
    private Priority priority = Priority.Medium;
    private DateTime deadLine = DateTime.MaxValue;
    private DateTime creationTime = DateTime.MinValue;
    private bool isCompleted = false;

    public TaskBuilder SetId(string id)
    {
      this.id = id;
      return this;
    }
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
      return new Task(name)
      { 
        Id = id,
        DeadLine = deadLine,
        Description = description,
        Priority = priority,
        CreationTime = creationTime,
        IsCompleted = isCompleted
      };
    }
  }
}
