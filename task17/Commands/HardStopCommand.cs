using System.Threading;

namespace task17;

public class HardStopCommand : ICommand
{
    private readonly ServerThread _server;

    public HardStopCommand(ServerThread server)
    {
        _server = server;
    }

    public void Execute()
    {
        if (Thread.CurrentThread.ManagedThreadId != _server.Thread.ManagedThreadId)
        {
            throw new InvalidOperationException("Можно выполнить только из самого потока");
        }
        _server.HardStop();
    }
}
