using System.Globalization;
using TodoList.Domain.Enum;
using TodoList.Domain.Exceptions;

namespace TodoList.Domain.UnitTest;

[TestClass]
public class TaskUpdate
{
    [TestMethod]
    [DataRow("NewTaskName")]
    public void UpdateTask_UpdateTaskName_ShouldUpdateTask(string newTaskName)
    {
        string taskName = nameof(UpdateTask_UpdateTaskName_ShouldUpdateTask);
        Task task = new Task.TaskBuilder(taskName)
            .Build();

        task.UpdateName(newTaskName);

        Assert.IsNotNull(task.Id);
        Assert.AreEqual(newTaskName, task.Name);
        Assert.IsTrue(string.IsNullOrEmpty(task.Description));
        Assert.AreEqual(Priority.Medium, task.Priority);
        Assert.AreEqual(DateTime.MaxValue, task.Deadline);
        Assert.IsTrue(task.TimeLeftBeforeDeadline == TimeSpan.MaxValue);
        Assert.IsTrue(task.CreationTime <= DateTime.UtcNow);
        Assert.IsFalse(task.IsCompleted);
    }

    [TestMethod]
    [DataRow("NouvelleDescription")]
    public void UpdateTask_UpdateTaskDescription_ShouldUpdateTaskDescription(string newTaskDescription)
    {
        string taskName = nameof(UpdateTask_UpdateTaskDescription_ShouldUpdateTaskDescription);
        Task task = new Task.TaskBuilder(taskName)
            .SetDescription("FirstTaskDescription")
            .Build();

        task.UpdateDescription(newTaskDescription);

        Assert.IsNotNull(task.Id);
        Assert.IsFalse(string.IsNullOrEmpty(task.Name));
        Assert.AreEqual(newTaskDescription, task.Description);
        Assert.AreEqual(Priority.Medium, task.Priority);
        Assert.AreEqual(DateTime.MaxValue, task.Deadline);
        Assert.IsTrue(task.TimeLeftBeforeDeadline == TimeSpan.MaxValue);
        Assert.IsTrue(task.CreationTime <= DateTime.UtcNow);
        Assert.IsFalse(task.IsCompleted);
    }

    [TestMethod]
    [DataRow("NouvelleDescription")]
    public void UpdateTask_UpdateTaskDescriptionWhenNoTaskDescription_ShouldUpdateTaskDescription(string newTaskDescription)
    {
        string taskName = nameof(UpdateTask_UpdateTaskDescriptionWhenNoTaskDescription_ShouldUpdateTaskDescription);
        Task task = new Task.TaskBuilder(taskName)
            .Build();

        task.UpdateDescription(newTaskDescription);

        Assert.IsNotNull(task.Id);
        Assert.IsFalse(string.IsNullOrEmpty(task.Name));
        Assert.AreEqual(newTaskDescription, task.Description);
        Assert.AreEqual(Priority.Medium, task.Priority);
        Assert.AreEqual(DateTime.MaxValue, task.Deadline);
        Assert.IsTrue(task.TimeLeftBeforeDeadline == TimeSpan.MaxValue);
        Assert.IsTrue(task.CreationTime <= DateTime.UtcNow);
        Assert.IsFalse(task.IsCompleted);
    }

    [TestMethod]
    [DataRow(Priority.High)]
    public void UpdateTask_UpdateTaskPriority_ShouldUpdateTaskPriority(Priority newTaskPriority)
    {
        string taskName = nameof(UpdateTask_UpdateTaskPriorityWhenNoTaskPriority_ShouldUpdateTaskPriority);
        Task task = new Task.TaskBuilder(taskName)
            .SetPriority(Priority.Low)
            .Build();

        task.UpdatePriority(newTaskPriority);

        Assert.IsNotNull(task.Id);
        Assert.IsFalse(string.IsNullOrEmpty(task.Name));
        Assert.IsTrue(string.IsNullOrEmpty(task.Description));
        Assert.AreEqual(newTaskPriority, task.Priority);
        Assert.AreEqual(DateTime.MaxValue, task.Deadline);
        Assert.IsTrue(task.TimeLeftBeforeDeadline == TimeSpan.MaxValue);
        Assert.IsTrue(task.CreationTime <= DateTime.UtcNow);
        Assert.IsFalse(task.IsCompleted);
    }

    [TestMethod]
    [DataRow(Priority.High)]
    public void UpdateTask_UpdateTaskPriorityWhenNoTaskPriority_ShouldUpdateTaskPriority(Priority newTaskPriority)
    {
        string taskName = nameof(UpdateTask_UpdateTaskPriorityWhenNoTaskPriority_ShouldUpdateTaskPriority);
        Task task = new Task.TaskBuilder(taskName)
            .Build();

        task.UpdatePriority(newTaskPriority);

        Assert.IsNotNull(task.Id);
        Assert.IsFalse(string.IsNullOrEmpty(task.Name));
        Assert.IsTrue(string.IsNullOrEmpty(task.Description));
        Assert.AreEqual(newTaskPriority, task.Priority);
        Assert.AreEqual(DateTime.MaxValue, task.Deadline);
        Assert.IsTrue(task.TimeLeftBeforeDeadline == TimeSpan.MaxValue);
        Assert.IsTrue(task.CreationTime <= DateTime.UtcNow);
        Assert.IsFalse(task.IsCompleted);
    }

    [TestMethod]
    [DataRow("01/01/2025")]
    [DataRow("01/01/2026")]
    [DataRow("01/01/2027")]
    public void UpdateTask_UpdateTaskDeadline_ShouldUpdateTaskDeadline(string taskDeadline)
    {
        string taskName = nameof(UpdateTask_UpdateTaskDeadline_ShouldUpdateTaskDeadline);
        DateTime taskDeadlineValue = DateTime.ParseExact(taskDeadline, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None);

        Task task = new Task.TaskBuilder(taskName)
            .SetDeadLine(DateTime.MaxValue)
            .Build();

        task.UpdateDeadLine(taskDeadlineValue);

        Assert.IsNotNull(task.Id);
        Assert.IsFalse(string.IsNullOrEmpty(task.Name));
        Assert.IsTrue(string.IsNullOrEmpty(task.Description));
        Assert.AreEqual(Priority.Medium, task.Priority);
        Assert.AreEqual(taskDeadlineValue, task.Deadline);
        Assert.IsTrue(task.CreationTime <= DateTime.UtcNow);
        Assert.IsFalse(task.IsCompleted);
    }
    [TestMethod]
    [DataRow("01/02/2025")]
    [DataRow("01/02/2026")]
    [DataRow("01/02/2027")]
    public void UpdateTask_UpdateTaskDeadlineWhenNoTaskDeadline_ShouldUpdateTaskDeadline(string taskDeadline)
    {
        string taskName = nameof(UpdateTask_UpdateTaskDeadlineWhenNoTaskDeadline_ShouldUpdateTaskDeadline);
        DateTime taskDeadlineValue = DateTime.ParseExact(taskDeadline, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None);

        Task task = new Task.TaskBuilder(taskName)
            .SetDeadLine(DateTime.MaxValue)
            .Build();

        task.UpdateDeadLine(taskDeadlineValue);

        Assert.IsNotNull(task.Id);
        Assert.IsFalse(string.IsNullOrEmpty(task.Name));
        Assert.IsTrue(string.IsNullOrEmpty(task.Description));
        Assert.AreEqual(Priority.Medium, task.Priority);
        Assert.AreEqual(taskDeadlineValue, task.Deadline);
        Assert.IsTrue(task.CreationTime <= DateTime.UtcNow);
        Assert.IsFalse(task.IsCompleted);
    }

    [TestMethod]
    [ExpectedException(typeof(DeadlineInThePastException))]
    [DataRow("01/01/2020")]
    [DataRow("01/01/2019")]
    [DataRow("01/01/2018")]
    public void UpdateTask_UpdateTaskDeadlineWhenDeadlineInThePast_ArgumentException(string deadline)
    {
        string taskName = nameof(UpdateTask_UpdateTaskDeadlineWhenDeadlineInThePast_ArgumentException);
        DateTime deadlineValue = DateTime.ParseExact(deadline, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None);

        Task task = new Task.TaskBuilder(taskName)
          .Build();

        task.UpdateDeadLine(deadlineValue);
    }

    [TestMethod]
    public void UpdateTask_UpdateTaskCompletion_ShouldUpdateTaskCompleton()
    {
        string taskName = nameof(UpdateTask_UpdateTaskCompletion_ShouldUpdateTaskCompleton);
        Task task = new Task.TaskBuilder(taskName)
            .Build();

        task.Complete();

        Assert.IsNotNull(task.Id);
        Assert.IsFalse(string.IsNullOrEmpty(task.Name));
        Assert.IsTrue(string.IsNullOrEmpty(task.Description));
        Assert.AreEqual(Priority.Medium, task.Priority);
        Assert.AreEqual(DateTime.MaxValue, task.Deadline);
        Assert.IsTrue(task.TimeLeftBeforeDeadline == TimeSpan.MaxValue);
        Assert.IsTrue(task.CreationTime <= DateTime.UtcNow);
        Assert.IsTrue(task.IsCompleted);
    }
}
