# Licentra

Software License Management System

Licentra is a web-based application developed to help organizations manage software licenses, employees, vendors, software inventory, and license assignments from a single platform. It replaces manual spreadsheet-based tracking with a centralized system that improves visibility, security, and accountability.

This project was developed as a final-year engineering project using ASP.NET Core Web API, React, and SQL Server.

---

## Problem Statement

Many organizations still track software licenses manually using spreadsheets or multiple disconnected systems. This makes it difficult to:

- Keep track of purchased licenses
- Monitor license expiry dates
- Manage software assigned to employees
- Identify unused licenses
- Maintain an audit trail of changes
- Control access based on employee roles

Licentra addresses these issues by providing a centralized license management system with secure authentication, role-based access, and automatic audit logging.

---

## Features

### Authentication

- JWT-based authentication
- BCrypt password hashing
- Secure login
- Role-based authorization

### Employee Management

- Add employees
- Update employee details
- Delete employees
- Assign departments
- Manage employee status

### Department Management

- Create departments
- Update department information
- Activate or deactivate departments

### Vendor Management

- Maintain vendor information
- Store contact details
- Associate vendors with software

### Software Management

- Maintain software inventory
- Track software versions
- Link software to vendors
- Subscription support

### License Management

- Store license keys
- Track purchase details
- Monitor expiry dates
- Manage license seats
- License status tracking

### License Assignment

- Assign licenses to employees
- Return assigned licenses
- Prevent duplicate active assignments
- Track assignment history

### User Management

- Create user accounts
- Map users to employees
- Assign roles
- Activate or deactivate users

### Role Management

- Manage system roles
- Restrict access using role-based authorization

### Audit Logging

Every successful Create, Update, and Delete operation is automatically recorded.

Each audit entry stores:

- Logged-in user
- Action performed
- Entity name
- Record ID
- Description
- Timestamp

---

## Technology Stack

| Layer | Technology |
|--------|------------|
| Frontend | React + TypeScript |
| Backend | ASP.NET Core 8 Web API |
| Database | SQL Server |
| ORM | Entity Framework Core |
| Authentication | JWT |
| Password Hashing | BCrypt.Net |
| API Testing | Swagger |
| Version Control | Git & GitHub |

---

## Project Architecture

The project follows a layered architecture.

```
Client (React)

        ¦

        ?

Controllers

        ¦

        ?

Services

        ¦

        ?

Repositories

        ¦

        ?

Entity Framework Core

        ¦

        ?

SQL Server
```

### Layers

**Controllers**

Receive HTTP requests and return API responses.

**Services**

Contain business rules and validations.

**Repositories**

Handle all database operations.

**Entity Framework Core**

Maps application models to SQL Server tables.

---

## Database Modules

The application consists of the following modules:

- Departments
- Employees
- Roles
- Users
- Vendors
- Software
- Licenses
- License Assignments
- Audit Logs

---

## Security

The application includes:

- JWT Authentication
- Password hashing using BCrypt
- Role-based authorization
- Protected API endpoints

---

## Audit Logging

Audit logging is handled automatically inside the service layer.

Whenever a record is created, updated, or deleted, the application stores:

- User performing the action
- Action
- Entity
- Record ID
- Description
- Date and time

No manual audit log entries are required through the API.

---

## Folder Structure

```
Licentra.API
¦
+-- Common
+-- Controllers
+-- Data
+-- DTOs
+-- Exceptions
+-- Interfaces
+-- Middleware
+-- Models
+-- Repositories
+-- Services
+-- Program.cs
+-- appsettings.json
```

---

## Getting Started

### Clone the repository

```bash
git clone https://github.com/<your-username>/Licentra.git
```

### Restore packages

```bash
dotnet restore
```

### Configure the database

Update the SQL Server connection string in:

```
appsettings.json
```

### Run the project

```bash
dotnet run
```

Swagger will be available after the application starts.

---

## API Modules

The backend exposes REST APIs for:

- Authentication
- Employees
- Departments
- Vendors
- Software
- Licenses
- License Assignments
- Users
- Roles
- Audit Logs

---

## Screenshots

Add screenshots of:

- Login Page
- Dashboard
- Employees
- Vendors
- Software
- Licenses
- License Assignments
- Users
- Audit Logs

---

## Future Improvements

Possible enhancements include:

- Email reminders for license expiry
- Dashboard analytics
- Export reports to Excel/PDF
- Multi-factor authentication
- Azure deployment
- Notification system
- Advanced search and filtering

---

## Contributors

Developed by:

- Your Name
- Team Members

Faculty Guide:

- Guide Name

---

## License

This project was developed for academic purposes as part of a Bachelor of Technology final-year project.