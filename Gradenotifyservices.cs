using EduConnect.Interfaces;
using EduConnect.Models;

namespace EduConnect.Services
{
    // SRP: Only manages grade records and CGPA computation
    public class GradeService : IGradeService
    {
        private readonly List<GradeRecord> _grades = new();
        private readonly IStudentService _students;
        public event Action? OnGradesSubmitted;

        public GradeService(IStudentService students) { _students = students; }

        public void SubmitGrade(Guid studentId, Guid courseId, double marks, string studentName, string courseTitle, int creditHours)
        {
            var existing = _grades.FirstOrDefault(g => g.StudentId == studentId && g.CourseId == courseId);
            if (existing != null)
            {
                existing.Marks = marks;
            }
            else
            {
                _grades.Add(new GradeRecord
                {
                    StudentId = studentId,
                    CourseId = courseId,
                    StudentName = studentName,
                    CourseTitle = courseTitle,
                    CreditHours = creditHours,
                    Marks = marks
                });
            }

            // Update CGPA on student record immediately
            var student = _students.GetById(studentId);
            if (student != null) student.CGPA = ComputeCGPA(studentId);

            OnGradesSubmitted?.Invoke();
        }

        public List<GradeRecord> GetGradesForStudent(Guid studentId) =>
            _grades.Where(g => g.StudentId == studentId).ToList();

        public List<GradeRecord> GetGradesForCourse(Guid courseId) =>
            _grades.Where(g => g.CourseId == courseId).ToList();

        public List<GradeRecord> GetAllGrades() => _grades.ToList();

        public double ComputeCGPA(Guid studentId)
        {
            var records = _grades.Where(g => g.StudentId == studentId && g.Marks >= 0).ToList();
            if (!records.Any()) return 0.0;
            double points = records.Sum(g => g.GradePoints * g.CreditHours);
            int credits = records.Sum(g => g.CreditHours);
            return credits == 0 ? 0.0 : Math.Round(points / credits, 2);
        }
    }

    // SRP: Only manages notification creation and delivery
    public class NotificationService : INotificationService
    {
        private readonly List<Notification> _notifications = new();

        // Components subscribe in OnInitialized, unsubscribe in IDisposable.Dispose
        public event Action<Notification>? OnNewNotification;

        public void Notify(Guid userId, string message, NotificationType type)
        {
            var n = new Notification { UserId = userId, Message = message, Type = type };
            _notifications.Add(n);
            OnNewNotification?.Invoke(n);
        }

        public void BroadcastToRole(UserRole role, string message, List<Person> allUsers)
        {
            foreach (var user in allUsers.Where(u => u.GetRole() == role))
                Notify(user.Id, message, NotificationType.Announcement);
        }

        public List<Notification> GetForUser(Guid userId) =>
            _notifications.Where(n => n.UserId == userId)
                          .OrderByDescending(n => n.CreatedAt).ToList();

        public void MarkAsRead(Guid notificationId)
        {
            var n = _notifications.FirstOrDefault(x => x.Id == notificationId);
            if (n != null) n.IsRead = true;
        }

        public int GetUnreadCount(Guid userId) =>
            _notifications.Count(n => n.UserId == userId && !n.IsRead);
    }
}