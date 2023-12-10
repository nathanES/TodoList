using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoList.Domain.Entities
{
  public class TaskTag
  {
        public string TaskId { get; set; }
        public Task Task { get; set; }

        public string TagId { get; set; }
        public Tag Tag { get; set; }
    }
}
