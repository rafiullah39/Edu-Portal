using EduConnect.Interfaces;
using EduConnect.Models;

namespace EduConnect.Services
{
    // SRP: Only manages authentication state
    public class AuthStateService
    {
        public Person? CurrentUser { get; private set; }
        public event Action? OnAuthChanged;

        public void Login(Person user)
        {
            CurrentUser = user;
            OnAuthChanged?.Invoke(); // fires login event
        }

        public void Logout()
        {
            CurrentUser = null;
            OnAuthChanged?.Invoke(); // fires logout event
        }

        public bool IsLoggedIn => CurrentUser != null;
    }

    // SRP: Only manages in-memory user store (seed data)
    public class UserStore
    {
        private readonly List<Person> _users;

        public UserStore()
        {
            _users = new List<Person>
            {
                new Admin   { FullName = "Admin User",   Email = "admin@edu.com",  Password = "admin" },
               new Faculty {
                          FullName = "Hafiz Obaid Ullah",
                          Email = "obaid@edu.com",
                          Password = "obaid123",
                          AssignedCourseIds = new List<Guid> {
                                Guid.Parse("aaaaaaaa-0000-0000-0000-000000000001"), 
                                Guid.Parse("aaaaaaaa-0000-0000-0000-000000000002"), 
                                Guid.Parse("aaaaaaaa-0000-0000-0000-000000000003"), 
                                Guid.Parse("aaaaaaaa-0000-0000-0000-000000000004")  
                            }
                        },
                new Student { FullName = "Student 241938",   Email = "241938@students.au.edu.pk",  Password = "pass938", Semester = 4 },
                new Student { FullName = "Student 241797",   Email = "241797@students.au.edu.pk",  Password = "pass797", Semester = 3 },
                new Student { FullName = "Student 241926",   Email = "241926@students.au.edu.pk",  Password = "pass926", Semester = 4 },
            };
        }

        public Person? FindUser(string email, string password) =>
            _users.FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase)
                                    && u.Password == password);

        public List<Person> GetAll() => _users.ToList();
        public List<Student> GetAllStudents() => _users.OfType<Student>().ToList();
        public List<Faculty> GetAllFaculty() => _users.OfType<Faculty>().ToList();
    }

    // SRP: Only manages student data — no grade logic (DIP via IStudentService)
    public class StudentService : IStudentService
    {
        private readonly List<Student> _students;

        public StudentService(UserStore store)
        {
            _students = store.GetAllStudents();
        }

        public Student? GetById(Guid id) => _students.FirstOrDefault(s => s.Id == id);

        public List<Student> GetAll() => _students.ToList();

        public List<Student> Search(string term) =>
            string.IsNullOrWhiteSpace(term)
                ? _students.ToList()
                : _students.Where(s =>
                    s.FullName.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                    s.Email.Contains(term, StringComparison.OrdinalIgnoreCase)).ToList();

        public void Add(Student s) => _students.Add(s);

        public void Update(Student s)
        {
            var idx = _students.FindIndex(x => x.Id == s.Id);
            if (idx >= 0) _students[idx] = s;
        }

        public void Delete(Guid id)
        {
            var s = GetById(id);
            if (s == null) return;
            // Custom exception: cannot delete student with active enrollments
            if (s.Enrollments.Any(e => e.State == EnrollmentState.Active))
                throw new StudentHasActiveEnrollmentsException();
            _students.RemoveAll(x => x.Id == id);
        }
    }

    // SRP: Only manages course data and enrollment logic
    public class CourseService : ICourseService
    {
        private readonly List<Course> _courses;
        public event Action? OnEnrollmentChanged;

        public CourseService()
        {
            // Seed demo courses
            // In CourseService constructor, replace the seed with this:
            _courses = new List<Course>
{
    new Course
    {
        Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000001"),
        Code = "CS-101", Title = "Introduction to Programming",
        CreditHours = 3, MaxCapacity = 30
    },
    new Course
    {
        Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000002"),
        Code = "CS-284", Title = "Visual Programming",
        CreditHours = 3, MaxCapacity = 25
    },
    new Course
    {
        Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000003"),
        Code = "CS-201", Title = "Data Structures",
        CreditHours = 3, MaxCapacity = 20
    },
    new Course
    {
        Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000004"),
        Code = "MT-101", Title = "Calculus I",
        CreditHours = 3, MaxCapacity = 35
    },
};
        }

        public Course? GetById(Guid id) => _courses.FirstOrDefault(c => c.Id == id);
        public List<Course> GetAll() => _courses.ToList();
        public List<Course> GetCoursesForFaculty(Guid facultyId) =>
            _courses.Where(c => c.AssignedFacultyId == facultyId).ToList();

        public void Add(Course c) => _courses.Add(c);

        public void Update(Course c)
        {
            var idx = _courses.FindIndex(x => x.Id == c.Id);
            if (idx >= 0) _courses[idx] = c;
        }

        public void Delete(Guid id) => _courses.RemoveAll(c => c.Id == id);

        public void EnrollStudent(Guid studentId, Student student, Guid courseId)
        {
            var course = GetById(courseId) ?? throw new Exception("Course not found.");
            if (course.Status == EnrollmentStatus.Full) throw new CourseFullException();

            // Business rule: cannot re-enroll in dropped course same semester (enforced in service, not component)
            bool alreadyDropped = student.Enrollments.Any(e => e.CourseId == courseId
                && e.Semester == student.Semester && e.State == EnrollmentState.Dropped);
            if (alreadyDropped) throw new Exception("Cannot re-enroll in a dropped course in the same semester.");

            bool alreadyActive = student.Enrollments.Any(e => e.CourseId == courseId && e.State == EnrollmentState.Active);
            if (alreadyActive) throw new Exception("Already enrolled in this course.");

            course.EnrolledStudentIds.Add(studentId);
            student.Enrollments.Add(new Enrollment
            {
                StudentId = studentId,
                CourseId = courseId,
                CourseTitle = course.Title,
                Semester = student.Semester,
                State = EnrollmentState.Active
            });

            OnEnrollmentChanged?.Invoke();
        }

        public void DropCourse(Guid studentId, Guid courseId)
        {
            var course = GetById(courseId);
            if (course != null) course.EnrolledStudentIds.Remove(studentId);
            OnEnrollmentChanged?.Invoke();
        }
    }
}