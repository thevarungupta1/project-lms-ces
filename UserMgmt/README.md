# UserMgmt - User Management Service

A .NET Core 8 Clean Architecture microservice for user authentication and management, following the same structure as CourseMgmt.

## 📁 Project Structure

```
UserMgmt/
├── UserMgmt.Domain/          # Domain layer
│   ├── Common/
│   │   └── BaseEntity.cs
│   ├── Entities/
│   │   └── User.cs
│   ├── Enums/
│   │   └── UserRole.cs
│   └── Exceptions/
│       ├── NotFoundException.cs
│       ├── BusinessRuleException.cs
│       ├── ValidationException.cs
│       └── UnauthorizedException.cs
│
├── UserMgmt.Application/     # Application layer
│   ├── DTOs/
│   │   ├── RegisterDto.cs
│   │   ├── LoginDto.cs
│   │   ├── AuthResponseDto.cs
│   │   ├── UserDto.cs
│   │   └── RefreshTokenDto.cs
│   ├── Interfaces/
│   │   ├── IRepository.cs
│   │   ├── IUserRepository.cs
│   │   ├── IUserService.cs
│   │   └── IJwtService.cs
│   └── Services/
│       ├── UserService.cs
│       └── JwtService.cs
│
├── UserMgmt.Infrastructure/  # Infrastructure layer
│   ├── Persistence/
│   │   └── ApplicationDbContext.cs
│   └── Repositories/
│       ├── Repository.cs
│       └── UserRepository.cs
│
└── UserMgmt.API/             # API layer
    ├── Controllers/
    │   └── AuthController.cs
    └── Middleware/
        └── ExceptionHandlingMiddleware.cs
```

## 🚀 Features

- **User Registration**: Create new user accounts with role assignment
- **User Login**: Authenticate users with email and password
- **JWT Authentication**: Secure token-based authentication
- **Refresh Tokens**: Long-lived refresh tokens for seamless authentication
- **Password Hashing**: SHA256 password hashing for security
- **Role-Based Access**: Support for Admin, Educator, and Learner roles
- **Exception Handling**: Global exception handling middleware
- **Clean Architecture**: Proper layer separation and dependency management

## 🛠️ Technologies

- **.NET 8.0**: Latest LTS framework
- **Entity Framework Core 8.0**: ORM for data access
- **SQL Server**: Database
- **JWT Bearer Authentication**: Token-based authentication
- **Swagger/OpenAPI**: API documentation

## 📋 Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (or SQL Server LocalDB)

## 🔧 Setup

1. **Navigate to the project directory**:
   ```bash
   cd UserMgmt
   ```

2. **Update Connection String**:
   Open `UserMgmt.API/appsettings.json` and ensure the `DefaultConnection` string points to your SQL Server instance.

3. **Apply Migrations**:
   ```bash
   cd UserMgmt.Infrastructure
   dotnet ef migrations add InitialCreate --startup-project ../UserMgmt.API
   dotnet ef database update --startup-project ../UserMgmt.API
   ```

4. **Run the API**:
   ```bash
   cd ../UserMgmt.API
   dotnet run
   ```

   The API will typically run on `https://localhost:7002` (or a similar port). Swagger UI will be available at `/swagger`.

## 📡 API Endpoints

### Authentication

#### Register User
```
POST /api/v1/auth/register
Content-Type: application/json

{
  "name": "John Doe",
  "email": "john.doe@example.com",
  "password": "password123",
  "department": "Engineering",
  "role": 3  // 1=Admin, 2=Educator, 3=Learner
}
```

#### Login
```
POST /api/v1/auth/login
Content-Type: application/json

{
  "email": "john.doe@example.com",
  "password": "password123"
}
```

#### Refresh Token
```
POST /api/v1/auth/refresh-token
Content-Type: application/json

{
  "refreshToken": "your-refresh-token-here"
}
```

#### Get Current User
```
GET /api/v1/auth/me
Authorization: Bearer {access-token}
```

## 🔐 JWT Configuration

JWT settings are configured in `appsettings.json`:

```json
{
  "Jwt": {
    "SecretKey": "YourSuperSecretKeyThatShouldBeAtLeast32CharactersLong!",
    "Issuer": "UserMgmt",
    "Audience": "UserMgmt",
    "AccessTokenExpiryMinutes": "60",
    "RefreshTokenExpiryDays": "7"
  }
}
```

**⚠️ Important**: Change the `SecretKey` in production to a secure, randomly generated key.

## 🏗️ Architecture

The project follows Clean Architecture principles:

- **Domain Layer**: Core business entities, enums, and exceptions (no dependencies)
- **Application Layer**: Business logic, DTOs, service interfaces and implementations
- **Infrastructure Layer**: Data access, repository implementations, EF Core
- **API Layer**: Controllers, middleware, HTTP request/response handling

## 📝 User Roles

- **Admin (1)**: System administrators
- **Educator (2)**: Course instructors
- **Learner (3)**: Students/learners

## 🔒 Security Features

- Password hashing using SHA256
- JWT token-based authentication
- Refresh token mechanism
- Role-based authorization
- Global exception handling
- Input validation

## 🧪 Testing

To test the API:

1. Start the API
2. Navigate to `/swagger` for interactive API documentation
3. Use Swagger UI to test endpoints
4. Register a new user
5. Login with credentials
6. Use the access token for authenticated requests

## 📚 Related Projects

- **CourseMgmt**: Course management service (same architecture)

## 🤝 Contributing

Contributions are welcome! Please follow the existing architectural patterns and coding standards.

