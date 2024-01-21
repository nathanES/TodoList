using Newtonsoft.Json;

namespace TodoList.Domain.Entities;

public class TaskTag
{
    public string Id { get; } = Guid.NewGuid().ToString();
    public string TaskId
    {
        get
        {
            return Task.Id;
        }
    }
    public Task Task { get; set; }

    public string TagId
    {
        get
        {
            return Tag.Id;
        }
    }
    public Tag Tag { get; set; }

    public static TaskTag Empty = new(Task.Empty, Tag.Empty);
    public TaskTag(Task task, Tag tag)
    {
        Task = task;
        Tag = tag;
    }

    [JsonConstructor]
    private TaskTag(string Id, string TaskId, Task Task, string TagId, Tag tag)
    {
        this.Id = Id;
        this.Task = Task;
        Tag = tag;
    }
}
