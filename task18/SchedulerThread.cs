using System;
using System.Collections.Concurrent;
using System.Threading;
using task17;

namespace task18;

public class SchedulerThread
{
    private readonly BlockingCollection<ICommand> _queue = new BlockingCollection<ICommand>();
    private readonly IScheduler _scheduler;
    private Thread? _thread;
    private volatile bool _hardStop = false;
    private volatile bool _softStop = false;

    public SchedulerThread(IScheduler scheduler)
    {
        _scheduler = scheduler;
    }

    public int ThreadId
    {
        get
        {
            if (_thread == null)
            {
                return -1;
            }
            return _thread.ManagedThreadId;
        }
    }
    
    public bool IsAlive
    {
        get
        {
            if (_thread == null)
            {
                return false;
            }
                
            return _thread.IsAlive;
        }
    }

    public void AddCommand(ICommand command)
    {
        if (command == null)
        {
            throw new ArgumentNullException(nameof(command));
        }
        if (_queue.IsAddingCompleted)
        {
            throw new InvalidOperationException("Поток остановлен");
        }
        _queue.Add(command);
    }

    public void Start()
    {
        if (_thread != null && _thread.IsAlive)
        {
            throw new InvalidOperationException("Поток уже запущен");
        }
        _thread = new Thread(Run);
        _thread.IsBackground = true;
        _thread.Name = "SchedulerThread";
        _thread.Start();
    }

    private void Run()
    {
        while (true)
        {
            if (_hardStop)
            {
                break;
            }
            if (_queue.TryTake(out ICommand? newCommand, TimeSpan.Zero))
            {
                _scheduler.Add(newCommand);
            }
            if (_scheduler.HasCommand())
            {
                if (_hardStop)
                {
                    break;
                }
                
                ICommand command = _scheduler.Select();
                try
                {
                    command.Execute();
                }
                catch (Exception ex)
                {
                    ExceptionHandler.Handle(command, ex);
                }

                if ( _queue.Count == 0 && _softStop && !_scheduler.HasCommand() )
                {
                    break;
                }
            }
            else
            {
                if (_softStop)
                {
                    break;
                }
                if (_queue.TryTake(out ICommand? cmd, Timeout.InfiniteTimeSpan))
                {
                    _scheduler.Add(cmd);
                }
            }
        }
    }

    internal void HardStop()
    {
        if (Thread.CurrentThread.ManagedThreadId != ThreadId)
        {
            throw new InvalidOperationException("Можно вызвать только из самого потока");
        }
        _hardStop = true;
        _queue.CompleteAdding();
    }
    
    internal void SoftStop()
    {
        if (Thread.CurrentThread.ManagedThreadId != ThreadId)
        {
            throw new InvalidOperationException("Можно вызвать только из самого потока");
        }
        _softStop = true;
        _queue.CompleteAdding();
    }

    public void Join()
    {
        _thread?.Join();
    }
}
