using Xunit;

public class TestClass
{
    public int PublicField;
    private string _privateField;
    public static int StaticField;
    public int Property { get; set; }

    public void Method() { }
}

[Serializable]
public class AttributedClass { }

public class ClassAnalyzerTests
{
    [Fact]
    public void GetPublicMethods_ReturnsCorrectMethods()
    {
        var analyzer = new ClassAnalyzer(typeof(TestClass));
        var methods = analyzer.GetPublicMethods();

        Assert.Contains("Method", methods);
    }

    [Fact]
    public void GetAllFields_IncludesPrivateFields()
    {
        var analyzer = new ClassAnalyzer(typeof(TestClass));
        var fields = analyzer.GetAllFields();

        Assert.Contains("_privateField", fields);
    }

    [Fact]
    public void GetAllFields_ExceptStaticFields()
    {
        var analyzer = new ClassAnalyzer(typeof(TestClass));
        var fields = analyzer.GetAllFields();

        Assert.DoesNotContain("StaticField", fields);
    }

    [Fact]
    public void GetProperties_ReturnsCorrectProperties()
    {
        var analyzer = new ClassAnalyzer(typeof(TestClass));
        var properties = analyzer.GetProperties();

        Assert.Contains("Property", properties);
    }
    
    [Fact]
    public void HasAttribute_ReturnsTrueAttributePresent()
    {
        var analyzer = new ClassAnalyzer(typeof(AttributedClass));

        Assert.True(analyzer.HasAttribute<SerializableAttribute>());
    }

    [Fact]
    public void HasAttribute_ReturnsFalseAttributeNotPresent()
    {
        var analyzer = new ClassAnalyzer(typeof(TestClass));

        Assert.False(analyzer.HasAttribute<SerializableAttribute>());
    }
}
