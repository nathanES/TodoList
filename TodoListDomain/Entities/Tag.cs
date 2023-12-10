using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoList.Domain.Enum;
using TodoList.Domain.ValueObjects;

namespace TodoList.Domain.Entities
{
  public class Tag
  {
    public string Id { get; } = Guid.NewGuid().ToString();
    private string name;
    public string Name
    {
      get { return name; }
      private set {
        ArgumentException.ThrowIfNullOrEmpty(value, nameof(name));
        name = value; 
      }
    }
    public string? Description { get; private set; }
    public Color Color { get; private set; } = new Color("#000000");
    public Tag? TagParent { get; private set; }
    //public List<Task> AssociatedTask { get; private set; } = new List<Task>();

    private Tag(string name)
    {
      Name = name;
    }
    public void UpdateName(string name)
    {
      Name = name;
    }
    public void UpdateDescription(string description)
    {
      Description = description;
    }
    public void UpdateColor(Color color)
    {
      Color = color;
    }
    public void UpdateTagParent(Tag tagParent)
    {
      TagParent = tagParent;
    }
    //public void AddAssociatedTask(Task task)
    //{
    //  AssociatedTask.Add(task);
    //}

    public class TagBuilder
    {
      private string name;
      private string description;
      private Color color = new Color("#000000");
      private Tag? tagParent;
      //private List<Task> associatedTask = new List<Task>();

      public TagBuilder SetName(string name)
      {
        this.name = name;
        return this;
      }

      public TagBuilder SetDescription(string description)
      {
        this.description = description;
        return this;
      }

      public TagBuilder SetColor(Color color)
      {
        this.color = color;
        return this;
      }

      public TagBuilder SetTagParent(Tag tagParent)
      {
        this.tagParent = tagParent;
        return this;
      }

      //public TagBuilder SetAssociatedTask(List<Task> associatedTask)
      //{
      //  this.associatedTask = associatedTask;
      //  return this;
      //}
      public Tag Build()
      {
        return new Tag(name)
        {
          Description = description,
          Color = color,
          TagParent = tagParent,
          //AssociatedTask = associatedTask
        };
      }
    }
  }
}
