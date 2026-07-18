using System;

namespace task17;

public class ErrorCommand : ICommand
{
    private readonly string _message;

    public ErrorCommand(string message = "Test exception")
    {
        _message = message;
    }

    public void Execute()
    {
        throw new InvalidOperationException(_message);
    }
}
