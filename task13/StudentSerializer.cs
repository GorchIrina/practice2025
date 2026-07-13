using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace task13;

public class StudentSerializer
{
    private readonly JsonSerializerOptions _options;

    public StudentSerializer()
    {
        _options = new JsonSerializerOptions
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Converters = { new CustomDateConverter() }
        };
    }

    public string SerializeToJson(Student student)
    {
        if (student == null)
        {
            throw new ArgumentNullException(nameof(student));
        }
        return JsonSerializer.Serialize(student, _options);
    }

    public Student DeserializeFromJson(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            throw new ArgumentException("JSON не может быть пустым", nameof(json));
        }
        var student = JsonSerializer.Deserialize<Student>(json, _options) ?? throw new JsonException("Не удалось десериализовать Student");
        Validate(student);
        return student;
    }

    public void SaveToFile(Student student, string filePath)
    {
        if (student == null)
        {
            throw new ArgumentNullException(nameof(student));
        }
        if (string.IsNullOrWhiteSpace(filePath))
        {
            throw new ArgumentException("Путь не может быть пустым", nameof(filePath));
        }
        var json = SerializeToJson(student);
        File.WriteAllText(filePath, json);
    }

    public Student LoadFromFile(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("Файл не найден");
        }
        var json = File.ReadAllText(filePath);
        return DeserializeFromJson(json);
    }

    private static void Validate(Student student)
    {
        if (string.IsNullOrWhiteSpace(student.FirstName))
        {
            throw new JsonException("Имя не может быть пустым");
        }
        if (string.IsNullOrWhiteSpace(student.LastName))
        {
            throw new JsonException("Фамилия не может быть пустой");
        }
        if (student.BirthDate == default)
        {
            throw new JsonException("Дата рождения не может быть пустым");
        }
        if (student.BirthDate > DateTime.Now)
        {
            throw new JsonException("Некорректная дата рождения");
        }
        if (student.Grades == null)
        {
            throw new JsonException("Оценка не может быть null");
        }
        foreach (var subject in student.Grades)
        {
            if (string.IsNullOrWhiteSpace(subject.Name))
            {
                throw new JsonException("Название предмета не может быть пустым.");
            } 
            if (subject.Grade < 1 || subject.Grade > 5)
            {
                throw new JsonException($"Оценка должна быть от 1 до 5.");
            }     
        }
    }
}
