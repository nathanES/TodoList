using TodoList.Domain.Enum;

namespace TodoList.Domain.Entities;

public class Task
{
  public string Id { get; } = Guid.NewGuid().ToString();
  private string name;
  public string Name
  {
    get { return name; }
    private set
    {
      ArgumentException.ThrowIfNullOrEmpty(value, nameof(name));
      name = value;
    }
  }
  public string? Description { get; private set; }
  public Priority Priority { get; private set; } = Priority.Medium;

  private DateTime deadLine = DateTime.MaxValue;
  public DateTime DeadLine
  {
    get { return deadLine; }
    set
    {
      if (value < DateTime.Now)
        throw new ArgumentException("DeadLine must be in the future");

      deadLine = value;
    }
  }

  public TimeSpan TimeLeftBeforeDeadLine
  {
    get
    {
      if (DeadLine == DateTime.MaxValue)
        return TimeSpan.MaxValue;
      return DeadLine - DateTime.UtcNow;
    }
  }
  public DateTime CreationTime { get; } = DateTime.UtcNow;
  public bool IsCompleted { get; private set; } = false;

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
