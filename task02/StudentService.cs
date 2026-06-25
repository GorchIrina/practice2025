using System;
using System.Collections.Generic;
using System.Linq;

public class StudentService
{
    private readonly List<Student> _students;

    public StudentService(List<Student> students) => _students = students;

    public IEnumerable<Student> GetStudentsByFaculty(string faculty)
    {
        var result = _students.Where(s => s.Faculty==faculty);
        return result;
    }

    public IEnumerable<Student> GetStudentsWithMinAverageGrade(double minAverageGrade)
    {
        var result = _students.Where(s => s.Grades.Average()>=minAverageGrade);
        return result;
    }

    public IEnumerable<Student> GetStudentsOrderedByName()
    {
        var result = _students.OrderBy(s => s.Name);
        return result;
    }

    public ILookup<string, Student> GroupStudentsByFaculty()
    {
        var result = _students.ToLookup(s => s.Faculty);
        return result;
    }

    public string GetFacultyWithHighestAverageGrade()
    {
        var groups = _students.GroupBy(s => s.Faculty);
        var maxAvGrade = groups.MaxBy(g => g.Select(s => s.Grades.Average()).Average());
        var result = maxAvGrade.Key;
        return result;
    }
}
