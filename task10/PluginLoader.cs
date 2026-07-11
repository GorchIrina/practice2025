using System;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

public class PluginLoader
{
    private readonly string _pluginPath;
    public PluginLoader(string pluginPath)
    {
        _pluginPath = pluginPath;
    }

    public List<ICommand> LoadPlugins()
    {
        if(!Directory.Exists(_pluginPath))
        {
            throw new DirectoryNotFoundException($"Папка {_pluginPath} не найдена.");
        }

        var plugins = new List<ICommand>();
        var pluginTypes = new Dictionary<string, Type>();
        var dependsOn= new Dictionary<string, List<string>>();
        var dllFiles = Directory.GetFiles(_pluginPath, "*.dll");
        foreach ( var dllFile in dllFiles)
        {
            try
            {
                var assembly = Assembly.LoadFrom(dllFile);
                var types = assembly.GetTypes().Where(t=> t.IsClass && !t.IsAbstract && typeof(ICommand).IsAssignableFrom(t));
                foreach (var type in types)
                {
                    var attribute = type.GetCustomAttribute<PluginLoadAttribute>();
                    if (attribute != null)
                    {
                        pluginTypes[type.Name] = type;

                        dependsOn[type.Name] = new List<string>();
                        if (attribute.DependsOn != null && attribute.DependsOn.Length > 0)
                        {
                            dependsOn[type.Name].AddRange(attribute.DependsOn);
                        }
                    }
                }
            }
            catch(Exception exception)
            {
                throw new InvalidOperationException($"Ошибка загрузки {dllFile}: {exception.Message}", exception);
            }
        }

        var sortedPlugins = TopologicalSort(dependsOn);
        foreach (var name in sortedPlugins)
        {
            if (pluginTypes.TryGetValue(name, out var type))
            {
                try
                {
                    var plugin = (ICommand?)Activator.CreateInstance(type);
                    if (plugin != null)
                    {
                        plugins.Add(plugin);
                    }
                }
                catch (Exception exception)
                {
                    throw new InvalidOperationException($"Ошибка создания плагина {name}: {exception.Message}", exception);
                }
            }
        }

        return plugins;
    }

    private List<string> TopologicalSort(Dictionary<string, List<string>> dependsOn)
    {
        var sorted = new List<string>();
        var visited = new HashSet<string>();
        var inprogress = new HashSet<string>();

        foreach (var plug in dependsOn.Keys)
        {
            Visit(plug, dependsOn, sorted, visited, inprogress);
        }
        return sorted;
    }

    private void Visit(string plug, Dictionary<string, List<string>> dependsOn, List<string> sorted, HashSet<string> visited, HashSet<string> inprogress)
    {
        if (visited.Contains(plug))
        {
            return;
        }

        if (inprogress.Contains(plug))
        {
            throw new InvalidOperationException($"Циклическая зависимость: {plug}");
        }

        inprogress.Add(plug);

        if (dependsOn.TryGetValue(plug, out var deps))
        {
            foreach (var dep in deps)
            {
                Visit(dep, dependsOn, sorted, visited, inprogress);
            }
        }

        inprogress.Remove(plug);
        visited.Add(plug);
        sorted.Add(plug);
    }
}
