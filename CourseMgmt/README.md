# CourseMgmt - .NET Core 8 Clean Architecture Project

A .NET Core 8 Web API for managing courses in a Learning Management System, built following Clean Architecture principles.

## 🏗️ Architecture

This project follows **Clean Architecture** with clear separation of concerns across four layers:

```
CourseMgmt/
├── CourseMgmt.Domain/          # Core business entities, enums, and interfaces
│   ├── Entities/               # Domain entities (Course, User, Category)
│   ├── Enums/                  # Domain enumerations (CourseLevel, CourseStatus)
│   └── Interfaces/             # Repository interfaces
│
├── CourseMgmt.Application/     # Application logic and use cases
│   ├── DTOs/                   # Data Transfer Objects
│   ├── Features/               # CQRS features (Commands & Queries)
│   │   └── Courses/
│   │       ├── Commands/       # Create, Update, Delete
│   │       └── Queries/        # Get, GetList
│   ├── Interfaces/             # Application interfaces (IUnitOfWork)
│   └── DependencyInjection.cs  # DI configuration
│
├── CourseMgmt.Infrastructure/  # Data access and external services
│   ├── Data/                   # DbContext and EF Core configuration
│   ├── Repositories/           # Repository implementations
│   └── UnitOfWork/             # Unit of Work implementation
│
└── CourseMgmt.API/             # Presentation layer
    ├── Controllers/            # API controllers
    ├── Program.cs              # Application startup
    └── appsettings.json       # Configuration
```

## 📦 Technology Stack

- **.NET Core 8.0**
- **Entity Framework Core 8.0** with SQL Server
- **MediatR** - CQRS pattern implementation
- **FluentValidation** - Request validation
- **Swagger/OpenAPI** - API documentation

## 🎯 Features

- ✅ Clean Architecture with separation of concerns
- ✅ CQRS pattern using MediatR
- ✅ Repository pattern with Unit of Work
- ✅ Entity Framework Core with SQL Server
- ✅ FluentValidation for request validation
- ✅ RESTful API endpoints
- ✅ Swagger documentation

## 🚀 Getting Started

### Prerequisites

- .NET 8 SDK
- SQL Server (LocalDB or full instance)
- Visual Studio 2022 or VS Code

### Setup

1. **Update Connection String**

   Edit `CourseMgmt.API/appsettings.json`:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=YOUR_SERVER;Database=CourseMgmtDb;Trusted_Connection=true;TrustServerCertificate=true"
   }
   ```

2. **Create Database Migration**

   ```bash
   cd CourseMgmt.Infrastructure
   dotnet ef migrations add InitialCreate --startup-project ../CourseMgmt.API
   dotnet ef database update --startup-project ../CourseMgmt.API
   ```

3. **Run the API**

   ```bash
   cd CourseMgmt.API
   dotnet run
   ```

4. **Access Swagger UI**

   Navigate to: `https://localhost:5001/swagger` (or the configured port)

## 📡 API Endpoints

### Courses

- `GET /api/v1/courses` - Get paginated list of courses
  - Query parameters: `page`, `limit`, `category`, `status`, `level`, `educator`, `search`
- `GET /api/v1/courses/{id}` - Get course by ID
- `POST /api/v1/courses` - Create a new course
- `PUT /api/v1/courses/{id}` - Update an existing course
- `DELETE /api/v1/courses/{id}` - Delete a course

## 📋 Project Structure Details

### Domain Layer
- **Entities**: Core business entities (Course, User, Category)
- **Enums**: Domain enumerations (CourseLevel, CourseStatus)
- **Interfaces**: Repository contracts

### Application Layer
- **DTOs**: Data Transfer Objects for API communication
- **Features**: CQRS implementation
  - **Commands**: CreateCourse, UpdateCourse, DeleteCourse
  - **Queries**: GetCourse, GetCourses
- **Validators**: FluentValidation validators

### Infrastructure Layer
- **DbContext**: Entity Framework Core database context
- **Repositories**: Data access implementations
- **Unit of Work**: Transaction management

### API Layer
- **Controllers**: RESTful API endpoints
- **Middleware**: CORS, Swagger configuration
- **Configuration**: appsettings.json

## 🔧 Development

### Building the Solution

```bash
dotnet build CourseMgmt.sln
```

### Running Tests

```bash
dotnet test
```

### Adding a New Migration

```bash
cd CourseMgmt.Infrastructure
dotnet ef migrations add MigrationName --startup-project ../CourseMgmt.API
```

## 📝 Notes

- This project mirrors the structure and functionality of the `node-web-api` course module
- All endpoints follow RESTful conventions
- CQRS pattern ensures clear separation between read and write operations
- Repository pattern abstracts data access for testability

## 🔗 Related Projects

- `node-web-api` - Node.js/Express backend (reference implementation)
- `react-web-app` - React frontend application

