using Xunit;
using System.Threading;
using task17;

namespace task17tests;

public class ServerThreadTests
{
    [Fact]
    public void ServerThread_CommandsInOrder()
    {
        var serv = new ServerThread();
        serv.Start();
        int res1 = 0;
        int res2 = 0;
        int order = 0;
        serv.Add(new TestCommand(() => { res1 = ++order; }, "Cmd1"));
        serv.Add(new TestCommand(() => { res2 = ++order; }, "Cmd2"));
        Thread.Sleep(200);
        serv.Add(new HardStopCommand(serv));
        serv.Join();

        Assert.Equal(1, res1);
        Assert.Equal(2, res2);
    }

    [Fact]
    public void HardStop_ShouldStop()
    {
        var serv = new ServerThread();
        int executed = 0;
        serv.Start();
        serv.Add(new TestCommand(() =>
        {
            executed++;
            Thread.Sleep(1000);
        }, "LongCmd"));
        Thread.Sleep(100);
        serv.Add(new HardStopCommand(serv));
        Thread.Sleep(500);
        serv.Join();

        Assert.True(executed <= 1);
        Assert.False(serv.IsAlive);
    }

    [Fact]
    public void SoftStop_ShouldStopAfterAll()
    {
        var serv = new ServerThread();
        serv.Start();
        int executed = 0;
        serv.Add(new TestCommand(() => { executed++; Thread.Sleep(50); }));
        serv.Add(new TestCommand(() => { executed++; Thread.Sleep(50); }));
        serv.Add(new SoftStopCommand(serv));
        Thread.Sleep(500);
        serv.Join();

        Assert.Equal(2, executed);
        Assert.False(serv.IsAlive);
    }

    [Fact]
    public void HardStopFromWrongThread_ThrowException()
    {
        var serv = new ServerThread();
        serv.Start();
        var hardStop = new HardStopCommand(serv);

        Assert.Throws<InvalidOperationException>(() => hardStop.Execute());
    }

    [Fact]
    public void SoftStopFromWrongThread_ThrowException()
    {
        var serv = new ServerThread();
        serv.Start();
        var softStop = new SoftStopCommand(serv);

        Assert.Throws<InvalidOperationException>(() => softStop.Execute());
    }

    [Fact]
    public void ErrorCommand_NotStopThread()
    {
        var serv = new ServerThread();
        serv.Start();
        ExceptionHandler.ClearErrors();
        serv.Add(new ErrorCommand("Test error"));
        serv.Add(new TestCommand(() => { }));
        Thread.Sleep(200);
        serv.Add(new HardStopCommand(serv));
        serv.Join();
        var errors = ExceptionHandler.GetErrors();

        Assert.Single(errors);
        Assert.Contains("Test error", errors[0].Message);
        Assert.False(serv.IsAlive);
    }

    [Fact]
    public void AddCommandAfterStop_ThrowException()
    {
        var serv = new ServerThread();
        serv.Start();
        serv.Add(new SoftStopCommand(serv));
        Thread.Sleep(200);
        serv.Join();

        Assert.Throws<InvalidOperationException>(() =>
        {
            serv.Add(new TestCommand(() => { }, "NewCmd"));
        });
    }
}
