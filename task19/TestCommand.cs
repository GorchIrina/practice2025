using task17;

namespace task19;

public class TestCommand : ICommand
{
    private readonly int _id;
    private int _counter = 0;

    public TestCommand(int id)
    {
        _id = id;
    }

    public void Execute()
    {
        _counter++;
    }

    public int Counter => _counter;
    public int Id => _id;
}
