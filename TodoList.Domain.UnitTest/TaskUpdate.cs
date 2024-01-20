using TodoList.Domain.Enum;

namespace TodoList.Domain.UnitTest;

[TestClass]
public class TaskUpdate
{
    [TestMethod]
    [DataRow("nouvelle Description")]
    public void UpdateDescription_ShouldUpdateDescription(string description)
    {
        string name = "Task 1";
        Task task = new Task.TaskBuilder(Guid.NewGuid().ToString(), name)
            .Build();

        task.UpdateDescription(description);

        Assert.AreEqual(task.Name, name);
        Assert.IsNotNull(task.Id);
        Assert.AreEqual(task.Description, description);
        Assert.AreEqual(task.Priority, Priority.Medium);
        Assert.AreEqual(task.DeadLine, DateTime.MaxValue);
        Assert.IsTrue(task.TimeLeftBeforeDeadLine == TimeSpan.MaxValue);
        Assert.IsTrue(task.CreationTime <= DateTime.UtcNow);
    }

    [TestMethod]
    [DataRow("nouvelle Description")]
    public void CreationTaskUpdateDescription_ShouldUpdateDescription(string description2)
    {
        string name = "Task 1";
        string description = "Description 1";
        Task task = new Task.TaskBuilder(Guid.NewGuid().ToString(), name)
            .SetDescription(description)
            .Build();

        Assert.AreEqual(task.Name, name);
        Assert.IsNotNull(task.Id);
        Assert.AreEqual(task.Description, description);
        Assert.AreEqual(task.Priority, Priority.Medium);
        Assert.AreEqual(task.DeadLine, DateTime.MaxValue);
        Assert.IsTrue(task.TimeLeftBeforeDeadLine == TimeSpan.MaxValue);
        Assert.IsTrue(task.CreationTime <= DateTime.UtcNow);

        task.UpdateDescription(description2);

        Assert.AreEqual(task.Name, name);
        Assert.IsNotNull(task.Id);
        Assert.AreEqual(task.Description, description2);
        Assert.AreEqual(task.Priority, Priority.Medium);
        Assert.AreEqual(task.DeadLine, DateTime.MaxValue);
        Assert.IsTrue(task.TimeLeftBeforeDeadLine == TimeSpan.MaxValue);
        Assert.IsTrue(task.CreationTime <= DateTime.UtcNow);
    }

    [TestMethod]
    [DataRow(Priority.High)]
    public void UpdatePriority_ShouldUpdatePriority(Priority priority)
    {
        string name = "Task 1";
        Task task = new Task.TaskBuilder(Guid.NewGuid().ToString(), name)
            .Build();

        task.UpdatePriority(priority);

        Assert.AreEqual(task.Name, name);
        Assert.IsNotNull(task.Id);
        Assert.IsNull(task.Description);
        Assert.AreEqual(task.Priority, Priority.High);
        Assert.AreEqual(task.DeadLine, DateTime.MaxValue);
        Assert.IsTrue(task.TimeLeftBeforeDeadLine == TimeSpan.MaxValue);
        Assert.IsTrue(task.CreationTime <= DateTime.UtcNow);
    }

    [TestMethod]
    [DataRow(Priority.High)]
    public void CreationTaskUpdatePriority_ShouldUpdatePriority(Priority priority2)
    {
        string name = "Task 1";
        Priority priority = Priority.Low;
        Task task = new Task.TaskBuilder(Guid.NewGuid().ToString(), name)
            .SetPriority(priority)
            .Build();

        Assert.AreEqual(task.Name, name);
        Assert.IsNotNull(task.Id);
        Assert.IsNull(task.Description);
        Assert.AreEqual(task.Priority, Priority.Low);
        Assert.AreEqual(task.DeadLine, DateTime.MaxValue);
        Assert.IsTrue(task.TimeLeftBeforeDeadLine == TimeSpan.MaxValue);
        Assert.IsTrue(task.CreationTime <= DateTime.UtcNow);

        task.UpdatePriority(priority2);

        Assert.AreEqual(task.Name, name);
        Assert.IsNotNull(task.Id);
        Assert.IsNull(task.Description);
        Assert.AreEqual(task.Priority, Priority.High);
        Assert.AreEqual(task.DeadLine, DateTime.MaxValue);
        Assert.IsTrue(task.TimeLeftBeforeDeadLine == TimeSpan.MaxValue);
        Assert.IsTrue(task.CreationTime <= DateTime.UtcNow);
    }

    [TestMethod]
    public void UpdateDeadLine_ShouldUpdateDeadLine()
    {
        string name = "Task 1";
        Task task = new Task.TaskBuilder(Guid.NewGuid().ToString(), name)
            .Build();

        DateTime deadLine = DateTime.UtcNow.AddDays(1);
        task.UpdateDeadLine(deadLine);

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
    public void CreationTaskUpdateDeadLine_ShouldUpdateDeadLine()
    {
        string name = "Task 1";
        DateTime deadLine = DateTime.UtcNow.AddDays(1);
        Task task = new Task.TaskBuilder(Guid.NewGuid().ToString(), name)
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

        DateTime deadLine2 = DateTime.UtcNow.AddDays(3);
        task.UpdateDeadLine(deadLine2);

        Assert.AreEqual(task.Name, name);
        Assert.IsNotNull(task.Id);
        Assert.IsNull(task.Description);
        Assert.AreEqual(task.Priority, Priority.Medium);
        Assert.AreEqual(task.DeadLine, deadLine2);
        Assert.IsTrue(task.TimeLeftBeforeDeadLine >= deadLine2 - DateTime.UtcNow);
        Assert.IsFalse(task.TimeLeftBeforeDeadLine == TimeSpan.MaxValue);
        Assert.IsTrue(task.CreationTime <= DateTime.UtcNow);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void UpdateDeadLine_ShouldThrowArgumentException_WhenDeadLineInThePast()
    {
        DateTime deadLine = DateTime.UtcNow.AddDays(-1);
        string name = "Task 1";

        Task task = new Task.TaskBuilder(Guid.NewGuid().ToString(), name)
          .Build();

        task.UpdateDeadLine(deadLine);
    }
}
