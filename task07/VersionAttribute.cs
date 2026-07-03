using System;

[AttributeUsage(AttributeTargets.Class)]
public class VersionAttribute: Attribute
{
    public int Major {get; }
    public int Minor {get; }

    public VersionAttribute(string version)
    {
        var parts = version.Split('.');
        if(parts.Length!=2 || !int.TryParse(parts[0], out int major) || !int.TryParse(parts[1], out int minor) )
        {
            throw new ArgumentException("Incorrect version format");
        }
        Major = major;
        Minor = minor;
    }
}
