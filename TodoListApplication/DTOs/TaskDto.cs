using TodoList.Domain.Enum;

namespace TodoList.Application.DTOs;

public class TaskDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public Priority Priority { get; set; }
    public DateTime DeadLine { get; set; }
    public DateTime CreationTime { get; set; }
    public bool IsCompleted { get; set; }

    public TaskDto() { }

    public static explicit operator Task(TaskDto taskDto)
    {
        if (taskDto == null)
            throw new ArgumentNullException(nameof(taskDto), $"L'objet {nameof(TaskDto)} ne doit pas être null.");

        Task.TaskBuilder builder = new Task.TaskBuilder(taskDto.Name)
            .SetDescription(taskDto.Description)
            .SetPriority(taskDto.Priority)
            .SetDeadline(taskDto.DeadLine)
            .SetCreationTime(taskDto.CreationTime)
            .SetIsCompleted(taskDto.IsCompleted);

        if (taskDto.Id != Guid.Empty)
            builder = builder.SetId(taskDto.Id);
        return builder.Build();
    }
    public static explicit operator TaskDto(Task task)
    {
        return new TaskDto()
        {
            Id = task.Id,
            Name = task.Name,
            Description = task.Description,
            Priority = task.Priority,
            DeadLine = task.Deadline,
            CreationTime = task.CreationTime,
            IsCompleted = task.IsCompleted
        };
    }
}
