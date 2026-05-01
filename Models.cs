using EduConnect.Interfaces;
using System.Text.RegularExpressions;

namespace EduConnect.Models
{
    public abstract class Person
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string FullName { get; set; } = "";
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
        public abstract UserRole GetRole();
    }

    public class Student : Person, IValidatable
    {
        public int Semester { get; set; } = 1;
        public double CGPA { get; set; } = 0.0;
        public List<Enrollment> Enrollments { get; set; } = new();
        public override UserRole GetRole() => UserRole.Student;

        public Dictionary<string, string> Validate()
        {
            var errors = new Dictionary<string, string>();

            if (string.IsNullOrWhiteSpace(FullName))
                errors["FullName"] = "Name is required.";

            if (string.IsNullOrWhiteSpace(Email))
            {
                errors["Email"] = "Email is required.";
            }
            else
            {
                var emailPattern = @"^\d{6}@students\.au\.edu\.pk$";
                if (!Regex.IsMatch(Email, emailPattern))
                    errors["Email"] = "Email must be in format: xxxxxx@students.au.edu.pk";
            }

            if (Semester < 1 || Semester > 8)
                errors["Semester"] = "Semester must be 1–8.";

            if (CGPA < 0 || CGPA > 4)
                errors["CGPA"] = "CGPA must be 0–4.";

            return errors;
        }
    }

    public class Faculty : Person
    {
        public List<Guid> AssignedCourseIds { get; set; } = new();
        public override UserRole GetRole() => UserRole.Faculty;
    }

    public class Admin : Person
    {
        public override UserRole GetRole() => UserRole.Admin;
    }

    public class Course : IValidatable
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Code { get; set; } = "";
        public string Title { get; set; } = "";
        public int CreditHours { get; set; } = 3;
        public int MaxCapacity { get; set; } = 30;
        public Guid? AssignedFacultyId { get; set; }
        public List<Guid> EnrolledStudentIds { get; set; } = new();

        public EnrollmentStatus Status =>
            EnrolledStudentIds.Count >= MaxCapacity ? EnrollmentStatus.Full :
            EnrolledStudentIds.Count >= MaxCapacity * 0.8 ? EnrollmentStatus.AlmostFull :
            EnrollmentStatus.Open;

        public int EnrollmentCount => EnrolledStudentIds.Count;

        public Dictionary<string, string> Validate()
        {
            var errors = new Dictionary<string, string>();
            if (string.IsNullOrWhiteSpace(Code))
                errors["Code"] = "Course code is required.";
            if (string.IsNullOrWhiteSpace(Title))
                errors["Title"] = "Title is required.";
            if (CreditHours < 1 || CreditHours > 4)
                errors["CreditHours"] = "Credit hours must be 1–4.";
            if (MaxCapacity < 1)
                errors["MaxCapacity"] = "Capacity must be at least 1.";
            return errors;
        }
    }

    public class Enrollment
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid StudentId { get; set; }
        public Guid CourseId { get; set; }
        public string CourseTitle { get; set; } = "";
        public int Semester { get; set; }
        public EnrollmentState State { get; set; } = EnrollmentState.Active;
    }

    public class GradeRecord
    {
        public Guid StudentId { get; set; }
        public Guid CourseId { get; set; }
        public string StudentName { get; set; } = "";
        public string CourseTitle { get; set; } = "";
        public int CreditHours { get; set; }
        public double Marks { get; set; } = -1;

        public string LetterGrade => Marks >= 85 ? "A" : Marks >= 70 ? "B" : Marks >= 55 ? "C" : Marks >= 45 ? "D" : "F";
        public double GradePoints => Marks >= 85 ? 4.0 : Marks >= 70 ? 3.0 : Marks >= 55 ? 2.0 : Marks >= 45 ? 1.0 : 0.0;
        public string RowCss => Marks >= 70 ? "table-success" : Marks >= 45 ? "table-warning" : "table-danger";
    }

    public class Notification
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        public string Message { get; set; } = "";
        public NotificationType Type { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }

    public class StudentHasActiveEnrollmentsException : Exception
    {
        public StudentHasActiveEnrollmentsException() : base("Cannot delete student with active enrollments.") { }
    }

    public class CourseFullException : Exception
    {
        public CourseFullException() : base("This course is full and cannot accept more students.") { }
    }
}