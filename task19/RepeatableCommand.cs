using System;
using task17;
using task18;

namespace task19;

public class RepeatableCommand : ICommand
{
    private readonly TestCommand _inner;
    private readonly int _id;
    private int _counter = 0;
    private readonly int _maxCalls;
    private readonly IScheduler _scheduler;
    private readonly Action<int, int> _onExecute;

    public RepeatableCommand(int id, int maxCalls, IScheduler scheduler, Action<int, int> onExecute)
    {
        _id = id;
        _maxCalls = maxCalls;
        _scheduler = scheduler;
        _onExecute = onExecute;
        _inner = new TestCommand(id);
    }

    public void Execute()
    {
        if (_counter >= _maxCalls)
        {
            return;
        }
        _counter++;
        _inner.Execute();
        _onExecute?.Invoke(_id, _counter);

        if (_counter < _maxCalls)
        {
            _scheduler.Add(this);
        }
    }

    public int Counter => _counter;
    public int Id => _id;
}
