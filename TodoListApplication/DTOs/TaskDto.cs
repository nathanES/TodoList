using TodoList.Domain.Enum;

namespace TodoList.Application.DTOs;

public class TaskDto
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public Priority Priority { get; set; }
    public DateTime DeadLine { get; set; }
    public DateTime CreationTime { get; set; }
    public bool IsCompleted { get; set; }

    public TaskDto() { }

    public static explicit operator Task(TaskDto taskDto)
    {
        return new Task.TaskBuilder()
            .SetId(taskDto.Id)
            .SetName(taskDto.Name)
            .SetDescription(taskDto.Description)
            .SetPriority(taskDto.Priority)
            .SetDeadLine(taskDto.DeadLine)
            .SetCreationTime(taskDto.CreationTime)
            .SetIsCompleted(taskDto.IsCompleted)
            .Build();
    }
    public static explicit operator TaskDto(Task task)
    {
        return new TaskDto()
        {
            Id = task.Id,
            Name = task.Name,
            Description = task.Description,
            Priority = task.Priority,
            DeadLine = task.DeadLine,
            CreationTime = task.CreationTime,
            IsCompleted = task.IsCompleted
        };
    }
}
