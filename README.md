Here's a README template for your **Komnu API** GitHub repository, focusing on installation, setup, and key features for the backend:

---

# Komnu API

**Komnu API** is the backend service for the Komnu social networking platform, built using C# and ASP.NET Core. It provides API endpoints for user management, posts, messaging, and more to support a dynamic social networking experience.

## Features

- **User Authentication**: Supports secure login, registration, and Two-Factor Authentication (2FA) with JWT tokens.
- **Profile Management**: CRUD operations for user profiles.
- **Friendship System**: Endpoints for sending, accepting, and managing friend requests.
- **Posts and Comments**: API for creating, viewing, and commenting on posts.
- **Messaging**: Real-time messaging functionality.
- **Events and Groups**: Support for event and group management.
- **Settings**: User-specific settings and notifications.

## Technologies Used

- **Framework**: ASP.NET Core
- **Database**: Microsoft SQL Server
- **ORM**: Entity Framework Core
- **Authentication**: JWT Tokens with optional Two-Factor Authentication (2FA)

## Installation and Setup

### Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/download) installed
- Microsoft SQL Server set up and running
- (Optional) Postman or another API testing tool

### Steps

1. **Clone the repository**:
   ```bash
   git clone https://github.com/Vados1789/komnu_api.git
   cd komnu_api
   ```

2. **Configure Database Connection**:
   - Update the database connection string in `appsettings.json` to point to your SQL Server instance.

3. **Apply Migrations**:
   - Run the following command to apply migrations and create the database:
     ```bash
     dotnet ef database update
     ```

4. **Run the API**:
   ```bash
   dotnet run
   ```

5. **Access the API Documentation**:
   - Once running, you can view API endpoints and test them through Swagger UI at `http://localhost:5000/swagger`.

## Environment Variables

- `JWT_SECRET`: Secret key for JWT token generation.
- `DATABASE_URL`: Connection string for SQL Server.
- `API_BASE_URL`: Base URL for the API (default is `http://localhost:5000`).

## Project Structure

```
komnu_api
├── Controllers          # API controllers for handling requests
├── Models               # Entity models
├── Data                 # Database context and migrations
├── Services             # Business logic and services
├── DTOs                 # Data Transfer Objects for requests and responses
├── Middleware           # Custom middleware (e.g., for error handling)
├── Program.cs           # Program entry point
└── Startup.cs           # Configurations and service registration
```

## Testing

- Use `dotnet test` to run unit tests for the API.
- API testing can be done via Postman or Swagger UI at `http://localhost:5000/swagger`.

## License

This project is licensed under the MIT License.

---
