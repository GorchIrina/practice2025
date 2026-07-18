using System;
using System.Collections.Generic;
using task17;

namespace task18;

public class RoundRobinScheduler : IScheduler
{
    private readonly List<ICommand> _commands = new List<ICommand>();
    private readonly object _lock = new object();
    private int _ind = 0;

    public bool HasCommand()
    {
        lock (_lock)
        {
            return _commands.Count > 0;
        }
    }

    public ICommand Select()
    {
        lock (_lock)
        {
            if (_commands.Count == 0)
            {
                throw new InvalidOperationException("Планировщик пуст");
            }
            if (_ind >= _commands.Count)
            {
                _ind = 0;
            }

            ICommand command = _commands[_ind];
            _commands.RemoveAt(_ind);

            if (_commands.Count > 0 && _ind >= _commands.Count)
            {
                _ind = 0;
            }
            
            return command;
        }
    }

    public void Add(ICommand cmd)
    {
        lock (_lock)
        {
            _commands.Add(cmd);
        }
    }
}
