using Newtonsoft.Json;

namespace TodoList.Domain.Entities;

public class TaskTag
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

    [JsonProperty("TaskId")]
    public string TaskIdString
    {
        get
        {
            return TaskId.ToString();
        }
    }

    [JsonIgnore]
    public Guid TaskId
    {
        get
        {
            return Task.Id;
        }
    }

    public Task Task { get; private set; }

    [JsonProperty("TagId")]
    public string TagIdString
    {
        get
        {
            return TagId.ToString();
        }
    }

    [JsonIgnore]
    public Guid TagId
    {
        get
        {
            return Tag.Id;
        }
    }
    public Tag Tag { get; private set; }

    public static TaskTag Default = new(Task.Default, Tag.Default);
    private TaskTag(Task task, Tag tag)
    {
        Id = Guid.NewGuid();
        Task = task;
        Tag = tag;
    }
    private TaskTag(Guid id, Task task, Tag tag)
    {
        Id = id;
        Task = task;
        Tag = tag;
    }
    [JsonConstructor]
    private TaskTag(string id, string taskId, Task task, string tagId, Tag tag)
    {
        if (!Guid.TryParse(id, out Guid idFormated))
            throw new ArgumentException("Id must be a valid Guid");
        Id = idFormated;
        Task = task;
        Tag = tag;
    }
    public void UpdateTag(Tag tag)
    {
        Tag = tag;
    }
    public void UpdateTask(Task task)
    {
        Task = task;
    }
    public class TaskTagBuilder
    {
        private Guid _id = Guid.Empty;
        private readonly Task _task;
        private readonly Tag _tag;

        public TaskTagBuilder(Task task, Tag tag)
        {
            _task = task;
            _tag = tag;
        }

        public TaskTagBuilder SetId(Guid id)
        {
            _id = id;
            return this;
        }

        public TaskTag Build()
        {
            if (_id == Guid.Empty)
                return new TaskTag(_task, _tag);

            return new TaskTag(_id, _task, _tag);
        }
    }
}
