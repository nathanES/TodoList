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
        Task task = new Task.TaskBuilder(name)
            .Build();

        task.UpdateDescription(description);

        Assert.AreEqual(name, task.Name);
        Assert.IsNotNull(task.Id);
        Assert.AreEqual(description, task.Description);
        Assert.AreEqual(Priority.Medium, task.Priority);
        Assert.AreEqual(DateTime.MaxValue, task.DeadLine);
        Assert.IsTrue(task.TimeLeftBeforeDeadLine == TimeSpan.MaxValue);
        Assert.IsTrue(task.CreationTime <= DateTime.UtcNow);
    }

    [TestMethod]
    [DataRow("nouvelle Description")]
    public void CreationTaskUpdateDescription_ShouldUpdateDescription(string description2)
    {
        string name = "Task 1";
        string description = "Description 1";
        Task task = new Task.TaskBuilder(name)
            .SetDescription(description)
            .Build();

        Assert.AreEqual(name, task.Name);
        Assert.IsNotNull(task.Id);
        Assert.AreEqual(description, task.Description);
        Assert.AreEqual(Priority.Medium, task.Priority);
        Assert.AreEqual(DateTime.MaxValue, task.DeadLine);
        Assert.IsTrue(task.TimeLeftBeforeDeadLine == TimeSpan.MaxValue);
        Assert.IsTrue(task.CreationTime <= DateTime.UtcNow);

        task.UpdateDescription(description2);

        Assert.AreEqual(name, task.Name);
        Assert.IsNotNull(task.Id);
        Assert.AreEqual(description2, task.Description);
        Assert.AreEqual(Priority.Medium, task.Priority);
        Assert.AreEqual(DateTime.MaxValue, task.DeadLine);
        Assert.IsTrue(task.TimeLeftBeforeDeadLine == TimeSpan.MaxValue);
        Assert.IsTrue(task.CreationTime <= DateTime.UtcNow);
    }

    [TestMethod]
    [DataRow(Priority.High)]
    public void UpdatePriority_ShouldUpdatePriority(Priority priority)
    {
        string name = "Task 1";
        Task task = new Task.TaskBuilder(name)
            .Build();

        task.UpdatePriority(priority);

        Assert.AreEqual(name, actual: task.Name);
        Assert.IsNotNull(task.Id);
        Assert.IsNull(task.Description);
        Assert.AreEqual(Priority.High, task.Priority);
        Assert.AreEqual(DateTime.MaxValue, task.DeadLine);
        Assert.IsTrue(task.TimeLeftBeforeDeadLine == TimeSpan.MaxValue);
        Assert.IsTrue(task.CreationTime <= DateTime.UtcNow);
    }

    [TestMethod]
    [DataRow(Priority.High)]
    public void CreationTaskUpdatePriority_ShouldUpdatePriority(Priority priority2)
    {
        string name = "Task 1";
        Priority priority = Priority.Low;
        Task task = new Task.TaskBuilder(name)
            .SetPriority(priority)
            .Build();

        Assert.AreEqual(name, actual: task.Name);
        Assert.IsNotNull(task.Id);
        Assert.IsNull(task.Description);
        Assert.AreEqual(Priority.Low, task.Priority);
        Assert.AreEqual(DateTime.MaxValue, task.DeadLine);
        Assert.IsTrue(task.TimeLeftBeforeDeadLine == TimeSpan.MaxValue);
        Assert.IsTrue(task.CreationTime <= DateTime.UtcNow);

        task.UpdatePriority(priority2);

        Assert.AreEqual(name, task.Name);
        Assert.IsNotNull(task.Id);
        Assert.IsNull(task.Description);
        Assert.AreEqual(Priority.High, task.Priority);
        Assert.AreEqual(DateTime.MaxValue, task.DeadLine);
        Assert.IsTrue(task.TimeLeftBeforeDeadLine == TimeSpan.MaxValue);
        Assert.IsTrue(task.CreationTime <= DateTime.UtcNow);
    }

    [TestMethod]
    public void UpdateDeadLine_ShouldUpdateDeadLine()
    {
        string name = "Task 1";
        Task task = new Task.TaskBuilder(name)
            .Build();

        DateTime deadLine = DateTime.UtcNow.AddDays(1);
        task.UpdateDeadLine(deadLine);

        Assert.AreEqual(name, task.Name);
        Assert.IsNotNull(task.Id);
        Assert.IsNull(task.Description);
        Assert.AreEqual(Priority.Medium, task.Priority);
        Assert.AreEqual(deadLine, task.DeadLine);
        Assert.IsTrue(task.TimeLeftBeforeDeadLine >= deadLine - DateTime.UtcNow);
        Assert.IsFalse(task.TimeLeftBeforeDeadLine == TimeSpan.MaxValue);
        Assert.IsTrue(task.CreationTime <= DateTime.UtcNow);
    }

    [TestMethod]
    public void CreationTaskUpdateDeadLine_ShouldUpdateDeadLine()
    {
        string name = "Task 1";
        DateTime deadLine = DateTime.UtcNow.AddDays(1);
        Task task = new Task.TaskBuilder(name)
            .SetDeadLine(deadLine)
            .Build();

        Assert.AreEqual(name, task.Name);
        Assert.IsNotNull(task.Id);
        Assert.IsNull(task.Description);
        Assert.AreEqual(Priority.Medium, task.Priority);
        Assert.AreEqual(deadLine, task.DeadLine);
        Assert.IsTrue(task.TimeLeftBeforeDeadLine >= deadLine - DateTime.UtcNow);
        Assert.IsFalse(task.TimeLeftBeforeDeadLine == TimeSpan.MaxValue);
        Assert.IsTrue(task.CreationTime <= DateTime.UtcNow);

        DateTime deadLine2 = DateTime.UtcNow.AddDays(3);
        task.UpdateDeadLine(deadLine2);

        Assert.AreEqual(name, task.Name);
        Assert.IsNotNull(task.Id);
        Assert.IsNull(task.Description);
        Assert.AreEqual(Priority.Medium, task.Priority);
        Assert.AreEqual(deadLine2, task.DeadLine);
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

        Task task = new Task.TaskBuilder(name)
          .Build();

        task.UpdateDeadLine(deadLine);
    }
}
