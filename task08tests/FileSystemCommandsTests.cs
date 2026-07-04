using Xunit;
using System;
using System.IO;

public class FileSystemCommandsTests
{
    [Fact]
    public void DirectorySizeCommand_ShouldCalculateSize()
    {
        var testDir = Path.Combine(Path.GetTempPath(), "TestDir");
        Directory.CreateDirectory(testDir);
        File.WriteAllText(Path.Combine(testDir, "test1.txt"), "Hello");
        File.WriteAllText(Path.Combine(testDir, "test2.txt"), "World");

        var command = new DirectorySizeCommand(testDir);
        var output = new StringWriter();
        Console.SetOut(output);
        command.Execute(); 
        var consoleOutput = output.ToString();
        Assert.Contains("10 байт", consoleOutput);

        Directory.Delete(testDir, true);
        var standardOutput = new StreamWriter(Console.OpenStandardOutput());
        Console.SetOut(standardOutput); 
    }

    [Fact]
    public void FindFilesCommand_ShouldFindMatchingFiles()
    {
        var testDir = Path.Combine(Path.GetTempPath(), "TestDir");
        Directory.CreateDirectory(testDir);
        File.WriteAllText(Path.Combine(testDir, "file1.txt"), "Text");
        File.WriteAllText(Path.Combine(testDir, "file2.log"), "Log");

        var command = new FindFilesCommand(testDir, "*.txt");
        var output = new StringWriter();
        Console.SetOut(output);
        command.Execute(); 
        var consoleOutput = output.ToString();
        Assert.Contains("file1.txt", consoleOutput);
        Assert.DoesNotContain("file2.log", consoleOutput);

        Directory.Delete(testDir, true);
        var standardOutput = new StreamWriter(Console.OpenStandardOutput());
        Console.SetOut(standardOutput); 
    }
}
