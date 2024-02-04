using Moq;
using TodoList.Domain.Enum;
using TodoList.Domain.Exceptions;
using TodoList.Domain.Interfaces.Logger;
using TodoList.Domain.Interfaces.Repositories;
using TodoList.Infrastructure.Loggers;
using TodoList.Infrastructure.Repositories;

namespace TodoList.Infrastructure.UnitTest.CRUD;

[TestClass]
public class TaskCRUD
{
    private ITaskRepository? _taskRepository;
    private ILogger? _logger;
    private readonly Mock<ILogDestination> _logDestination = new();

    [TestInitialize]
    public void TaskCRUDInitialize()
    {
        _logger = new LoggerCustom(_logDestination.Object);
        _taskRepository = new TaskRepositoryJson(_logger);
    }

    [TestMethod]
    [DataRow("Add Task", "Description", Priority.High)]
    public void AddTask(string name, string description, Priority priority)
    {
        //Arrange
        Task task = new Task.TaskBuilder(name)
            .SetDescription(description)
            .SetPriority(priority)
            .Build();
        //Act
        _ = _taskRepository.AddTask(task);
        //Assert
        Assert.IsTrue(_taskRepository.GetAllTasks().Any(t => t.Id == task.Id));
    }

    [TestMethod]
    [DataRow("Add Task ", "Description", Priority.High)]
    public void AddTask_DuplicateKey(string name, string description, Priority priority)
    {
        //Arrange
        Task task = new Task.TaskBuilder(name)
            .SetDescription(description)
            .SetPriority(priority)
            .Build();
        bool addResult = _taskRepository.AddTask(task);
        Assert.IsTrue(addResult);
        Assert.IsTrue(_taskRepository.GetAllTasks().Any(t => t.Id == task.Id));
        //Act and Assert
        _ = Assert.ThrowsException<DuplicateKeyException>(() => _taskRepository.AddTask(task: task));

    }
    [TestMethod]
    [DataRow("Delete Task", "Description", Priority.High)]
    public void DeleteTask(string name, string description, Priority priority)
    {
        //Arrange
        Task task = new Task.TaskBuilder(name)
            .SetDescription(description)
            .SetPriority(priority)
            .Build();

        _ = _taskRepository.AddTask(task);
        //Act
        bool deleteResult = _taskRepository.DeleteTaskById(task.Id);
        //Assert
        Assert.IsTrue(deleteResult);
        Assert.IsFalse(_taskRepository.GetAllTasks().Any(t => t.Id == task.Id));
    }
    [TestMethod]
    [DataRow("Delete Tasks", "Description", Priority.High)]
    public void DeleteTasks(string name, string description, Priority priority)
    {
        //Arrange
        Task task = new Task.TaskBuilder(name)
            .SetDescription(description)
            .SetPriority(priority)
            .Build();
        _ = _taskRepository.AddTask(task);

        Task task2 = new Task.TaskBuilder(name)
          .SetDescription(description)
          .SetPriority(priority)
          .Build();
        _ = _taskRepository.AddTask(task2);

        //Act
        bool deleteResult = _taskRepository.DeleteTaskByIds(new List<Guid>() { task.Id, task2.Id });
        //Assert
        Assert.IsTrue(deleteResult);
        Assert.IsFalse(_taskRepository.GetAllTasks().Any(t => t.Id == task.Id));
        Assert.IsFalse(_taskRepository.GetAllTasks().Any(t => t.Id == task2.Id));
    }
    [TestMethod]
    public void DeleteTask_NotFound()
    {
        //Act
        bool deleteResult = _taskRepository.DeleteTaskById(Guid.NewGuid());
        //Assert
        Assert.IsFalse(deleteResult);
    }
    [TestMethod]
    public void DeleteTasks_NotFound()
    {
        IEnumerable<Guid> guids = new List<Guid>() { Guid.NewGuid(), Guid.NewGuid(), };

        //Act
        bool deleteResult = _taskRepository.DeleteTaskByIds(guids);
        //Assert
        Assert.IsFalse(deleteResult);
    }
    [TestMethod]
    [DataRow("Update Task", "Description", Priority.High)]
    public void UpdateTask(string name, string description, Priority priority)
    {
        //Arrange
        Task task = new Task.TaskBuilder(name)
            .SetDescription(description)
            .SetPriority(priority)
            .Build();

        _ = _taskRepository.AddTask(task);

        task.UpdateName("UpdateTask");
        //Act
        bool updateResult = _taskRepository.UpdateTask(task);
        //Assert
        Assert.IsTrue(updateResult);
        Assert.IsTrue(_taskRepository.GetAllTasks().Any(t => t.Name == "UpdateTask"));
    }
    [TestMethod]
    [DataRow("Update Task NotFound", "Description", Priority.High)]
    public void UpdateTask_NotFound(string name, string description, Priority priority)
    {
        //Arrange
        Task task = new Task.TaskBuilder(name)
            .SetDescription(description)
            .SetPriority(priority)
            .Build();

        //Act
        bool updateResult = _taskRepository.UpdateTask(task);
        //Assert
        Assert.IsFalse(updateResult);
        Assert.IsFalse(_taskRepository.GetAllTasks().Any(t => t.Name == name));
    }
    [TestMethod]
    [DataRow("GetTaskById", "Description", Priority.High)]
    public void GetTaskById(string name, string description, Priority priority)
    {
        //Arrange
        Task task = new Task.TaskBuilder(name)
            .SetDescription(description)
            .SetPriority(priority)
            .Build();
        _ = _taskRepository.AddTask(task);
        //Act
        Task taskFound = _taskRepository.GetTaskById(task.Id);
        //Assert
        TaskCompare(taskFound, task);

    }
    [TestMethod]
    [DataRow("Get Task By Id", "Description", Priority.High)]
    public void GetTaskById_NotFound(string name, string description, Priority priority)
    {
        //Arrange
        Task task = new Task.TaskBuilder(name)
            .SetDescription(description)
            .SetPriority(priority)
            .Build();
        //Act
        Task taskFound = _taskRepository.GetTaskById(task.Id);
        //Assert
        TaskCompare(taskFound, Task.Default);
    }
    [TestMethod]
    [DataRow("Get All Tasks", "Description", Priority.High)]
    public void GetAllTasks(string name, string description, Priority priority)
    {
        //Arrange
        Task task = new Task.TaskBuilder(name)
            .SetDescription(description)
            .SetPriority(priority)
            .Build();
        _ = _taskRepository.AddTask(task);
        //Act
        IEnumerable<Task> tasks = _taskRepository.GetAllTasks();
        //Assert  
        Assert.IsTrue(tasks.Any());
    }
    public static void TaskCompare(Task task, Task task2)
    {
        Assert.AreEqual(task.Id, task2.Id);
        Assert.AreEqual(task.Name, task2.Name);
        Assert.AreEqual(task.Description, task2.Description);
        Assert.AreEqual(task.Priority, task2.Priority);
        Assert.AreEqual(task.IsCompleted, task2.IsCompleted);
        Assert.AreEqual(task.CreationTime, task2.CreationTime);
        Assert.AreEqual(task.DeadLine, task2.DeadLine);
    }
}
