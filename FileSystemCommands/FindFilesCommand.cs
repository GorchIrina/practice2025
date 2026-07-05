using System;
using System.IO;
using System.Linq; 

[DisplayName("Комманда маска")]
[Version("2.0")]
public class FindFilesCommand: ICommand
{
    private readonly string _path;
    private readonly string _mask;
    public FindFilesCommand(string path, string mask)
    {
        _path=path;
        _mask=mask;
    }

    public void Execute()
    {
        if(!Directory.Exists(_path))
        {
            Console.WriteLine($"Каталог {_path} не найден");
            return;
        }
        var files = Directory.GetFiles(_path, _mask ,SearchOption.AllDirectories);
        Console.WriteLine("Файлы по маске");
        foreach(var file in files)
        {
            Console.WriteLine($"{file}");
        }
    }
}
