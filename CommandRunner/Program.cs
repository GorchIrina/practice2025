using System;
using System.Reflection;
using System.Linq;

class Program
{
    static void Main(string[] args)
    {
        string dllPath = "FileSystemCommands.dll";
        Assembly assembly = Assembly.LoadFrom(dllPath);

        var commandTypes = assembly.GetTypes().Where(c => c.IsClass && c.GetInterface("ICommand") != null );
        string path="";
        if (args.Length > 0)
        {
            path = args[0];
        }
        else
        {
            path = "/tmp/TestDir";
        }
        string mask="";
        if (args.Length > 1)
        {
            mask = args[1];
        }
        else
        {
            mask = "*.txt";
        }

        foreach (var type in commandTypes)
        {
            ICommand command = null;
            if (type.Name == "DirectorySizeCommand")
            {
                command = (ICommand)Activator.CreateInstance(type, path);
            }
            if (type.Name == "FindFilesCommand")
            {
                command = (ICommand)Activator.CreateInstance(type, path, mask);
            }

            if (command != null)
            {
                command.Execute();
            }
        }
    }
}
