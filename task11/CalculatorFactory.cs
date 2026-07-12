using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

public class CalculatorFactory
{
    public T CreateInstance<T>(string code) where T : class
    {
        string fullcode = "using System;\n" + code;

        SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(fullcode);

        MetadataReference[] references =
        [
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
            MetadataReference.CreateFromFile(Assembly.Load("System.Runtime").Location),
            MetadataReference.CreateFromFile(typeof(T).Assembly.Location),
        ];

        var compilation = CSharpCompilation
            .Create("DynamicAssembly")
            .AddSyntaxTrees(syntaxTree)
            .AddReferences(references)
            .WithOptions(new CSharpCompilationOptions(
                OutputKind.DynamicallyLinkedLibrary));

        using var memoryStream = new MemoryStream();

        var emitResult = compilation.Emit(memoryStream);

        if (!emitResult.Success)
        {
            var errors = string.Join("\n", emitResult.Diagnostics
                .Where(d => d.Severity == DiagnosticSeverity.Error)
                .Select(d => d.ToString()));

            throw new Exception($"Ошибка компиляции:\n{errors}");
        }

        memoryStream.Seek(0, SeekOrigin.Begin);

        var assembly = Assembly.Load(memoryStream.ToArray());

        var type = assembly.GetType("Calculator")
            ?? throw new Exception("Класс 'Calculator' не найден в сборке.");

        var instance = Activator.CreateInstance(type)
            ?? throw new Exception("Не удалось создать Calculator.");

        return (T)instance;
    }
}
