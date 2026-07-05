using System;
using System.Reflection;
using System.Linq;

if (args.Length == 0)
{
    Console.WriteLine("Укажите путь до библиотеки");
    return;
}

var assembly = Assembly.LoadFrom(args[0]);
var types = assembly.GetTypes().Where(t => t.IsClass);
foreach(var type in types)
{
    Console.WriteLine($"Класс: {type.Name}");

    foreach (var method in type.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance))
    {
        Console.WriteLine($"Метод {method.Name}");
        foreach (var param in method.GetParameters())
        {
            Console.WriteLine($"Параметр {param.Name}: тип {param.ParameterType.Name}");
        }
    }
    
    foreach(var attribute in type.GetCustomAttributes().Where(a => a.GetType().Namespace != "System.Runtime.CompilerServices"))
    {
        Console.WriteLine($"Атрибут: {attribute}");
    }
    
    foreach (var constructor in type.GetConstructors())
    {
        Console.WriteLine($"Конструктор {type.Name}");
        foreach (var param in constructor.GetParameters())
        {
            Console.WriteLine($"Параметр {param.Name}: тип {param.ParameterType.Name}");
        }
    }
}
