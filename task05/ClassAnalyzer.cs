using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

public class ClassAnalyzer
{
    private Type _type;

    public ClassAnalyzer(Type type)
    {
        _type = type;
    }

    public IEnumerable<string> GetPublicMethods()
    {
        return _type.GetMethods(BindingFlags.Public | BindingFlags.Instance ).Select(m => m.Name);
    }

    public IEnumerable<string> GetMethodParams(string methodname)
    {
        var method = _type.GetMethod(methodname);
        if (method is null)
        {
            return Enumerable.Empty<string>();
        }
        var result = method.GetParameters().Select(p => p.Name).ToList();
        result.Add(method.ReturnType.Name);
        return result;
    }

    public IEnumerable<string> GetAllFields()
    {
        return _type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Select(f => f.Name);
    }

    public IEnumerable<string> GetProperties()
    {
        return _type.GetProperties(BindingFlags.Public | BindingFlags.Instance).Select(p => p.Name);
    }

    public bool HasAttribute<T>() where T : Attribute
    {
        return _type.IsDefined(typeof(T), false);
    }
}
