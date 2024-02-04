using TodoList.Domain.Enum;

namespace TodoList.Domain.UnitTest;

[TestClass]
public class TaskCreation
{
    [TestMethod]
    [DataRow("Task 1")]
    public void Constructor_ShouldCreateTask_WithGivenName(string name)
    {
        Task task = new Task.TaskBuilder(name)
            .Build();

        Assert.AreEqual(task.Name, name);
        Assert.IsNotNull(task.Id);
        Assert.IsNull(task.Description);
        Assert.AreEqual(task.Priority, Priority.Medium);
        Assert.AreEqual(task.DeadLine, DateTime.MaxValue);
        Assert.IsTrue(task.TimeLeftBeforeDeadLine == TimeSpan.MaxValue);
        Assert.IsTrue(task.CreationTime <= DateTime.UtcNow);
    }

    [TestMethod]
    [DataRow("Task 1", Priority.High)]
    public void Constructor_ShouldCreateTask_WithPriority(string name, Priority priority)
    {
        Task task = new Task.TaskBuilder(name)
            .SetPriority(priority)
            .Build();

        Assert.AreEqual(task.Name, name);
        Assert.IsNotNull(task.Id);
        Assert.IsNull(task.Description);
        Assert.AreEqual(task.Priority, Priority.High);
        Assert.AreEqual(task.DeadLine, DateTime.MaxValue);
        Assert.IsTrue(task.TimeLeftBeforeDeadLine == TimeSpan.MaxValue);
        Assert.IsTrue(task.CreationTime <= DateTime.UtcNow);
    }

    [TestMethod]
    [DataRow("Task 1", "Description 1")]
    public void Constructor_ShouldCreateTask_WithDescription(string name, string description)
    {
        Task task = new Task.TaskBuilder(name)
            .SetDescription(description)
            .Build();

        Assert.AreEqual(task.Name, name);
        Assert.IsNotNull(task.Id);
        Assert.AreEqual(task.Description, description);
        Assert.AreEqual(task.Priority, Priority.Medium);
        Assert.AreEqual(task.DeadLine, DateTime.MaxValue);
        Assert.IsTrue(task.TimeLeftBeforeDeadLine == TimeSpan.MaxValue);
        Assert.IsTrue(task.CreationTime <= DateTime.UtcNow);
    }

    [TestMethod]
    [DataRow("Task 1")]
    public void Constructor_ShouldCreateTask_WithDeadline(string name)
    {
        DateTime deadLine = DateTime.UtcNow.AddDays(1);
        Task task = new Task.TaskBuilder(name)
            .SetDeadLine(deadLine)
            .Build();

        Assert.AreEqual(task.Name, name);
        Assert.IsNotNull(task.Id);
        Assert.IsNull(task.Description);
        Assert.AreEqual(task.Priority, Priority.Medium);
        Assert.AreEqual(task.DeadLine, deadLine);
        Assert.IsTrue(task.TimeLeftBeforeDeadLine >= deadLine - DateTime.UtcNow);
        Assert.IsFalse(task.TimeLeftBeforeDeadLine == TimeSpan.MaxValue);
        Assert.IsTrue(task.CreationTime <= DateTime.UtcNow);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    [DataRow(null!)]
    public void Constructor_ShouldThrowArgumentNullException_WhenNameIsNull(string name)
    {
        _ = new Task.TaskBuilder(name)
          .Build();
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    [DataRow("")]
    public void Constructor_ShouldThrowArgumentNullException_WhenNameIsEmpty(string name)
    {
        _ = new Task.TaskBuilder(name)
          .Build();
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    [DataRow("Task 2")]
    public void Constructor_ShouldThrowArgumentException_WhenDeadLineInThePast(string name)
    {
        DateTime deadLine = DateTime.UtcNow.AddDays(-1);
        _ = new Task.TaskBuilder(name)
          .SetDeadLine(deadLine)
          .Build();
    }
}