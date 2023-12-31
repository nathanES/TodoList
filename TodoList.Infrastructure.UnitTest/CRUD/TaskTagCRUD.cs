﻿using TodoList.Domain.Entities;
using TodoList.Domain.Enum;
using TodoList.Domain.Interfaces;
using TodoList.Infrastructure.Repositories;

namespace TodoList.Infrastructure.UnitTest.CRUD;

[TestClass]
public class TaskTagCRUD
{
  [TestMethod]
  public void GetAllTaskTag()
  {
    //Arrange
    Tag tag = AddTag("GetAllTaskTagNameTag", "GetAllTaskTag", "#000000", "GetAllTaskTagParentNameTag");
    Task task = AddTask("GetAllTaskTagNameTask", "GetAllTaskTag", Priority.High);
    ITaskTagRepository taskTagRepository = new TaskTagRepositoryJson();
    TaskTag taskTag = new(task, tag);
    taskTagRepository.AddTaskTag(taskTag);

    //Act
    List<TaskTag> taskTags = taskTagRepository.GetAllTaskTags().ToList();

    //Assert
    Assert.IsTrue(taskTags.Any(t => t.Id == taskTag.Id && t.TaskId == taskTag.TaskId && t.TagId == taskTag.TagId));
  }

  [TestMethod]
  public void GetTaskTagById()
  {
    //Arrange
    Tag tag = AddTag("GetTaskTagByIdNameTag", "GetTaskTagById", "#000000", "GetTaskTagByIdParentNameTag");
    Task task = AddTask("GetTaskTagByIdNameTask", "GetTaskTagById", Priority.High);
    ITaskTagRepository taskTagRepository = new TaskTagRepositoryJson();
    TaskTag taskTag = new(task, tag);
    taskTagRepository.AddTaskTag(taskTag);

    //Act
    TaskTag taskTagFound = taskTagRepository.GetTaskTagById(taskTag.Id);

    //Assert
    TaskTagCompare(taskTag, taskTagFound);

  }

  [TestMethod]
  public void AddTaskTag()
  {
    //Arrange
    Tag tag = AddTag("AddTaskTagNameTag", "AddTaskTag", "#000000", "AddTaskTagParentNameTag");
    Task task = AddTask("AddTaskTagByIdNameTask", "AddTaskTag", Priority.High);
    ITaskTagRepository taskTagRepository = new TaskTagRepositoryJson();
    TaskTag taskTag = new(task, tag);

    //Act
    taskTagRepository.AddTaskTag(taskTag);

    //Assert
    IEnumerable<TaskTag> taskTagsFound = taskTagRepository.GetAllTaskTags().ToList().Where(t => t.Id == taskTag.Id);
    TaskTagCompare(taskTag, taskTagsFound.ToList()[0]);
  }

  [TestMethod]
  public void UpdateTaskTag()
  {
    //Arrange
    Tag tag = AddTag("UpdateTaskTagNameTag", "UpdateTaskTag", "#000000", "UpdateTaskTagParentNameTag");
    Task task = AddTask("UpdateTaskTagNameTask", "UpdateTaskTagById", Priority.High);
    ITaskTagRepository taskTagRepository = new TaskTagRepositoryJson();
    TaskTag taskTag = new(task, tag);
    taskTagRepository.AddTaskTag(taskTag);
    TagCRUD.TagCompare(tag, taskTag.Tag);
    //Act
    Tag tag2 = AddTag("UpdateTaskTagNameTag2", "UpdateTaskTag2", "#000000", "UpdateTaskTagParentNameTag2");
    taskTag.Tag = tag2;
    taskTagRepository.UpdateTaskTag(taskTag);

    //Assert
    TaskTag taskTagFound = taskTagRepository.GetAllTaskTags().ToList().Where(t => t.Id == taskTag.Id).First();
    TaskTagCompare(taskTag, taskTagFound);
  }
  private void TaskTagCompare(TaskTag taskTag, TaskTag taskTag2)
  {
    Assert.AreEqual(taskTag.Id, taskTag2.Id);
    Assert.AreEqual(taskTag.TagId, taskTag2.TagId);
    Assert.AreEqual(taskTag.TaskId, taskTag2.TaskId);
    TagCRUD.TagCompare(taskTag.Tag, taskTag2.Tag);
    TaskCRUD.TaskCompare(taskTag.Task, taskTag2.Task);
  }
  [TestMethod]
  public void DeleteTaskTag()
  {
    //Arrange
    Tag tag = AddTag("DeleteTaskTagNameTag", "DeleteTaskTag", "#000000", "DeleteTaskTagParentNameTag");
    Task task = AddTask("DeleteTaskTagNameTask", "DeleteTaskTag", Priority.High);
    ITaskTagRepository taskTagRepository = new TaskTagRepositoryJson();
    TaskTag taskTag = new(task, tag);
    taskTagRepository.AddTaskTag(taskTag);

    //Act
    taskTagRepository.DeleteTaskTagById(taskTag.Id);

    //Assert
    Assert.IsFalse(taskTagRepository.GetAllTaskTags().Any(t => t.Id == taskTag.Id));
  }
  [TestMethod]
  public void DeleteTaskTags()
  {
    //Arrange
    ITaskTagRepository taskTagRepository = new TaskTagRepositoryJson();

    Tag tag = AddTag("DeleteTaskTagNameTag", "DeleteTaskTag", "#000000", "DeleteTaskTagParentNameTag");
    Task task = AddTask("DeleteTaskTagNameTask", "DeleteTaskTag", Priority.High);
    TaskTag taskTag = new(task, tag);
    taskTagRepository.AddTaskTag(taskTag);

    Tag tag2 = AddTag("DeleteTaskTagNameTag2", "DeleteTaskTag2", "#000000", "DeleteTaskTag2ParentNameTag");
    Task task2 = AddTask("DeleteTaskTagNameTask2", "DeleteTaskTag2", Priority.High);
    TaskTag taskTag2 = new(task2, tag2);
    taskTagRepository.AddTaskTag(taskTag2);

    //Act
    taskTagRepository.DeleteTaskTagByIds(new List<string>() { taskTag.Id, taskTag2.Id });

    //Assert
    Assert.IsFalse(taskTagRepository.GetAllTaskTags().Any(t => t.Id == taskTag.Id));
    Assert.IsFalse(taskTagRepository.GetAllTaskTags().Any(t => t.Id == taskTag2.Id));
  }

  [TestMethod]
  public void GetTaskTagsByTaskId()
  {
    //Arrange
    Tag tag = AddTag("GetTaskTagsByTaskIdNameTag", "GetTaskTagsByTaskId", "#000000", "GetTaskTagsByTaskIdParentNameTag");
    Task task = AddTask("GetTaskTagsByTaskIdNameTask", "GetTaskTagsByTaskId", Priority.High);
    ITaskTagRepository taskTagRepository = new TaskTagRepositoryJson();
    TaskTag taskTag = new(task, tag);
    taskTagRepository.AddTaskTag(taskTag);
    //Act
    IEnumerable<TaskTag> taskTags = taskTagRepository.GetTaskTagsByTaskId(task.Id);
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
    ITaskTagRepository taskTagRepository = new TaskTagRepositoryJson();
    TaskTag taskTag = new(task, tag);
    taskTagRepository.AddTaskTag(taskTag);
    //Act
    IEnumerable<TaskTag> taskTags = taskTagRepository.GetTaskTagsByTagId(tag.Id);
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
    ITaskTagRepository taskTagRepository = new TaskTagRepositoryJson();
    taskTagRepository.AddTaskTag(new TaskTag(task, tag));
    //Act
    bool isRelationExists = taskTagRepository.IsRelationExists(task.Id, tag.Id);

    //Assert
    Assert.IsTrue(isRelationExists);
  }
  [TestMethod]
  public void IsRelationExistsFalse()
  {
    //Arrange
    Tag tag = AddTag("IsRelationExistsFalseNameTag", "IsRelationExistsFalse", "#000000", "IsRelationExistsFalseParentNameTag");
    Task task = AddTask("IsRelationExistsFalseNameTask", "IsRelationExistsFalse", Priority.High);
    ITaskTagRepository taskTagRepository = new TaskTagRepositoryJson();
    //Act
    bool isRelationExists = taskTagRepository.IsRelationExists(task.Id, tag.Id);

    //Assert
    Assert.IsFalse(isRelationExists);
  }

  //Méthodes pour créer les objets nécessaire aux tests
  private Tag AddTag(string name, string description, string color, string parentName)
  {
    ITagRepository tagRepository = new TagRepositoryJson();
    Tag tagParent = new Tag.TagBuilder()
        .SetName(parentName)
        .Build();
    tagRepository.AddTag(tagParent);
    Assert.IsTrue(tagRepository.GetAllTags().Any(t => t.Id == tagParent.Id));

    Tag tag = new Tag.TagBuilder()
        .SetName(name)
        .SetDescription(description)
        .SetColor(new Domain.ValueObjects.Color(color))
        .SetParentTagIds(new List<string>() { tagParent.Id })
        .Build();
    //Act
    tagRepository.AddTag(tag);
    //Assert
    Assert.IsTrue(tagRepository.GetAllTags().Any(t => t.Id == tag.Id));
    return tag;
  }
  private Task AddTask(string name, string description, Priority priority)
  {
    //Arrange
    ITaskRepository taskRepository = new TaskRepositoryJson();

    Task task = new Task.TaskBuilder()
        .SetName(name)
        .SetDescription(description)
        .SetPriority(priority)
        .Build();
    //Act
    taskRepository.AddTask(task);
    //Assert
    Assert.IsTrue(taskRepository.GetAllTasks().Any(t => t.Id == task.Id));
    return task;
  }
}
