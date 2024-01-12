using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoList.Domain.Exceptions;
public class DuplicateKeyException : Exception
{
    public DuplicateKeyException()
    {
    }

    public DuplicateKeyException(string message)
        : base(message)
    {
    }

    public DuplicateKeyException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
