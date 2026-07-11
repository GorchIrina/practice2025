using System;

[PluginLoad]
public class PluginA : ICommand
{
    public void Execute()
    {
        Console.WriteLine("PluginA executed.");
    }
}

[PluginLoad("PluginA")]
public class PluginB : ICommand
{
    public void Execute()
    {
        Console.WriteLine("PluginB executed.");
    }
}

[PluginLoad("PluginB")]
public class PluginC : ICommand
{
    public void Execute()
    {
        Console.WriteLine("PluginC executed.");
    }
}

public class PluginWithoutAttr : ICommand
{
    public void Execute()
    {
        Console.WriteLine("PluginWithoutAttr executed.");
    }
}
