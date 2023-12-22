using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TodoList.Domain.Entities;
using TodoList.Domain.Enum;
using TodoList.Domain.Helpers;
using TodoList.Domain.Interfaces;
using TodoList.Infrastructure.Repositories;

namespace TodoList.Infrastructure.UnitTest
{
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
      TaskTag taskTag = new TaskTag() {Task = task, Tag = tag };
      taskTagRepository.AddTaskTag(taskTag);

      //Act
      List<TaskTag> taskTags = taskTagRepository.GetAllTaskTags().ToList();

      //Assert
      Assert.IsTrue(taskTags.Any(t => t.Id == taskTag.Id && t.TaskId == task.Id && t.TagId == tag.Id));
    }

    [TestMethod]
    public void GetTaskTagById()
    {
      //Arrange
      Tag tag = AddTag("GetTaskTagByIdNameTag", "GetTaskTagById", "#000000", "GetTaskTagByIdParentNameTag");
      Task task = AddTask("GetTaskTagByIdNameTask", "GetTaskTagById", Priority.High);
      ITaskTagRepository taskTagRepository = new TaskTagRepositoryJson();
      TaskTag taskTag = new TaskTag() { Task = task, Tag = tag };
      taskTagRepository.AddTaskTag(taskTag);

      //Act
      TaskTag taskTagFound = taskTagRepository.GetTaskTagById(taskTag.Id);

      //Assert
      Assert.IsTrue(ObjectHelper.AreObjectsEqual(taskTag, taskTagFound));
    }

    [TestMethod]
    public void AddTaskTag()
    {
      //Arrange
      Tag tag = AddTag("AddTaskTagNameTag", "AddTaskTag", "#000000", "AddTaskTagParentNameTag");
      Task task = AddTask("AddTaskTagByIdNameTask", "AddTaskTag", Priority.High);
      ITaskTagRepository taskTagRepository = new TaskTagRepositoryJson();
      TaskTag taskTag = new TaskTag() { Task = task, Tag = tag };

      //Act
      taskTagRepository.AddTaskTag(taskTag);

      //Assert
      TaskTag taskTagFound = taskTagRepository.GetAllTaskTags().ToList().Where(t => t.Id == taskTag.Id).First();
      Assert.IsTrue(ObjectHelper.AreObjectsEqual(taskTag, taskTagFound));
    }

    [TestMethod]
    public void UpdateTaskTag()
    {
      //Arrange
      Tag tag = AddTag("UpdateTaskTagNameTag", "UpdateTaskTag", "#000000", "UpdateTaskTagParentNameTag");
      Task task = AddTask("UpdateTaskTagNameTask", "UpdateTaskTagById", Priority.High);
      ITaskTagRepository taskTagRepository = new TaskTagRepositoryJson();
      TaskTag taskTag = new TaskTag() { Task = task, Tag = tag };
      taskTagRepository.AddTaskTag(taskTag);

      //Act
      Tag tag2 = AddTag("UpdateTaskTagNameTag2", "UpdateTaskTag2", "#000000", "UpdateTaskTagParentNameTag2");
      taskTag.Tag = tag2;
      taskTagRepository.UpdateTaskTag(taskTag);

      //Assert
      TaskTag taskTagFound = taskTagRepository.GetAllTaskTags().ToList().Where(t => t.Id == taskTag.Id).First();
      Assert.IsTrue(ObjectHelper.AreObjectsEqual(taskTag, taskTagFound));
    }
    [TestMethod]
    public void DeleteTaskTag()
    {
      //Arrange
      Tag tag = AddTag("DeleteTaskTagNameTag", "DeleteTaskTag", "#000000", "DeleteTaskTagParentNameTag");
      Task task = AddTask("DeleteTaskTagNameTask", "DeleteTaskTag", Priority.High);
      ITaskTagRepository taskTagRepository = new TaskTagRepositoryJson();
      TaskTag taskTag = new TaskTag() { Task = task, Tag = tag };
      taskTagRepository.AddTaskTag(taskTag);

      //Act
      taskTagRepository.DeleteTaskTag(taskTag);

      //Assert
      Assert.IsFalse(taskTagRepository.GetAllTaskTags().Any(t => t.Id == taskTag.Id));
    }
    [TestMethod]
    public void GetTaskTagsByTaskId()
    {
      //Arrange
      Tag tag = AddTag("GetTaskTagsByTaskIdNameTag", "GetTaskTagsByTaskId", "#000000", "GetTaskTagsByTaskIdParentNameTag");
      Task task = AddTask("GetTaskTagsByTaskIdNameTask", "GetTaskTagsByTaskId", Priority.High);
      ITaskTagRepository taskTagRepository = new TaskTagRepositoryJson();
      TaskTag taskTag = new TaskTag() { Task = task, Tag = tag };
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
      TaskTag taskTag = new TaskTag() { Task = task, Tag = tag };
      taskTagRepository.AddTaskTag(taskTag);
      //Act
      IEnumerable<TaskTag> taskTags = taskTagRepository.GetTaskTagsByTagId(tag.Id);
      //Assert
      Assert.IsTrue(taskTags.Any(t => t.TagId == tag.Id));
      Assert.IsFalse(taskTags.Any(t => t.TagId != tag.Id));
    }

    [TestMethod]
    public void IsRelationExists()
    {
      //TODO a finir
    }
    //class TaskTag
    //public string Id { get; } = Guid.NewGuid().ToString();
    //public string TaskId { get; set; }
    //public Task Task { get; set; }

    //public string TagId { get; set; }
    //public Tag Tag { get; set; }






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
}
