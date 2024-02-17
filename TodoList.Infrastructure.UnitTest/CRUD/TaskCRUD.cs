using Moq;
using System.Globalization;
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
    public void TaskCRUD_AddTask_ShouldAddTask()
    {
        //Arrange
        string taskName = nameof(TaskCRUD_AddTask_ShouldAddTask);
        Task task = new Task.TaskBuilder(taskName)
            .Build();
        //Act
        bool addResult = _taskRepository.AddTask(task);
        //Assert
        Task taskResult = _taskRepository.GetTaskById(task.Id);
        Assert.IsTrue(addResult);
        Assert.AreNotEqual(Task.Default, taskResult);
        Assert.AreEqual(taskName, taskResult.Name);
    }
    [TestMethod]
    [ExpectedException(typeof(DuplicateKeyException))]
    public void TaskCRUD_AddTaskAlreadyExisting_DuplicateKeyException()
    {
        //Arrange
        string taskName = nameof(TaskCRUD_AddTask_ShouldAddTask);
        Task task = new Task.TaskBuilder(taskName)
            .Build();
        //Act
        bool addResult = _taskRepository.AddTask(task);
        _ = _taskRepository.AddTask(task: task);
    }
    [TestMethod]
    public void TaskCRUD_DeleteTask_ShouldDeleteTask()
    {
        string taskName = nameof(TaskCRUD_DeleteTask_ShouldDeleteTask);
        Task task = new Task.TaskBuilder(taskName)
            .Build();
        _ = _taskRepository.AddTask(task);
        //Act
        bool deleteResult = _taskRepository.DeleteTaskById(task.Id);
        //Assert
        Assert.IsTrue(deleteResult);
        Assert.AreEqual(Task.Default, _taskRepository.GetTaskById(task.Id));
    }
    [TestMethod]
    public void TaskCRUD_DeleteTaskWhenNotFound_ShouldReturnFalse()
    {
        //Act
        bool deleteResult = _taskRepository.DeleteTaskById(Guid.NewGuid());
        //Assert
        Assert.IsFalse(deleteResult);
    }
    [TestMethod]
    public void TaskCRUD_DeleteTasks_ShouldDeleteTasks()
    {
        //Arrange
        string taskName = nameof(TaskCRUD_DeleteTask_ShouldDeleteTask);
        Task task1 = new Task.TaskBuilder(taskName)
            .Build();
        _ = _taskRepository.AddTask(task1);

        Task task2 = new Task.TaskBuilder(taskName + "2")
            .Build();
        _ = _taskRepository.AddTask(task2);

        //Act
        bool deleteResult = _taskRepository.DeleteTaskByIds(new List<Guid>() { task1.Id, task2.Id });
        //Assert
        Assert.IsTrue(deleteResult);
        Assert.AreEqual(Task.Default, _taskRepository.GetTaskById(task1.Id));
        Assert.AreEqual(Task.Default, _taskRepository.GetTaskById(task2.Id));
    }
    [TestMethod]
    public void TaskCRUD_DeleteTasksWhenNotFound_ShouldReturnFalse()
    {
        IEnumerable<Guid> guids = new List<Guid>() { Guid.NewGuid(), Guid.NewGuid(), };

        //Act
        bool deleteResult = _taskRepository.DeleteTaskByIds(guids);
        //Assert
        Assert.IsFalse(deleteResult);
    }

    [TestMethod]
    [DataRow("Update Task", "Description", Priority.Low, "31/12/2029", true)]
    public void TaskCRUD_UpdateTaskAllProperties_ShouldUpdateAllProperties(string newTaskName, string newTaskDescription, Priority newTaskPriority, string newTaskDeadline, bool isNewTaskComplete)
    {
        //Arrange
        string taskName = nameof(TaskCRUD_UpdateTaskAllProperties_ShouldUpdateAllProperties);
        DateTime newTaskDeadlineValue = DateTime.ParseExact(newTaskDeadline, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None);

        Task task = new Task.TaskBuilder(taskName)
            .Build();
        _ = _taskRepository.AddTask(task);

        //Act
        task.UpdateName(newTaskName);
        task.UpdateDescription(newTaskDescription);
        task.UpdatePriority(newTaskPriority);
        task.UpdateDeadline(newTaskDeadlineValue);
        if (isNewTaskComplete)
            task.Complete();
        bool updateResult = _taskRepository.UpdateTask(task);
        //Assert
        Task taskResult = _taskRepository.GetTaskById(task.Id);
        Assert.IsTrue(updateResult);
        Assert.AreEqual(newTaskName, taskResult.Name);
        Assert.AreEqual(newTaskDescription, taskResult.Description);
        Assert.AreEqual(newTaskPriority, taskResult.Priority);
        Assert.AreEqual(newTaskDeadlineValue, taskResult.Deadline);
        Assert.AreEqual(isNewTaskComplete, taskResult.IsCompleted);
    }
    [TestMethod]
    [DataRow("Update Task", "Description", Priority.Low, "31/12/2029", true)]
    public void TaskCRUD_UpdateTaskAllPropertiesWhenNotFound_ShouldReturnFalse(string newTaskName, string newTaskDescription, Priority newTaskPriority, string newTaskDeadline, bool isNewTaskComplete)
    {
        string taskName = nameof(TaskCRUD_UpdateTaskAllProperties_ShouldUpdateAllProperties);
        DateTime newTaskDeadlineValue = DateTime.ParseExact(newTaskDeadline, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None);

        Task task = new Task.TaskBuilder(taskName)
            .Build();

        //Act
        task.UpdateName(newTaskName);
        task.UpdateDescription(newTaskDescription);
        task.UpdatePriority(newTaskPriority);
        task.UpdateDeadline(newTaskDeadlineValue);
        if (isNewTaskComplete)
            task.Complete();

        bool updateResult = _taskRepository.UpdateTask(task);

        //Assert
        Assert.IsFalse(updateResult);
        Assert.AreEqual(Task.Default, _taskRepository.GetTaskById(task.Id));
    }
    [TestMethod]
    [DataRow("Update Task", "Description", Priority.Low, "31/12/2029", true)]
    public void TaskCRUD_GetTaskById_ShouldGetTask(string taskName, string taskDescription, Priority taskPriority, string taskDeadline, bool isTaskComplete)
    {
        //Arrange
        DateTime taskDeadlineValue = DateTime.ParseExact(taskDeadline, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None);

        Task task = new Task.TaskBuilder(taskName)
            .SetDescription(taskDescription)
            .SetPriority(taskPriority)
            .SetDeadline(taskDeadlineValue)
            .SetIsCompleted(isTaskComplete)
            .Build();
        _ = _taskRepository.AddTask(task);

        //Act
        Task taskResult = _taskRepository.GetTaskById(task.Id);
        //Assert
        Assert.AreEqual(task.Id, taskResult.Id);
        Assert.AreEqual(task.Name, taskResult.Name);
        Assert.AreEqual(task.Description, taskResult.Description);
        Assert.AreEqual(task.Priority, taskResult.Priority);
        Assert.AreEqual(task.IsCompleted, taskResult.IsCompleted);
        Assert.AreEqual(task.CreationTime, taskResult.CreationTime);
        Assert.AreEqual(task.Deadline, taskResult.Deadline);
    }
    [TestMethod]
    public void TaskCRUD_GetTaskByIdWhenNotFound_ShouldReturnTaskDefault()
    {
        //Arrang
        Guid taskId = Guid.NewGuid();
        //Act
        Task taskResult = _taskRepository.GetTaskById(taskId);
        //Assert
        Assert.AreEqual(Task.Default.Id, taskResult.Id);
        Assert.AreEqual(Task.Default.Name, taskResult.Name);
        Assert.AreEqual(Task.Default.Description, taskResult.Description);
        Assert.AreEqual(Task.Default.Priority, taskResult.Priority);
        Assert.AreEqual(Task.Default.IsCompleted, taskResult.IsCompleted);
        Assert.AreEqual(Task.Default.CreationTime, taskResult.CreationTime);
        Assert.AreEqual(Task.Default.Deadline, taskResult.Deadline);
    }
    [TestMethod]
    public void TaskCRUD_GetAllTask_ShouldReturnAllTasks()
    {
        //Arrange
        string taskName = nameof(TaskCRUD_GetAllTask_ShouldReturnAllTasks);
        Task task1 = new Task.TaskBuilder(taskName)
            .Build();
        Task task2 = new Task.TaskBuilder(taskName + "2")
            .Build();
        Task task3 = new Task.TaskBuilder(taskName + "3")
            .Build();
        _ = _taskRepository.AddTask(task1);
        _ = _taskRepository.AddTask(task2);
        _ = _taskRepository.AddTask(task3);

        //Act
        IEnumerable<Task> tasks = _taskRepository.GetAllTasks();
        //Assert  
        Assert.IsTrue(tasks.Any());
        Assert.IsTrue(tasks.Count() >= 3);
        Assert.IsTrue(tasks.Any(t => t.Id == task1.Id));
        Assert.IsTrue(tasks.Any(t => t.Id == task2.Id));
        Assert.IsTrue(tasks.Any(t => t.Id == task3.Id));
    }
    public static void TaskCompare(Task task, Task task2)
    {
        Assert.AreEqual(task.Id, task2.Id);
        Assert.AreEqual(task.Name, task2.Name);
        Assert.AreEqual(task.Description, task2.Description);
        Assert.AreEqual(task.Priority, task2.Priority);
        Assert.AreEqual(task.Deadline, task2.Deadline);
        Assert.AreEqual(task.IsCompleted, task2.IsCompleted);
    }
}
