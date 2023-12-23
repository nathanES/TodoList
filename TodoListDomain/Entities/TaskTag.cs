using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoList.Domain.Entities
{
  public class TaskTag
  {
    public string Id { get; } = Guid.NewGuid().ToString();
    public string TaskId { get => Task.Id; }
    public Task Task { get; set; }

    public string TagId { get => Tag.Id; }
    public Tag Tag { get; set; }

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
      this.Tag = tag;        
    }
  }

}
