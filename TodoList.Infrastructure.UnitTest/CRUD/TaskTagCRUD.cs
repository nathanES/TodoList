using Moq;
using System.Globalization;
using TodoList.Domain.Entities;
using TodoList.Domain.Enum;
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
    //TODO faire la refonte des tests
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
        string tagName = nameof(TaskTagCRUD_GetTaskTagByIdWhenNotFound_ShouldGetTaskTagDefauld);
        string taskName = nameof(TaskTagCRUD_GetTaskTagByIdWhenNotFound_ShouldGetTaskTagDefauld);
        TaskTag taskTag = CreateBasicTaskTag(taskName, tagName);
        _ = _taskTagRepository.AddTaskTag(taskTag);

        //Act
        TaskTag taskTagFound = _taskTagRepository.GetTaskTagById(taskTag.Id);

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
        _ = _taskTagRepository.AddTaskTag(taskTag);

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
        string tagName = nameof(TaskTagCRUD_DeleteTaskTag_ShouldDeleteTaskTag);
        string taskName = nameof(TaskTagCRUD_DeleteTaskTag_ShouldDeleteTaskTag);
        TaskTag taskTag = CreateBasicTaskTag(taskName, tagName);

        //Act
        bool resultUpdateTaskTag = _taskTagRepository.DeleteTaskTagById(taskTag.Id);

        //Assert
        Assert.IsFalse(resultUpdateTaskTag);
        Assert.AreEqual(TaskTag.Default, _taskTagRepository.GetTaskTagById(taskTag.Id));
    }

    [TestMethod]
    public void DeleteTaskTags()
    {
        //Arrange
        Tag tag = AddTag("DeleteTaskTagNameTag", "DeleteTaskTag", "#000000", "DeleteTaskTagParentNameTag");
        Task task = AddTask("DeleteTaskTagNameTask", "DeleteTaskTag", Priority.High);
        TaskTag taskTag = new TaskTagBuilder(task, tag).Build();
        _ = _taskTagRepository.AddTaskTag(taskTag);

        Tag tag2 = AddTag("DeleteTaskTagNameTag2", "DeleteTaskTag2", "#000000", "DeleteTaskTag2ParentNameTag");
        Task task2 = AddTask("DeleteTaskTagNameTask2", "DeleteTaskTag2", Priority.High);
        TaskTag taskTag2 = new TaskTagBuilder(task2, tag2).Build();
        _ = _taskTagRepository.AddTaskTag(taskTag2);

        //Act
        _ = _taskTagRepository.DeleteTaskTagByIds(new List<Guid>() { taskTag.Id, taskTag2.Id });

        //Assert
        Assert.IsFalse(_taskTagRepository.GetAllTaskTags().Any(t => t.Id == taskTag.Id));
        Assert.IsFalse(_taskTagRepository.GetAllTaskTags().Any(t => t.Id == taskTag2.Id));
    }

    [TestMethod]
    public void GetTaskTagsByTaskId()
    {
        //Arrange
        Tag tag = AddTag("GetTaskTagsByTaskIdNameTag", "GetTaskTagsByTaskId", "#000000", "GetTaskTagsByTaskIdParentNameTag");
        Task task = AddTask("GetTaskTagsByTaskIdNameTask", "GetTaskTagsByTaskId", Priority.High);

        TaskTag taskTag = new TaskTagBuilder(task, tag).Build();
        _ = _taskTagRepository.AddTaskTag(taskTag);
        //Act
        IEnumerable<TaskTag> taskTags = _taskTagRepository.GetTaskTagsByTaskId(task.Id);
        //Assert
        Assert.IsTrue(taskTags.Any(t => t.TaskId == task.Id));
        Assert.IsFalse(taskTags.Any(t => t.TaskId != task.Id));

    }

    [TestMethod]
    public void GetTaskTagsByTagId()
    {
        //Arrange
        Tag tag = AddTag("GetTaskTagsByTagIdNameTag", "GetTaskTagsByTagId", "#000000", "GetTaskTagsByTagIdParentNameTag");
        Task task = AddTask("GetTaskTagsByTagIdNameTask", "GetTaskTagsByTagId", Priority.High);

        TaskTag taskTag = new TaskTagBuilder(task, tag).Build();
        _ = _taskTagRepository.AddTaskTag(taskTag);
        //Act
        IEnumerable<TaskTag> taskTags = _taskTagRepository.GetTaskTagsByTagId(tag.Id);
        //Assert
        Assert.IsTrue(taskTags.Any(t => t.TagId == tag.Id));
        Assert.IsFalse(taskTags.Any(t => t.TagId != tag.Id));
    }

    [TestMethod]
    public void IsRelationExistsTrue()
    {
        //Arrange
        Tag tag = AddTag("IsRelationExistsTrueNameTag", "IsRelationExistsTrue", "#000000", "IsRelationExistsTrueParentNameTag");
        Task task = AddTask("IsRelationExistsTrueNameTask", "IsRelationExistsTrue", Priority.High);

        _ = _taskTagRepository.AddTaskTag(new TaskTagBuilder(task, tag).Build());
        //Act
        bool isRelationExists = _taskTagRepository.IsRelationExists(task.Id, tag.Id);

        //Assert
        Assert.IsTrue(isRelationExists);
    }
    [TestMethod]
    public void IsRelationExistsFalse()
    {
        //Arrange
        Tag tag = AddTag("IsRelationExistsFalseNameTag", "IsRelationExistsFalse", "#000000", "IsRelationExistsFalseParentNameTag");
        Task task = AddTask("IsRelationExistsFalseNameTask", "IsRelationExistsFalse", Priority.High);

        //Act
        bool isRelationExists = _taskTagRepository.IsRelationExists(task.Id, tag.Id);

        //Assert
        Assert.IsFalse(isRelationExists);
    }
}
