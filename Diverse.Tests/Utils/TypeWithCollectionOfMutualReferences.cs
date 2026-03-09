using System.Collections.Generic;

namespace Diverse.Tests.Utils
{
    /// <summary>
    /// Two types that reference each other via collections.
    /// Tests the worst-case scenario: mutual references through collections.
    /// </summary>
    public class Teacher
    {
        public string Name { get; }
        public IEnumerable<Student> Students { get; }

        public Teacher(string name, IEnumerable<Student> students)
        {
            Name = name;
            Students = students;
        }
    }

    public class Student
    {
        public string Name { get; }
        public IEnumerable<Teacher> Teachers { get; }

        public Student(string name, IEnumerable<Teacher> teachers)
        {
            Name = name;
            Teachers = teachers;
        }
    }
}
