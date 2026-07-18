using System;
using System.Collections.Generic;

namespace task17;

public static class ExceptionHandler
{
    private static readonly List<Exception> _errors = new();

    public static void Handle(ICommand command, Exception exception)
    {
        lock (_errors)
        {
            _errors.Add(exception);
        }
    }

    public static void ClearErrors()
    {
        lock (_errors)
        {
            _errors.Clear();
        }
    }

    public static IReadOnlyList<Exception> GetErrors()
    {
        lock (_errors)
        {
            return _errors.ToArray();
        }
    }
}
