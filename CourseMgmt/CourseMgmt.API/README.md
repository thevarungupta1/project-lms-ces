# CourseMgmt API

A .NET Core 8 Web API for managing courses in a Learning Management System, built with Clean Architecture principles.

## Architecture

This project follows Clean Architecture with the following layers:

- **CourseMgmt.Domain**: Core business entities, enums, and interfaces
- **CourseMgmt.Application**: Application logic, DTOs, use cases (CQRS with MediatR), and validation
- **CourseMgmt.Infrastructure**: Data access (Entity Framework Core), repositories, and external services
- **CourseMgmt.API**: Controllers, middleware, and API configuration

## Features

- Clean Architecture with separation of concerns
- CQRS pattern using MediatR
- Entity Framework Core with SQL Server
- FluentValidation for request validation
- Repository pattern with Unit of Work
- RESTful API endpoints

## Prerequisites

- .NET 8 SDK
- SQL Server (LocalDB or full instance)

## Setup

1. Update the connection string in `appsettings.json`:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER;Database=CourseMgmtDb;Trusted_Connection=true;"
}
```

2. Create and apply migrations:
```bash
cd CourseMgmt.Infrastructure
dotnet ef migrations add InitialCreate --startup-project ../CourseMgmt.API
dotnet ef database update --startup-project ../CourseMgmt.API
```

3. Run the API:
```bash
cd CourseMgmt.API
dotnet run
```

## API Endpoints

### Courses

- `GET /api/v1/courses` - Get paginated list of courses
- `GET /api/v1/courses/{id}` - Get course by ID
- `POST /api/v1/courses` - Create a new course
- `PUT /api/v1/courses/{id}` - Update an existing course
- `DELETE /api/v1/courses/{id}` - Delete a course

## Swagger

Once running, access Swagger UI at: `https://localhost:5001/swagger` (or the configured port)

