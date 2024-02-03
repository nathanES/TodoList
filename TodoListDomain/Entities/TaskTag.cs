using Newtonsoft.Json;

namespace TodoList.Domain.Entities;

public class TaskTag
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

    public static TaskTag Default = new(Task.Default, Tag.Default);
    public TaskTag(Task task, Tag tag)
    {
        Id = Guid.NewGuid().ToString();//TODO corriger les problèmes de l'id de taskTag
        Task = task;
        Tag = tag;
    }

    [JsonConstructor]
    private TaskTag(string id, string taskId, Task task, string tagId, Tag tag)
    {
        Id = id;
        Task = task;
        Tag = tag;
    }
}
