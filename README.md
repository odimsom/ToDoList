# ToDoList API

This project is a modular and extensible To-Do List API built with ASP.NET Core (.NET 9) and Entity Framework Core. It is designed with scalability and maintainability in mind, making it easy to add new features and enhancements in the future.

## Features

- RESTful API for managing to-do tasks
- Entity Framework Core with code-first migrations
- Modular architecture with layered persistence and shared infrastructure
- OpenAPI/Swagger documentation for easy API exploration
- CORS support for cross-origin requests

## Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- SQL Server (or update the connection string for your preferred database)

### Setup

1. Clone the repository:
2. Update the `appsettings.json` with your database connection string.
3. Apply database migrations
4. Run the API:


5. Access the Swagger UI at `https://localhost:5001/swagger` (or the port configured in your launch settings).

## Project Structure

- **ToDoList.Core.Domain**: Domain entities and interfaces
- **ToDoList.Core.Application**: Application logic and use cases
- **ToDoList.Infrastructure.Persistence**: Database context, migrations, and repositories
- **ToDoList.Infrastructure.Shared**: Shared infrastructure and dependency injection helpers
- **ToDoList.Presentation.Apis.ToDoListApiDefault**: API project (entry point)

## Contributing

Contributions are welcome! Please open issues or submit pull requests for new features, bug fixes, or improvements.

## Roadmap / Ideas for Future Enhancements

- User authentication and authorization
- Task prioritization and categorization
- Notification system (email, push, etc.)
- Advanced filtering and search
- API versioning
- Unit and integration tests
- Frontend client (web or mobile)

---

*This project is under active development. Feel free to suggest or add new features as needed!*
