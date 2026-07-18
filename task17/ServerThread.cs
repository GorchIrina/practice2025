using System;
using System.Threading;
using System.Collections.Concurrent;

namespace task17;

public class ServerThread
{
    private readonly BlockingCollection<ICommand> _commands = new();
    private readonly Thread _thread;
    private volatile bool _softStop = false;
    private volatile bool _hardStop = false;

    public ServerThread()
    {
        _thread = new Thread(Run);
        _thread.IsBackground = true;
        _thread.Name = "ServerThread";
    }

    public Thread Thread => _thread;

    public void Start()
    {
        if (_thread.IsAlive)
        {
            throw new InvalidOperationException("Поток уже запущен");
        }
        _thread.Start();
    }

    public void Add(ICommand command)
    {
        if (command == null)
        {
            throw new ArgumentNullException(nameof(command));
        }
        if (_commands.IsAddingCompleted)
        {
            throw new InvalidOperationException("Поток остановлен");
        }
        _commands.Add(command);
    }

    public void SoftStop()
    {
        if (Thread.CurrentThread.ManagedThreadId != _thread.ManagedThreadId)
        {
            throw new InvalidOperationException("SoftStop можно вызвать только из самого потока");
        }
        _softStop = true;
        _commands.CompleteAdding();
    }

    public void HardStop()
    {
        if (Thread.CurrentThread.ManagedThreadId != _thread.ManagedThreadId)
        {
            throw new InvalidOperationException("HardStop можно вызвать только из самого потока");
        }
        _hardStop = true;
        _commands.CompleteAdding();
    }

    public bool IsAlive => _thread.IsAlive;

    public void Join()
    {
        _thread.Join();
    }

    private void Run()
    {
        foreach (var command in _commands.GetConsumingEnumerable())
        {
            if (_hardStop)
            {
                break;
            }

            try
            {
                command.Execute();
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(command, ex);
            }

            if (_softStop && _commands.Count == 0)
            {
                break;
            }
                
        }
    }
}
