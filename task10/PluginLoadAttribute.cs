using System;

[AttributeUsage(AttributeTargets.Class)]
public class PluginLoadAttribute: Attribute
{
    public string[] DependsOn { get; }  
    public PluginLoadAttribute(params string[] dependsOn)  
    {
        if (dependsOn == null)
        {
            DependsOn = Array.Empty<string>();
        }
        else
        {
            DependsOn = dependsOn;
        }
    }
}
