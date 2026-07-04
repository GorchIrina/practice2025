using System;
using System.IO;
using System.Linq; 

public class DirectorySizeCommand: ICommand
{
    private readonly string _path;
    public DirectorySizeCommand(string path)
    {
        _path=path;
    }

    public void Execute()
    {
        if(!Directory.Exists(_path))
        {
            Console.WriteLine($"Каталог {_path} не найден");
            return;
        }
        var files = Directory.GetFiles(_path,"*",SearchOption.AllDirectories);
        long filesSize= 0;
        foreach (var file in files)
        {
            filesSize+= new FileInfo(file).Length;
        }
        Console.WriteLine($"Размер каталога: {filesSize} байт");
    }
}
