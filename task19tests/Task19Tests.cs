using Xunit;
using System;
using System.Collections.Generic;
using System.Threading;
using task17;
using task18;
using task19;

namespace task19tests;

public class Task19Tests
{
    [Fact]
    public void FiveCommands_Total15Calls()
    {
        var scheduler = new RoundRobinScheduler();
        var thread = new SchedulerThread(scheduler);
        int callCount = 0;
        for (int i = 0; i < 5; i++)
        {
            int id = i;
            var cmd = new RepeatableCommand(
                id,
                3,
                scheduler,
                (cmdId, call) =>
                {
                    Interlocked.Increment(ref callCount);
                }
            );
            scheduler.Add(cmd);
        }

        thread.Start();
        Thread.Sleep(1000);
        thread.AddCommand(new HardStop(thread));
        thread.Join();

        Assert.Equal(15, callCount);
    }

    [Fact]
    public void EachCommand_ExactlyThreeTimes()
    {
        var scheduler = new RoundRobinScheduler();
        var thread = new SchedulerThread(scheduler);
        int[] counters = new int[5];
        for (int i = 0; i < 5; i++)
        {
            int id = i;
            var cmd = new RepeatableCommand(id, 3, scheduler,
                (cmdId, call) =>
                {
                    Interlocked.Increment(ref counters[id]);
                }
            );
            scheduler.Add(cmd);
        }
        thread.Start();
        Thread.Sleep(1000);
        thread.AddCommand(new HardStop(thread));
        thread.Join();

        for (int i = 0; i < 5; i++)
        {
            Assert.Equal(3, counters[i]);
        }
    }

    [Fact]
    public void Order_RoundRobin()
    {
        var scheduler = new RoundRobinScheduler();
        var thread = new SchedulerThread(scheduler);
        var order = new List<int>();
        for (int i = 0; i < 5; i++)
        {
            int id = i;
            var cmd = new RepeatableCommand(id, 3, scheduler,
                (cmdId, call) =>
                {
                    lock (order)
                    {
                        order.Add(cmdId);
                    }
                }
            );
            scheduler.Add(cmd);
        }
        thread.Start();
        Thread.Sleep(1000);
        thread.AddCommand(new HardStop(thread));
        thread.Join();

        Assert.Equal(15, order.Count);
        int[] expected = { 0, 1, 2, 3, 4, 0, 1, 2, 3, 4, 0, 1, 2, 3, 4 };
        for (int i = 0; i < expected.Length; i++)
        {
            Assert.Equal(expected[i], order[i]);
        }
    }
}
