using EduConnect.Models;

namespace EduConnect.Interfaces
{
    // ISP: IValidatable is separate — only implemented by classes that need validation
    public interface IValidatable
    {
        Dictionary<string, string> Validate();
    }

    // OCP: IRepository<T> allows new entity types without modifying existing repo code
    public interface IRepository<T>
    {
        T? GetById(Guid id);
        List<T> GetAll();
        void Add(T entity);
        void Update(T entity);
        void Delete(Guid id);
    }

    // ISP: IStudentService extends IRepository<Student> but does NOT include grade methods
    public interface IStudentService : IRepository<Student>
    {
        List<Student> Search(string term);
    }

    // ISP: Grade methods belong here, not in IStudentService
    public interface IGradeService
    {
        void SubmitGrade(Guid studentId, Guid courseId, double marks, string studentName, string courseTitle, int creditHours);
        List<GradeRecord> GetGradesForStudent(Guid studentId);
        List<GradeRecord> GetGradesForCourse(Guid courseId);
        List<GradeRecord> GetAllGrades();
        double ComputeCGPA(Guid studentId);
        event Action? OnGradesSubmitted;
    }

    public interface ICourseService : IRepository<Course>
    {
        void EnrollStudent(Guid studentId, Student student, Guid courseId);
        void DropCourse(Guid studentId, Guid courseId);
        List<Course> GetCoursesForFaculty(Guid facultyId);
        event Action? OnEnrollmentChanged;
    }

    public interface INotificationService
    {
        event Action<Notification>? OnNewNotification;
        void Notify(Guid userId, string message, NotificationType type);
        void BroadcastToRole(UserRole role, string message, List<Person> allUsers);
        List<Notification> GetForUser(Guid userId);
        void MarkAsRead(Guid notificationId);
        int GetUnreadCount(Guid userId);
    }
}
