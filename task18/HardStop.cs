using System.Threading;
using task17;

namespace task18;

public class HardStop : ICommand
{
    private readonly SchedulerThread _schedulerThread;

    public HardStop(SchedulerThread schedulerThread)
    {
        _schedulerThread = schedulerThread;
    }

    public void Execute()
    {
        if (Thread.CurrentThread.ManagedThreadId != _schedulerThread.ThreadId)
        {
            throw new InvalidOperationException("Может выполняться только в потоке SchedulerThread");
        }
        _schedulerThread.HardStop();
    }
}
