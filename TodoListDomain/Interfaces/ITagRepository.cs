﻿using TodoList.Domain.Entities;

namespace TodoList.Domain.Interfaces
{
  public interface ITagRepository
  {
      IEnumerable<Tag> GetAllTags();
      Tag GetTagById(string id);
      void AddTag(Tag tag);
      void UpdateTag(Tag tag);
      void DeleteTag(Tag tag);
  }
}
