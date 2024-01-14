using TodoList.Domain.Enum;
using TodoList.Domain.Interfaces;
using TodoList.Domain.Interfaces.Repositories;
using TodoList.Infrastructure.Repositories;

namespace TodoList.Infrastructure.UnitTest.CRUD;

[TestClass]
public class TaskCRUD
{
    private ITaskRepository? taskRepository;
    private ILogger? logger;
    [TestInitialize]
    public void TaskCRUDInitialize()
    {
        logger = new LoggerCustom();
        taskRepository = new TaskRepositoryJson(logger);
    }

    [TestMethod]
    [DataRow("Add Task", "Description", Priority.High)]
    public void AddTask(string name, string description, Priority priority)
    {
        //Arrange
        Task task = new Task.TaskBuilder()
            .SetName(name)
            .SetDescription(description)
            .SetPriority(priority)
            .Build();
        //Act
        taskRepository.AddTask(task);
        //Assert
        Assert.IsTrue(taskRepository.GetAllTasks().Any(t => t.Id == task.Id));
    }

    [TestMethod]
    [DataRow("Delete Task", "Description", Priority.High)]
    public void DeleteTask(string name, string description, Priority priority)
    {
        //Arrange
        Task task = new Task.TaskBuilder()
            .SetName(name)
            .SetDescription(description)
            .SetPriority(priority)
            .Build();

        taskRepository.AddTask(task);
        //Act
        taskRepository.DeleteTaskById(task.Id);
        //Assert
        Assert.IsFalse(taskRepository.GetAllTasks().Any(t => t.Id == task.Id));
    }
    [TestMethod]
    [DataRow("Delete Tasks", "Description", Priority.High)]
    public void DeleteTasks(string name, string description, Priority priority)
    {
        //Arrange
        Task task = new Task.TaskBuilder()
            .SetName(name)
            .SetDescription(description)
            .SetPriority(priority)
            .Build();
        taskRepository.AddTask(task);

        Task task2 = new Task.TaskBuilder()
          .SetName(name)
          .SetDescription(description)
          .SetPriority(priority)
          .Build();
        taskRepository.AddTask(task2);

        //Act
        taskRepository.DeleteTaskByIds(new List<string>() { task.Id, task2.Id });
        //Assert
        Assert.IsFalse(taskRepository.GetAllTasks().Any(t => t.Id == task.Id));
        Assert.IsFalse(taskRepository.GetAllTasks().Any(t => t.Id == task2.Id));
    }

    [TestMethod]
    [DataRow("Update Task", "Description", Priority.High)]
    public void UpdateTask(string name, string description, Priority priority)
    {
        //Arrange
        Task task = new Task.TaskBuilder()
            .SetName(name)
            .SetDescription(description)
            .SetPriority(priority)
            .Build();

        taskRepository.AddTask(task);

        task.UpdateName("Test2");
        //Act
        taskRepository.UpdateTask(task);
        //Assert
        Assert.IsTrue(taskRepository.GetAllTasks().Any(t => t.Name == "Test2"));
    }

    [TestMethod]
    [DataRow("Get Task By Id", "Description", Priority.High)]
    public void GetTaskById(string name, string description, Priority priority)
    {
        //Arrange
        Task task = new Task.TaskBuilder()
            .SetName(name)
            .SetDescription(description)
            .SetPriority(priority)
            .Build();
        taskRepository.AddTask(task);
        //Act
        Task taskFound = taskRepository.GetTaskById(task.Id);
        //Assert
        Assert.AreEqual(taskFound.Id, task.Id);
    }

    [TestMethod]
    [DataRow("Get All Tasks", "Description", Priority.High)]
    public void GetAllTasks(string name, string description, Priority priority)
    {
        //Arrange
        Task task = new Task.TaskBuilder()
            .SetName(name)
            .SetDescription(description)
            .SetPriority(priority)
            .Build();
        taskRepository.AddTask(task);
        //Act
        _ = taskRepository.GetAllTasks();
        //Assert  
        Assert.IsTrue(taskRepository.GetAllTasks().Any());
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
