using System.Globalization;
using TodoList.Domain.Enum;
using TodoList.Domain.Exceptions;

namespace TodoList.Domain.UnitTest;

[TestClass]
public class TaskCreation
{
    [TestMethod]
    [DataRow("TaskName")]
    public void ConstructorTask_WithGivenName_ShouldCreateTask(string taskName)
    {
        Task task = new Task.TaskBuilder(taskName)
            .Build();

        Assert.IsNotNull(task.Id);
        Assert.AreEqual(task.Name, taskName);
        Assert.IsNull(task.Description);
        Assert.AreEqual(Priority.Medium, task.Priority);
        Assert.AreEqual(DateTime.MaxValue, task.Deadline);
        Assert.AreEqual(TimeSpan.MaxValue, task.TimeLeftBeforeDeadline);
        Assert.IsTrue(task.CreationTime <= DateTime.UtcNow);
        Assert.IsFalse(task.IsCompleted);
    }

    [TestMethod]
    [DataRow(Priority.High)]
    public void ConstructorTask_WithGivenPriority_ShouldCreateTask(Priority taskPriority)
    {
        string taskName = nameof(ConstructorTask_WithGivenPriority_ShouldCreateTask);

        Task task = new Task.TaskBuilder(taskName)
            .SetPriority(taskPriority)
            .Build();

        Assert.IsNotNull(task.Id);
        Assert.AreEqual(task.Name, taskName);
        Assert.IsNull(task.Description);
        Assert.AreEqual(Priority.High, task.Priority);
        Assert.AreEqual(DateTime.MaxValue, task.Deadline);
        Assert.AreEqual(TimeSpan.MaxValue, task.TimeLeftBeforeDeadline);
        Assert.IsTrue(task.CreationTime <= DateTime.UtcNow);
        Assert.IsFalse(task.IsCompleted);
    }

    [TestMethod]
    [DataRow("DescriptionName")]
    public void ConstructorTask_WithGivenDescription_ShouldCreateTask(string taskDescription)
    {
        string taskName = nameof(ConstructorTask_WithGivenDescription_ShouldCreateTask);

        Task task = new Task.TaskBuilder(taskName)
            .SetDescription(taskDescription)
            .Build();

        Assert.IsNotNull(task.Id);
        Assert.AreEqual(taskName, task.Name);
        Assert.AreEqual(taskDescription, task.Description);
        Assert.AreEqual(Priority.Medium, task.Priority);
        Assert.AreEqual(DateTime.MaxValue, task.Deadline);
        Assert.IsTrue(task.TimeLeftBeforeDeadline.CompareTo(TimeSpan.MaxValue) == 0);
        Assert.IsTrue(task.CreationTime <= DateTime.UtcNow);
        Assert.IsFalse(task.IsCompleted);
    }

    [TestMethod]
    [DataRow("01/01/2025")]
    [DataRow("01/01/2026")]
    [DataRow("01/01/2027")]
    public void ConstructorTask_WithGivenDeadline_ShouldCreateTask(string taskDeadline)
    {
        string taskName = nameof(ConstructorTask_WithGivenDeadline_ShouldCreateTask);
        DateTime taskDeadlineValue = DateTime.ParseExact(taskDeadline, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None);

        Task task = new Task.TaskBuilder(taskName)
            .SetDeadLine(taskDeadlineValue)
            .Build();

        Assert.IsNotNull(task.Id);
        Assert.AreEqual(taskName, task.Name);
        Assert.IsNull(task.Description);
        Assert.AreEqual(Priority.Medium, task.Priority);
        Assert.AreEqual(taskDeadlineValue, task.Deadline);
        Assert.IsTrue(task.TimeLeftBeforeDeadline >= taskDeadlineValue - DateTime.UtcNow);
        Assert.IsTrue(task.TimeLeftBeforeDeadline.CompareTo(TimeSpan.MaxValue) <= 0);
        Assert.IsTrue(task.CreationTime <= DateTime.UtcNow);
        Assert.IsFalse(task.IsCompleted);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void ConstructorTask_WhenNameIsNull_ShouldNotCreateTask_ArgumentNullException()
    {
        _ = new Task.TaskBuilder(null!)
          .Build();
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void ConstructorTask_WhenNameIsEmpty_ShouldNotCreateTask_ArgumentException()
    {
        _ = new Task.TaskBuilder(string.Empty)
          .Build();
    }

    [TestMethod]
    [ExpectedException(typeof(DeadlineInThePastException))]
    [DataRow("01/01/2020")]
    [DataRow("01/01/2019")]
    [DataRow("01/01/2018")]
    public void ConstructorTask_WhenDeadLineInThePast_ShouldNotCreateTask_ArgumentException(string taskDeadline)
    {
        DateTime taskDeadlineValue = DateTime.ParseExact(taskDeadline, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None);
        string taskName = nameof(ConstructorTask_WhenDeadLineInThePast_ShouldNotCreateTask_ArgumentException);

        _ = new Task.TaskBuilder(taskName)
          .SetDeadLine(taskDeadlineValue)
          .Build();
    }
}