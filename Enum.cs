namespace EduConnect.Models
{
    public enum UserRole { Admin, Faculty, Student }
    public enum EnrollmentState { Active, Dropped, Completed }
    public enum EnrollmentStatus { Open, AlmostFull, Full }
    public enum NotificationType { Enrollment, GradePosted, Announcement }
    public enum AlertType { Success, Warning, Error, Info }
}
