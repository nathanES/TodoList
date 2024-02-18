using Moq;
using System.Globalization;
using TodoList.Domain.Entities;
using TodoList.Domain.Enum;
using TodoList.Domain.Exceptions;
using TodoList.Domain.Interfaces.Logger;
using TodoList.Domain.Interfaces.Repositories;
using TodoList.Infrastructure.Loggers;
using TodoList.Infrastructure.Repositories;
using static TodoList.Domain.Entities.Tag;
using static TodoList.Domain.Entities.Task;
using static TodoList.Domain.Entities.TaskTag;

namespace TodoList.Infrastructure.UnitTest.CRUD;

[TestClass]
public class TaskTagCRUD
{

    private ITaskTagRepository? _taskTagRepository;
    private ILogger? logger;
    private readonly Mock<ILogDestination> logDestination = new();

    private TaskTag CreateBasicTaskTag(string taskName, string tagName)
    {
        Tag tag = new TagBuilder(tagName).Build();
        Task task = new TaskBuilder(name: taskName).Build();
        return new TaskTagBuilder(task, tag).Build();
    }
    [TestInitialize]
    public void TagCRUDInitialize()
    {
        logger = new LoggerCustom(logDestination.Object);
        _taskTagRepository = new TaskTagRepositoryJson(logger);
    }
    [TestMethod]
    public void TaskTagCRUD_GetAllTaskTag_ShouldGetAllTaskTag()
    {
        //Arrange
        string tagName = nameof(TaskTagCRUD_GetAllTaskTag_ShouldGetAllTaskTag);
        string taskName = nameof(TaskTagCRUD_GetAllTaskTag_ShouldGetAllTaskTag);
        TaskTag taskTag1 = CreateBasicTaskTag(taskName, tagName);
        TaskTag taskTag2 = CreateBasicTaskTag(taskName, tagName);

        _ = _taskTagRepository.AddTaskTag(taskTag1);
        _ = _taskTagRepository.AddTaskTag(taskTag2);

        //Act
        IEnumerable<TaskTag> taskTags = _taskTagRepository.GetAllTaskTags();

        //Assert
        Assert.IsTrue(taskTags.Any(t => t.Id == taskTag1.Id && t.TaskId == taskTag1.TaskId && t.TagId == taskTag1.TagId));
        Assert.IsTrue(taskTags.Any(t => t.Id == taskTag2.Id && t.TaskId == taskTag2.TaskId && t.TagId == taskTag2.TagId));

    }

    [TestMethod]
    public void TaskTagCRUD_GetTaskTagById_ShouldGetTaskTag()
    {
        //Arrange
        string tagName = nameof(TaskTagCRUD_GetTaskTagById_ShouldGetTaskTag);
        string taskName = nameof(TaskTagCRUD_GetTaskTagById_ShouldGetTaskTag);
        TaskTag taskTag = CreateBasicTaskTag(taskName, tagName);
        _ = _taskTagRepository.AddTaskTag(taskTag);

        //Act
        TaskTag taskTagFound = _taskTagRepository.GetTaskTagById(taskTag.Id);

        //Assert
        Assert.AreEqual(taskTag.Id, taskTagFound.Id);
        Assert.AreEqual(taskTag.TagId, taskTagFound.TagId);
        Assert.AreEqual(taskTag.TaskId, taskTagFound.TaskId);
        Assert.IsTrue(taskTag.Tag.Equals(taskTagFound.Tag));
        Assert.IsTrue(taskTag.Task.Equals(taskTagFound.Task));
    }
    [TestMethod]
    public void TaskTagCRUD_GetTaskTagByIdWhenNotFound_ShouldGetTaskTagDefauld()
    {
        //Arrange
        Guid taskTagId = Guid.NewGuid();

        //Act
        TaskTag taskTagFound = _taskTagRepository.GetTaskTagById(taskTagId);

        //Assert
        Assert.AreEqual(TaskTag.Default, taskTagFound);
    }

    [TestMethod]
    public void TaskTagCRUD_AddTaskTag_ShouldAddTaskTag()
    {
        //Arrange
        string tagName = nameof(TaskTagCRUD_AddTaskTag_ShouldAddTaskTag);
        string taskName = nameof(TaskTagCRUD_AddTaskTag_ShouldAddTaskTag);
        TaskTag taskTag = CreateBasicTaskTag(taskName, tagName);
        //Act
        _ = _taskTagRepository.AddTaskTag(taskTag);

        //Assert
        TaskTag taskTagFound = _taskTagRepository.GetTaskTagById(taskTag.Id);
        Assert.AreEqual(taskTag.Id, taskTagFound.Id);
        Assert.AreEqual(taskTag.TagId, taskTagFound.TagId);
        Assert.AreEqual(taskTag.TaskId, taskTagFound.TaskId);
        Assert.IsTrue(taskTag.Tag.Equals(taskTagFound.Tag));
        Assert.IsTrue(taskTag.Task.Equals(taskTagFound.Task));
    }

    [TestMethod]
    [ExpectedException(typeof(DuplicateKeyException))]
    public void TaskTagCRUD_AddTaskTagDuplicateKey_ShouldReturnDuplicateKeyException()
    {
        //Arrange
        string tagName = nameof(TaskTagCRUD_AddTaskTagDuplicateKey_ShouldReturnDuplicateKeyException);
        string taskName = nameof(TaskTagCRUD_AddTaskTagDuplicateKey_ShouldReturnDuplicateKeyException);
        TaskTag taskTag = CreateBasicTaskTag(taskName, tagName);
        //Act
        _ = _taskTagRepository.AddTaskTag(taskTag);
        _ = _taskTagRepository.AddTaskTag(taskTag);
    }

    [TestMethod]
    [DataRow("NouveauNom", "NouvelleDescription", "#000000")]
    public void TaskTagCRUD_UpdateTaskTagTag_ShouldUpdateTaskTagTag(string newTagName, string newTagDescription, string newTagColor)
    {
        //Arrange
        string tagName = nameof(TaskTagCRUD_UpdateTaskTagTag_ShouldUpdateTaskTagTag);
        string taskName = nameof(TaskTagCRUD_UpdateTaskTagTag_ShouldUpdateTaskTagTag);
        TaskTag taskTag = CreateBasicTaskTag(taskName, tagName);
        _ = _taskTagRepository.AddTaskTag(taskTag);

        //Act
        Color newTagColorValue = new(newTagColor);
        Tag newTag = new TagBuilder(newTagName)
            .SetDescription(newTagDescription)
            .SetColor(newTagColorValue)
            .Build();

        taskTag.UpdateTag(newTag);
        bool updateTaskTagResult = _taskTagRepository.UpdateTaskTag(taskTag);

        //Assert
        Assert.IsTrue(updateTaskTagResult);
        TaskTag taskTagFound = _taskTagRepository.GetTaskTagById(taskTag.Id);
        Assert.AreEqual(taskTag.Id, taskTagFound.Id);
        Assert.AreEqual(taskTag.TagId, taskTagFound.TagId);
        Assert.AreEqual(taskTag.TaskId, taskTagFound.TaskId);
        Assert.IsTrue(taskTag.Tag.Equals(taskTagFound.Tag));
        Assert.IsTrue(taskTag.Task.Equals(taskTagFound.Task));
    }
    [TestMethod]
    [DataRow("NouveauNom", "NouvelleDescription", "#000000")]
    public void TaskTagCRUD_UpdateTaskTagTagWhenNotExisting_ShouldReturnFalse(string newTagName, string newTagDescription, string newTagColor)
    {
        //Arrange
        string tagName = nameof(TaskTagCRUD_UpdateTaskTagTagWhenNotExisting_ShouldReturnFalse);
        string taskName = nameof(TaskTagCRUD_UpdateTaskTagTagWhenNotExisting_ShouldReturnFalse);
        TaskTag taskTag = CreateBasicTaskTag(taskName, tagName);

        //Act
        Color newTagColorValue = new(newTagColor);
        Tag newTag = new TagBuilder(newTagName)
            .SetDescription(newTagDescription)
            .SetColor(newTagColorValue)
            .Build();

        taskTag.UpdateTag(newTag);
        bool updateTaskTagResult = _taskTagRepository.UpdateTaskTag(taskTag);

        //Assert
        Assert.IsFalse(updateTaskTagResult);
        TaskTag taskTagFound = _taskTagRepository.GetTaskTagById(taskTag.Id);
        Assert.AreEqual(TaskTag.Default, taskTagFound);
    }
    [TestMethod]
    [DataRow("NouveauNom", "NouvelleDescription", true, Priority.High, "12/06/2025")]
    public void TaskTagCRUD_UpdateTaskTagTask_ShouldUpdateTaskTagTask(string newTaskName, string newTaskDescription, bool isnewTaskCompled, Priority newTaskPriority, string newTaskDeadline)
    {
        //Arrange
        string tagName = nameof(TaskTagCRUD_UpdateTaskTagTask_ShouldUpdateTaskTagTask);
        string taskName = nameof(TaskTagCRUD_UpdateTaskTagTask_ShouldUpdateTaskTagTask);
        TaskTag taskTag = CreateBasicTaskTag(taskName, tagName);
        _ = _taskTagRepository.AddTaskTag(taskTag);

        //Act
        DateTime newTaskDeadlineValue = DateTime.ParseExact(newTaskDeadline, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None);

        Task newTask = new TaskBuilder(newTaskName)
        .SetDescription(newTaskDescription)
        .SetIsCompleted(isnewTaskCompled)
        .SetPriority(newTaskPriority)
        .SetDeadline(newTaskDeadlineValue)
        .Build();

        taskTag.UpdateTask(newTask);
        bool updateTaskTagResult = _taskTagRepository.UpdateTaskTag(taskTag);

        //Assert
        Assert.IsTrue(updateTaskTagResult);
        TaskTag taskTagFound = _taskTagRepository.GetTaskTagById(taskTag.Id);
        Assert.AreEqual(taskTag.Id, taskTagFound.Id);
        Assert.AreEqual(taskTag.TagId, taskTagFound.TagId);
        Assert.AreEqual(taskTag.TaskId, taskTagFound.TaskId);
        Assert.IsTrue(taskTag.Tag.Equals(taskTagFound.Tag));
        Assert.IsTrue(taskTag.Task.Equals(taskTagFound.Task));
    }

    [TestMethod]
    [DataRow("NouveauNom", "NouvelleDescription", true, Priority.High, "12/06/2025")]
    public void TaskTagCRUD_UpdateTaskTagTaskWhenNotExisting_ShouldUpdateTaskTagTask(string newTaskName, string newTaskDescription, bool isnewTaskCompled, Priority newTaskPriority, string newTaskDeadline)
    {
        //Arrange
        string tagName = nameof(TaskTagCRUD_UpdateTaskTagTaskWhenNotExisting_ShouldUpdateTaskTagTask);
        string taskName = nameof(TaskTagCRUD_UpdateTaskTagTaskWhenNotExisting_ShouldUpdateTaskTagTask);
        TaskTag taskTag = CreateBasicTaskTag(taskName, tagName);

        //Act
        DateTime newTaskDeadlineValue = DateTime.ParseExact(newTaskDeadline, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None);

        Task newTask = new TaskBuilder(newTaskName)
        .SetDescription(newTaskDescription)
        .SetIsCompleted(isnewTaskCompled)
        .SetPriority(newTaskPriority)
        .SetDeadline(newTaskDeadlineValue)
        .Build();

        taskTag.UpdateTask(newTask);
        bool updateTaskTagResult = _taskTagRepository.UpdateTaskTag(taskTag);

        //Assert
        Assert.IsFalse(updateTaskTagResult);
        TaskTag taskTagFound = _taskTagRepository.GetTaskTagById(taskTag.Id);
        Assert.AreEqual(TaskTag.Default, taskTagFound);
    }

    [TestMethod]
    public void TaskTagCRUD_DeleteTaskTag_ShouldDeleteTaskTag()
    {
        //Arrange
        string tagName = nameof(TaskTagCRUD_DeleteTaskTag_ShouldDeleteTaskTag);
        string taskName = nameof(TaskTagCRUD_DeleteTaskTag_ShouldDeleteTaskTag);
        TaskTag taskTag = CreateBasicTaskTag(taskName, tagName);
        _ = _taskTagRepository.AddTaskTag(taskTag);

        //Act
        bool resultUpdateTaskTag = _taskTagRepository.DeleteTaskTagById(taskTag.Id);

        //Assert
        Assert.IsTrue(resultUpdateTaskTag);
        Assert.AreEqual(TaskTag.Default, _taskTagRepository.GetTaskTagById(taskTag.Id));
    }
    [TestMethod]
    public void TaskTagCRUD_DeleteTaskTagWhenNotExisting_ShouldReturnFalse()
    {
        //Arrange
        Guid taskTagId = Guid.NewGuid();

        //Act
        bool resultUpdateTaskTag = _taskTagRepository.DeleteTaskTagById(taskTagId);

        //Assert
        Assert.IsFalse(resultUpdateTaskTag);
        Assert.AreEqual(TaskTag.Default, _taskTagRepository.GetTaskTagById(taskTagId));
    }

    [TestMethod]
    public void TaskTagCRUD_DeleteTaskTags_ShouldDeleteTaskTags()
    {
        //Arrange
        string tagName = nameof(TaskTagCRUD_DeleteTaskTags_ShouldDeleteTaskTags);
        string taskName = nameof(TaskTagCRUD_DeleteTaskTags_ShouldDeleteTaskTags);
        TaskTag taskTag = CreateBasicTaskTag(taskName, tagName);
        _ = _taskTagRepository.AddTaskTag(taskTag);

        TaskTag taskTag2 = CreateBasicTaskTag(taskName + "2", tagName + "2");
        _ = _taskTagRepository.AddTaskTag(taskTag2);

        //Act
        bool resultDeleteTaskTags = _taskTagRepository.DeleteTaskTagByIds(new List<Guid>() { taskTag.Id, taskTag2.Id });

        //Assert
        Assert.IsTrue(resultDeleteTaskTags);
        Assert.AreEqual(TaskTag.Default, _taskTagRepository.GetTaskTagById(taskTag.Id));
        Assert.AreEqual(TaskTag.Default, _taskTagRepository.GetTaskTagById(taskTag2.Id));
    }
    [TestMethod]
    public void TaskTagCRUD_DeleteTaskTagsWhenNotExisting_ShouldReturnFalse()
    {
        //Arrange
        Guid taskTagId = Guid.NewGuid();
        Guid taskTagId2 = Guid.NewGuid();
        //Act
        bool resultDeleteTaskTags = _taskTagRepository.DeleteTaskTagByIds(new List<Guid>() { taskTagId, taskTagId2 });

        //Assert
        Assert.IsFalse(resultDeleteTaskTags);
        Assert.AreEqual(TaskTag.Default, _taskTagRepository.GetTaskTagById(taskTagId));
        Assert.AreEqual(TaskTag.Default, _taskTagRepository.GetTaskTagById(taskTagId2));
    }
    [TestMethod]
    public void TaskTagCRUD_GetTaskTagByTaskID_ShouldGetTaskTag()
    {
        //Arrange
        string tagName = nameof(TaskTagCRUD_GetTaskTagByTaskID_ShouldGetTaskTag);
        string taskName = nameof(TaskTagCRUD_GetTaskTagByTaskID_ShouldGetTaskTag);
        TaskTag taskTag = CreateBasicTaskTag(taskName, tagName);
        _ = _taskTagRepository.AddTaskTag(taskTag);

        TaskTag taskTag2 = CreateBasicTaskTag(taskName + "2", tagName + "2");
        taskTag2.UpdateTag(taskTag.Tag);
        taskTag2.UpdateTask(taskTag.Task);

        _ = _taskTagRepository.AddTaskTag(taskTag2);
        //Act
        IEnumerable<TaskTag> taskTags = _taskTagRepository.GetTaskTagsByTaskId(taskTag.TaskId);
        //Assert
        Assert.IsTrue(taskTags.Count() >= 2);
        Assert.IsTrue(taskTags.Any(t => t.TaskId == taskTag.TaskId));
        Assert.IsFalse(taskTags.Any(t => t.TaskId != taskTag.TaskId));

    }
    [TestMethod]
    public void TaskTagCRUD_GetTaskTagByTaskIDWhenNotExisting_ShouldReturnEmptyListTaskTag()
    {
        //Arrange
        Guid taskId = Guid.NewGuid();
        //Act
        IEnumerable<TaskTag> taskTags = _taskTagRepository.GetTaskTagsByTaskId(taskId);
        //Assert
        Assert.IsFalse(taskTags.Any());

    }
    [TestMethod]
    public void TaskTagCRUD_GetTaskTagByTagID_ShouldGetTaskTagByTagID()
    {
        //Arrange
        string tagName = nameof(TaskTagCRUD_GetTaskTagByTagID_ShouldGetTaskTagByTagID);
        string taskName = nameof(TaskTagCRUD_GetTaskTagByTagID_ShouldGetTaskTagByTagID);
        TaskTag taskTag = CreateBasicTaskTag(taskName, tagName);
        _ = _taskTagRepository.AddTaskTag(taskTag);

        TaskTag taskTag2 = CreateBasicTaskTag(taskName + "2", tagName + "2");
        taskTag2.UpdateTag(taskTag.Tag);
        taskTag2.UpdateTask(taskTag.Task);
        _ = _taskTagRepository.AddTaskTag(taskTag2);
        //Act
        IEnumerable<TaskTag> taskTags = _taskTagRepository.GetTaskTagsByTagId(taskTag.TagId);
        //Assert
        Assert.IsTrue(taskTags.Count() >= 2);
        Assert.IsTrue(taskTags.Any(t => t.TagId == taskTag.TagId));
        Assert.IsFalse(taskTags.Any(t => t.TagId != taskTag.TagId));
    }

    [TestMethod]
    public void TaskTagCRUD_GetTaskTagByTagIDWhenNotExisting_ShouldReturnEmptyListTaskTag()
    {
        //Arrange
        Guid tagId = Guid.NewGuid();
        //Act
        IEnumerable<TaskTag> taskTags = _taskTagRepository.GetTaskTagsByTagId(tagId);
        //Assert
        Assert.IsFalse(taskTags.Any());
    }

    [TestMethod]
    public void TaskTagCRUD_IsRelationExist_ShouldReturnTrue()
    {
        //Arrange
        string tagName = nameof(TaskTagCRUD_GetTaskTagByTagID_ShouldGetTaskTagByTagID);
        string taskName = nameof(TaskTagCRUD_GetTaskTagByTagID_ShouldGetTaskTagByTagID);
        TaskTag taskTag = CreateBasicTaskTag(taskName, tagName);
        _ = _taskTagRepository.AddTaskTag(taskTag);

        //Act
        bool isRelationExists = _taskTagRepository.IsRelationExists(taskTag.TaskId, taskTag.TagId);

        //Assert
        Assert.IsTrue(isRelationExists);
    }
    [TestMethod]
    public void TaskTagCRUD_IsRelationExist_ShouldReturnFalse()
    {
        //Arrange
        Guid taskId = Guid.NewGuid();
        Guid tagId = Guid.NewGuid();

        //Act
        bool isRelationExists = _taskTagRepository.IsRelationExists(taskId, tagId);

        //Assert
        Assert.IsFalse(isRelationExists);
    }
}
