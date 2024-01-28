using Moq;
using TodoList.Domain.Enum;
using TodoList.Domain.Interfaces.Logger;
using TodoList.Domain.Interfaces.Repositories;
using TodoList.Infrastructure.Loggers;
using TodoList.Infrastructure.Repositories;

namespace TodoList.Infrastructure.UnitTest.CRUD;

[TestClass]
public class TaskCRUD
{
    //TODO continuer la modification des tests comme  TagCRUD
    private ITaskRepository? taskRepository;
    private ILogger? logger;
    private readonly Mock<ILogDestination> logDestination = new();

    [TestInitialize]
    public void TaskCRUDInitialize()
    {
        logger = new LoggerCustom(logDestination.Object);
        taskRepository = new TaskRepositoryJson(logger);
    }

    [TestMethod]
    [DataRow("Add Task", "Description", Priority.High)]
    public void AddTask(string name, string description, Priority priority)
    {
        //Arrange
        Task task = new Task.TaskBuilder(Guid.NewGuid().ToString(), name)
            .SetDescription(description)
            .SetPriority(priority)
            .Build();
        //Act
        _ = taskRepository.AddTask(task);
        //Assert
        Assert.IsTrue(taskRepository.GetAllTasks().Any(t => t.Id == task.Id));
    }

    [TestMethod]
    [DataRow("Delete Task", "Description", Priority.High)]
    public void DeleteTask(string name, string description, Priority priority)
    {
        //Arrange
        Task task = new Task.TaskBuilder(Guid.NewGuid().ToString(), name)
            .SetDescription(description)
            .SetPriority(priority)
            .Build();

        _ = taskRepository.AddTask(task);
        //Act
        _ = taskRepository.DeleteTaskById(task.Id);
        //Assert
        Assert.IsFalse(taskRepository.GetAllTasks().Any(t => t.Id == task.Id));
    }
    [TestMethod]
    [DataRow("Delete Tasks", "Description", Priority.High)]
    public void DeleteTasks(string name, string description, Priority priority)
    {
        //Arrange
        Task task = new Task.TaskBuilder(Guid.NewGuid().ToString(), name)
            .SetDescription(description)
            .SetPriority(priority)
            .Build();
        _ = taskRepository.AddTask(task);

        Task task2 = new Task.TaskBuilder(Guid.NewGuid().ToString(), name)
          .SetDescription(description)
          .SetPriority(priority)
          .Build();
        _ = taskRepository.AddTask(task2);

        //Act
        _ = taskRepository.DeleteTaskByIds(new List<string>() { task.Id, task2.Id });
        //Assert
        Assert.IsFalse(taskRepository.GetAllTasks().Any(t => t.Id == task.Id));
        Assert.IsFalse(taskRepository.GetAllTasks().Any(t => t.Id == task2.Id));
    }

    [TestMethod]
    [DataRow("Update Task", "Description", Priority.High)]
    public void UpdateTask(string name, string description, Priority priority)
    {
        //Arrange
        Task task = new Task.TaskBuilder(Guid.NewGuid().ToString(), name)
            .SetDescription(description)
            .SetPriority(priority)
            .Build();

        _ = taskRepository.AddTask(task);

        task.UpdateName("UpdateTask");
        //Act
        bool updateResult = taskRepository.UpdateTask(task);
        //Assert
        Assert.IsTrue(updateResult);
        Assert.IsTrue(taskRepository.GetAllTasks().Any(t => t.Name == "UpdateTask"));
    }
    [TestMethod]
    [DataRow("Update Task NotFound", "Description", Priority.High)]
    public void UpdateTask_NotFound(string name, string description, Priority priority)
    {
        //Arrange
        Task task = new Task.TaskBuilder(Guid.NewGuid().ToString(), name)
            .SetDescription(description)
            .SetPriority(priority)
            .Build();

        _ = taskRepository.AddTask(task);

        task.UpdateName("UpdateTask_NotFound");
        //Act
        bool updateResult = taskRepository.UpdateTask(task);
        //Assert
        Assert.IsFalse(updateResult);
        Assert.IsFalse(taskRepository.GetAllTasks().Any(t => t.Name == "UpdateTask_NotFound"));
    }
    [TestMethod]
    [DataRow("Get Task By Id", "Description", Priority.High)]
    public void GetTaskById(string name, string description, Priority priority)
    {
        //Arrange
        Task task = new Task.TaskBuilder(Guid.NewGuid().ToString(), name)
            .SetDescription(description)
            .SetPriority(priority)
            .Build();
        _ = taskRepository.AddTask(task);
        //Act
        Task taskFound = taskRepository.GetTaskById(task.Id);
        //Assert
        TaskCompare(taskFound, task);

    }
    [TestMethod]
    [DataRow("Get Task By Id", "Description", Priority.High)]
    public void GetTaskById_NotFound(string name, string description, Priority priority)
    {
        //Arrange
        Task task = new Task.TaskBuilder(Guid.NewGuid().ToString(), name)
            .SetDescription(description)
            .SetPriority(priority)
            .Build();
        //Act
        Task taskFound = taskRepository.GetTaskById(task.Id);
        //Assert
        TaskCompare(taskFound, Task.Empty);
    }
    [TestMethod]
    [DataRow("Get All Tasks", "Description", Priority.High)]
    public void GetAllTasks(string name, string description, Priority priority)
    {
        //Arrange
        Task task = new Task.TaskBuilder(Guid.NewGuid().ToString(), name)
            .SetDescription(description)
            .SetPriority(priority)
            .Build();
        _ = taskRepository.AddTask(task);
        //Act
        IEnumerable<Task> tasks = taskRepository.GetAllTasks();
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
        Assert.AreEqual(task.TimeLeftBeforeDeadLine, task2.TimeLeftBeforeDeadLine);
    }
}
