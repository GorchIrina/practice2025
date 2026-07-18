using System;
using System.Collections.Generic;
using System.Threading;
using task17;
using task18;
using Xunit;

namespace task18tests;

public class LongCommand : ICommand
{
    private readonly List<int> _log;
    private readonly int _id;
    private readonly int _steps;
    private int _currentStep = 0;
    private readonly IScheduler _scheduler;

    public LongCommand(List<int> log, int id, int steps, IScheduler scheduler)
    {
        _log = log;
        _id = id;
        _steps = steps;
        _scheduler = scheduler;
    }

    public void Execute()
    {
        _currentStep++;
        _log.Add(_id);
        if (_currentStep < _steps)
        {
            _scheduler.Add(this);
        } 
    }
}

public class SimpleCommand : ICommand
{
    private readonly List<int> _log;
    private readonly int _id;

    public SimpleCommand(List<int> log, int id)
    {
        _log = log;
        _id = id;
    }

    public void Execute()
    {
        _log.Add(_id);
    }
}

public class SchedulerThreadTests
{
    [Fact]
    public void RoundRobin_InOrder()
    {
        var scheduler = new RoundRobinScheduler();
        var log = new List<int>();
        scheduler.Add(new SimpleCommand(log, 1));
        scheduler.Add(new SimpleCommand(log, 2));
        scheduler.Add(new SimpleCommand(log, 3));
        scheduler.Select().Execute();
        scheduler.Select().Execute();
        scheduler.Select().Execute();

        Assert.Equal(new List<int> {1,2,3}, log);
    }

    [Fact]
    public void RoundRobin_HasCommandCorrectly()
    {
        var scheduler = new RoundRobinScheduler();

        Assert.False(scheduler.HasCommand());

        scheduler.Add(new SimpleCommand(new List<int>(), 1));

        Assert.True(scheduler.HasCommand());
    }

    [Fact]
    public void SchedulerThread_HardStop_HardStop_ShouldStop()
    {
        var scheduler = new RoundRobinScheduler();
        var serv = new SchedulerThread(scheduler);
        var log = new List<int>();
        serv.Start();
        serv.AddCommand(new HardStop(serv));
        serv.AddCommand(new SimpleCommand(log, 1));
        serv.AddCommand(new SimpleCommand(log, 2));
        serv.Join();

        Assert.False(serv.IsAlive);
    }

    [Fact]
    public void SchedulerThread_SoftStop_ShouldStopAfterAll()
    {
        var scheduler = new RoundRobinScheduler();
        var serv = new SchedulerThread(scheduler);
        var log = new List<int>();
        serv.Start();
        serv.AddCommand(new SimpleCommand(log, 1));
        serv.AddCommand(new SimpleCommand(log, 2));
        serv.AddCommand(new SimpleCommand(log, 3));
        serv.AddCommand(new SoftStop(serv));
        serv.Join();

        Assert.Contains(1, log);
        Assert.Contains(2, log);
        Assert.Contains(3, log);
        Assert.False(serv.IsAlive);
    }

    [Fact]
    public void SchedulerThread_HardStop_ThrowsException()
    {
        var scheduler = new RoundRobinScheduler();
        var serv = new SchedulerThread(scheduler);
        serv.Start();
        var hardStop = new HardStop(serv);

        Assert.Throws<InvalidOperationException>(() => hardStop.Execute());

        serv.AddCommand(new HardStop(serv));
        serv.Join();
    }

    [Fact]
    public void SchedulerThread_SoftStop_ThrowsException()
    {
        var scheduler = new RoundRobinScheduler();
        var serv = new SchedulerThread(scheduler);
        serv.Start();
        var softStop = new SoftStop(serv);

        Assert.Throws<InvalidOperationException>(() => softStop.Execute());

        serv.AddCommand(new HardStop(serv));
        serv.Join();
    }

    [Fact]
    public void LongCommand_ToScheduler()
    {
        var scheduler = new RoundRobinScheduler();
        var serv = new SchedulerThread(scheduler);
        var log = new List<int>();
        serv.Start();
        serv.AddCommand(new LongCommand(log, 1, 3, scheduler));
        serv.AddCommand(new SoftStop(serv));
        serv.Join();

        Assert.Equal(3, log.Count);
        Assert.All(log, id => Assert.Equal(1, id));
    }

    [Fact]
    public void TwoLongCommands_Alternate()
    {
        var scheduler = new RoundRobinScheduler();
        var serv = new SchedulerThread(scheduler);
        var log = new List<int>();
        serv.Start();
        serv.AddCommand(new LongCommand(log, 1, 3, scheduler));
        serv.AddCommand(new LongCommand(log, 2, 3, scheduler));
        serv.AddCommand(new SoftStop(serv));
        serv.Join();

        Assert.Equal(3, log.Count(x => x == 1));
        Assert.Equal(3, log.Count(x => x == 2));
        for (int i = 0; i < log.Count - 2; i++)
        {
            Assert.False(log[i] == log[i + 1] && log[i + 1] == log[i + 2]);
        }
    }

    [Fact]
    public void AddCommand_AfterStop_ThrowsException()
    {
        var scheduler = new RoundRobinScheduler();
        var serv = new SchedulerThread(scheduler);
        serv.Start();
        serv.AddCommand(new HardStop(serv));
        serv.Join();

        Assert.Throws<InvalidOperationException>(() =>
        {
            serv.AddCommand(new SimpleCommand(new List<int>(), 1));
        });
    }
}


