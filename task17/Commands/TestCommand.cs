using System;

namespace task17;

public class TestCommand : ICommand
{
    private readonly Action _action;
    private readonly string _name;

    public TestCommand(Action action, string name = "Test")
    {
        if (action == null)
        {
            throw new ArgumentNullException(nameof(action));
        }
        _action = action;
        if (name == null)
        {
            _name = "Test";
        }
        else
        {
            _name = name;
        }
    }

    public void Execute()
    {
        _action?.Invoke();
    }
}
