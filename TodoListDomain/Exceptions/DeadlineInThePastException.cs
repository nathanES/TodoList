namespace TodoList.Domain.Exceptions;
public class DeadlineInThePastException : Exception
{
    public DeadlineInThePastException()
    {
    }

    public DeadlineInThePastException(string message)
        : base(message)
    {
    }

    public DeadlineInThePastException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
