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
  }

}
