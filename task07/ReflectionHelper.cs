using System;
using System.Reflection;

public static class ReflectionHelper
{
    public static void PrintTypeInfo(Type type)
    {
        var displayName = type.GetCustomAttribute<DisplayNameAttribute>();
        if(displayName!= null)
        {
            Console.WriteLine($"DisplayName: {displayName.DisplayName}");
        }

        var version = type.GetCustomAttribute<VersionAttribute>();
        if(version!= null)
        {
            Console.WriteLine($"Version: {version.Major}.{version.Minor}");
        }

        var methods = type.GetMethods();
        foreach(var method in methods)
        {
            var methodAttribute = method.GetCustomAttribute<DisplayNameAttribute>();
            if(methodAttribute != null)
            {
                Console.WriteLine($"{method.Name}: {methodAttribute.DisplayName}");
            }
        }

        var properties = type.GetProperties();
        foreach(var property in properties)
        {
            var propertyAttribute = property.GetCustomAttribute<DisplayNameAttribute>();
            if(propertyAttribute != null)
            {
                Console.WriteLine($"{property.Name}: {propertyAttribute.DisplayName}");
            }
        }
    }
}
