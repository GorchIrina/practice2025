using Xunit;
using System;
using System.Reflection;
using System.Collections.Generic;

public class PluginLoaderTest
{
    [Fact]
    public void LoadPlugins_ShouldFindWithAttributes()
    {
        var pluginAType = typeof(PluginA);
        var attrA = pluginAType.GetCustomAttribute<PluginLoadAttribute>();

        var pluginBType = typeof(PluginB);
        var attrB = pluginBType.GetCustomAttribute<PluginLoadAttribute>();

        var pluginWithoutAttrType = typeof(PluginWithoutAttr);
        var noAttr = pluginWithoutAttrType.GetCustomAttribute<PluginLoadAttribute>();

        Assert.NotNull(attrA);
        Assert.NotNull(attrB);
        Assert.Null(noAttr);
    }

    [Fact]
    public void LoadPlugins_ShouldDependencyOrder()
    {
        var dependsOn = new Dictionary<string, List<string>>
        {
            ["PluginA"] = new List<string>(),
            ["PluginB"] = new List<string> {"PluginA"},
            ["PluginC"] = new List<string> {"PluginB"}
        };

        var loader = new PluginLoader("aaa");
        var topologicalSort = typeof(PluginLoader).GetMethod("TopologicalSort", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.NotNull(topologicalSort);
        var result = (List<string>)topologicalSort.Invoke(loader, new object[] {dependsOn});

        Assert.Equal(new[] {"PluginA", "PluginB", "PluginC"}, result);
    }

    [Fact]
    public void LoadPlugins_ShouldThrowCircularDependency()
    {
        var dependsOn = new Dictionary<string, List<string>>
        {
            ["PluginA"] = new List<string> {"PluginB"},
            ["PluginB"] = new List<string> {"PluginA"}
        };

        var loader = new PluginLoader("aaa");
        var topologicalSort = typeof(PluginLoader).GetMethod("TopologicalSort", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.NotNull(topologicalSort);

        var exception = Assert.Throws<TargetInvocationException>(() => topologicalSort.Invoke(loader, new object[] {dependsOn}));
        Assert.IsType<InvalidOperationException>(exception.InnerException);
        Assert.Contains("Циклическая зависимость", exception.InnerException.Message);
    }
}
