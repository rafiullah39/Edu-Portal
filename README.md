# 🎓 EduConnect — University Course Management System

A full-stack web application built with **Blazor Server (.NET)** that manages university operations across three role-based portals — Admin, Faculty, and Student. Features real-time UI updates, SOLID design principles, live grade computation, and an in-app notification system.

---

## 📌 Overview

EduConnect replaces manual university administration with a clean, role-aware web portal. Admins manage students and courses, faculty submit grades and view their assigned courses, and students enroll, drop courses, and track their CGPA in real time — all from a single application with no page reloads thanks to Blazor's interactive server rendering.

---

## ✨ Features

### 🔐 Authentication & Role-Based Access
- Login system with three user roles: **Admin**, **Faculty**, **Student**
- Route-level authorization via `AuthGuard` component
- Unauthorized access redirects with clear messaging
- Session state managed via `AuthStateService`

### 🧑‍💼 Admin Portal
- **Dashboard** — live stats: total students, courses, active enrollments, full courses
- **Student Management** — add, edit, search, and delete students with validation
- **Course Management** — create and manage courses with capacity tracking (Open / Almost Full / Full)
- **Grade Reports** — view all submitted grades across all courses
- **Broadcast Notifications** — send announcements to all students or faculty at once

### 👨‍🏫 Faculty Portal
- **My Courses** — view all assigned courses with enrollment counts
- **Grade Submission** — select a course, view enrolled students, submit or update marks
- Automatic CGPA recalculation on the student record upon grade submission
- Notification sent to student automatically when grade is posted

### 🎓 Student Portal
- **Course Enrollment** — browse available courses, enroll or drop with business rule enforcement
- **My Grades** — view grades with letter grade, grade points, and color-coded rows
- **Live CGPA** — automatically recalculated and displayed on dashboard
- **Notifications** — in-app alerts for enrollment changes and grade postings
- Cannot re-enroll in a dropped course within the same semester

---

## 🏗️ Architecture & Design Patterns

This project was built with a strong focus on **SOLID principles** — each principle is explicitly implemented and commented in the codebase:

| Principle | Implementation |
|---|---|
| **SRP** — Single Responsibility | `AuthStateService`, `StudentService`, `CourseService`, `GradeService`, `NotificationService` each handle exactly one concern |
| **OCP** — Open/Closed | `IRepository<T>` generic interface allows new entity types without modifying existing service code |
| **ISP** — Interface Segregation | `IStudentService`, `IGradeService`, `ICourseService`, `INotificationService` are separate — components only depend on what they use |
| **DIP** — Dependency Inversion | All components inject interfaces (`IGradeService`, `ICourseService`), never concrete implementations |
| **IValidatable** | Implemented by `Student` and `Course` — validation logic lives in the model, not the UI |

---

## 🛠️ Tech Stack

| Layer | Technology |
|---|---|
| Framework | Blazor Server (.NET 9 / .NET 10) |
| Language | C# |
| UI | Razor Components + Bootstrap 5 |
| State Management | Scoped services + C# Events |
| Data Storage | In-memory (no database — pure OOP) |
| Authentication | Custom session-based `AuthStateService` |
| Notifications | Event-driven `NotificationService` |

---

## 📁 Project Structure

```
EduConnect/
├── Components/
│   ├── App.razor               # Root component
│   ├── MainLayout.razor        # Shared layout with navigation
│   ├── Routes.razor            # Route definitions
│   └── _Imports.razor          # Global using statements
├── Pages/
│   ├── Login.razor             # Authentication page
│   ├── Dashboard.razor         # Role-aware home screen
│   ├── AdminCourses.razor      # Admin course management
│   ├── AdminExtra.razor        # Admin notifications & reports
│   ├── FacultyGrades.razor     # Faculty courses & grade submission
│   ├── StudentPages.razor      # Student enrollment & grades
│   ├── StudentList.razor       # Admin student list view
│   ├── StudentForm.razor       # Add/edit student form
│   ├── CourseCard.razor        # Reusable course display card
│   ├── AlertBox.razor          # Reusable alert component
│   ├── AuthGuard.razor         # Route protection component
│   └── ConfirmDialog.razor     # Reusable confirm dialog
├── Models.cs                   # All domain models (Person, Student, Faculty, Course, GradeRecord, etc.)
├── Interfaces.cs               # All service interfaces (IRepository<T>, IStudentService, IGradeService, etc.)
├── Enum.cs                     # All enumerations (UserRole, EnrollmentState, etc.)
├── DataServices.cs             # AuthStateService, UserStore, StudentService, CourseService
├── Gradenotifyservices.cs      # GradeService, NotificationService
└── appsettings.json            # App configuration
```

---

## 📊 Data Models

```
Person (abstract)
├── Admin
├── Faculty          → AssignedCourseIds[]
└── Student          → Semester, CGPA, Enrollments[]

Course              → Code, Title, CreditHours, MaxCapacity, EnrollmentStatus
Enrollment          → StudentId, CourseId, Semester, EnrollmentState
GradeRecord         → Marks, LetterGrade (auto), GradePoints (auto), CGPA contribution
Notification        → UserId, Message, NotificationType, IsRead
```

---

## ⚙️ Setup & Installation

### Prerequisites
- [.NET 9 SDK](https://dotnet.microsoft.com/download) or higher
- Visual Studio 2022 / VS Code with C# extension

### Steps

1. **Clone the repository**
```bash
git clone https://github.com/rafiullah39/EduConnect.git
cd EduConnect
```

2. **Restore dependencies**
```bash
dotnet restore
```

3. **Run the application**
```bash
dotnet run
```

4. **Open in browser**
```
https://localhost:5001
```

### Demo Login Credentials

| Role | Email | Password |
|---|---|---|
| Admin | admin@edu.com | admin |
| Faculty | obaid@edu.com | obaid123 |
| Student | 241926@students.au.edu.pk | pass926 |
| Student | 241938@students.au.edu.pk | pass938 |

---

## 🔔 Business Rules Enforced

- Students cannot enroll in a course that is at full capacity (`CourseFullException`)
- Students cannot re-enroll in a course they dropped in the same semester
- Students with active enrollments cannot be deleted (`StudentHasActiveEnrollmentsException`)
- Student email must follow university format: `xxxxxx@students.au.edu.pk`
- CGPA recalculates automatically on every grade submission
- Grade color coding: green (≥70), yellow (45–69), red (<45)
- Enrollment status auto-updates: Open → Almost Full (80%) → Full (100%)

---

## 🎓 Academic Context

Developed as a semester project for **Visual Programming (CS-284)** at university level. Demonstrates Blazor Server architecture, component-based UI development, event-driven state management, generic repository patterns, and full implementation of SOLID design principles in C#.

---

## 👨‍💻 Author

**Rafi Khan** — CS Student
GitHub: [@rafiullah39](https://github.com/rafiullah39)
Fiverr: [@rafii_codes](https://fiverr.com/rafii_codes)
