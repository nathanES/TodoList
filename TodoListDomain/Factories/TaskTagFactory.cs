using TodoList.Domain.Entities;
using TodoList.Domain.Interfaces.Repositories;

namespace TodoList.Domain.Factories;

public static class TaskTagFactory
{
    public static TaskTag CreateTaskTag(Task task, Tag tag)
    {
        return new TaskTag(task, tag);
    }
    public static TaskTag CreateTaskTagWithValidation(Task task, Tag tag, ITaskTagRepository taskTagRepository)
    {
        if (taskTagRepository.IsRelationExists(task.Id, tag.Id))
            throw new Exception("Task already has this tag");

        return new TaskTag(task, tag);
    }
    //on pourrait aussi rajouter une méthode qui prend en paramètre des paramètres optionnels
}
