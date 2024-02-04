using TodoList.Domain.Entities;
using TodoList.Domain.Interfaces.Repositories;
using static TodoList.Domain.Entities.TaskTag;

namespace TodoList.Domain.Factories;

public static class TaskTagFactory
{
    public static TaskTag CreateTaskTag(Task task, Tag tag)
    {
        return new TaskTagBuilder(task, tag).Build();
    }
    public static TaskTag CreateTaskTagWithValidation(Task task, Tag tag, ITaskTagRepository taskTagRepository)
    {
        if (taskTagRepository.IsRelationExists(task.Id, tag.Id))
            throw new Exception("Task already has this tag");

        return new TaskTagBuilder(task, tag).Build();
    }
    //on pourrait aussi rajouter une méthode qui prend en paramètre des paramètres optionnels
}
