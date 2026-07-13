using Xunit;
using System;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;
using task13;

namespace task13tests;

public class StudentSerializerTests
{
    private readonly StudentSerializer _serializer = new StudentSerializer();
    private static Student MakeStudent()
    {
        var student = new Student();
        student.FirstName = "Ivan";
        student.LastName = "Ivanov";
        student.BirthDate = new DateTime(2007, 2, 1);
        student.Grades = new List<Subject>
        {
            new Subject { Name = "Math", Grade = 5 },
            new Subject { Name = "Science", Grade = 4 }
        };
        return student;
    }

    [Fact]
    public void Serialize_ReturnJson()
    {
        var json = _serializer.SerializeToJson(MakeStudent());

        Assert.Contains("Ivan", json);
        Assert.Contains("Ivanov", json);
        Assert.Contains("01.02.2007", json);
        Assert.Contains("Math", json);
        Assert.Contains("Science", json);
        Assert.Contains("5", json);
        Assert.Contains("4", json);
    }
    [Fact]
    public void Serialize_GradesNullShouldIgnoreNull()
    {
        var student = new Student
        {
            FirstName = "Ivan",
            LastName = "Ivanov",
            BirthDate = new DateTime(2007, 2, 1),
            Grades = null
        };
        var json = _serializer.SerializeToJson(student);

        Assert.DoesNotContain("Grades", json);
        Assert.Contains("Ivan", json);
        Assert.Contains("Ivanov", json);
        Assert.Contains("01.02.2007", json);
    }

    [Fact]
    public void Serialize_ThrowsNull()
    {
        Action act = () => _serializer.SerializeToJson(null!);

        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public void Deserialize_ReturnStudent()
    {
        var originalStudent = MakeStudent();
        var json = _serializer.SerializeToJson(originalStudent);
        var returnedStudent = _serializer.DeserializeFromJson(json);

        Assert.Equal(originalStudent.FirstName, returnedStudent.FirstName);
        Assert.Equal(originalStudent.LastName, returnedStudent.LastName);
        Assert.Equal(originalStudent.BirthDate, returnedStudent.BirthDate);
        Assert.Equal(originalStudent.Grades[0].Name, returnedStudent.Grades[0].Name);
        Assert.Equal(originalStudent.Grades[0].Grade, returnedStudent.Grades[0].Grade);
        Assert.Equal(originalStudent.Grades.Count, returnedStudent.Grades.Count);
    }

    [Fact]
    public void Deserialize_ThrowsEmptyJson()
    {
        Action act = () => _serializer.DeserializeFromJson("");

        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void Deserialize_ThrowsEmptyFirstName()
    {
        var json = @"
{
  ""FirstName"": """",
  ""LastName"": ""Ivanov"",
  ""BirthDate"": ""01.02.2997"",
  ""Grades"": [{ ""Name"": ""Math"", ""Grade"": 5 }]
}";
        Action act = () => _serializer.DeserializeFromJson(json);

        Assert.Throws<JsonException>(act);
    }

    [Fact]
    public void Deserialize_ThrowsOnFutureBirthDate()
    {
        var json = @"
{
  ""FirstName"": ""Ivan"",
  ""LastName"": ""Ivanov"",
  ""BirthDate"": ""01.02.2997"",
  ""Grades"": [{ ""Name"": ""Math"", ""Grade"": 5 }]
}";
        Action act = () => _serializer.DeserializeFromJson(json);

        Assert.Throws<JsonException>(act);
    }

    [Fact]
    public void Deserialize_ThrowsOnInvalidGrade()
    {
        var json = @"
{
  ""FirstName"": ""Ivan"",
  ""LastName"": ""Ivanov"",
  ""BirthDate"": ""01.02.2007"",
  ""Grades"": [{ ""Name"": ""Math"", ""Grade"": 0 }]
}";
        Action act = () => _serializer.DeserializeFromJson(json);

        Assert.Throws<JsonException>(act);
    }

    [Fact]
    public void Deserialize_InvalidDateFormat()
    {
        var json = @"
{
  ""FirstName"": ""Ivan"",
  ""LastName"": ""Ivanov"",
  ""BirthDate"": ""2007-02-01"",
  ""Grades"": [{ ""Name"": ""Math"", ""Grade"": 5 }]
}";
        Action act = () => _serializer.DeserializeFromJson(json);

        Assert.Throws<JsonException>(act);
    }

    [Fact]
    public void SaveAndLoad_File()
    {
        var originalStudent = MakeStudent();
        var filePath = Path.GetTempFileName();
        try
        {
            _serializer.SaveToFile(originalStudent, filePath);
            var loadedStudent = _serializer.LoadFromFile(filePath);

            Assert.Equal(originalStudent.FirstName, loadedStudent.FirstName);
            Assert.Equal(originalStudent.LastName, loadedStudent.LastName);
            Assert.Equal(originalStudent.BirthDate, loadedStudent.BirthDate);
            Assert.Equal(originalStudent.Grades.Count, loadedStudent.Grades.Count);
        }
        finally
        {
            File.Delete(filePath); 
        }
    }

    [Fact]
    public void LoadFromFile_ThrowsFileNotFound()
    {
        Action act = () => _serializer.LoadFromFile("notexist.json");
        Assert.Throws<FileNotFoundException>(act);
    }
}
