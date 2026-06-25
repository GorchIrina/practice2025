using System;
using System.Collections.Generic;
using System.Linq;

public class StudentService
{
    private readonly List<Student> _students;

    public StudentService(List<Student> students) => _students = students;

    public IEnumerable<Student> GetStudentsByFaculty(string faculty)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Student> GetStudentsWithMinAverageGrade(double minAverageGrade)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Student> GetStudentsOrderedByName()
    {
        throw new NotImplementedException();
    }

    public ILookup<string, Student> GroupStudentsByFaculty()
    {
        throw new NotImplementedException();
    }

    public string GetFacultyWithHighestAverageGrade()
    {
        throw new NotImplementedException();
    }
}
