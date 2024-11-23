# TodoApp

A modern, responsive Todo application built with ASP.NET Core MVC and Bootstrap 5.

## Features

- Responsive design for both mobile and desktop
- Modern UI with gradient design
- User authentication and authorization
- Task management with CRUD operations
- Mobile-first approach with adaptive navigation
- Clean and intuitive interface

## Technologies Used

- ASP.NET Core MVC
- Bootstrap 5
- FontAwesome 6.0.0
- jQuery
- Entity Framework Core
- SQL Server

## Prerequisites

- .NET 6.0 SDK or later
- SQL Server (LocalDB or higher)
- Visual Studio 2022 or VS Code

## Getting Started

1. Clone the repository
```bash
git clone https://github.com/yourusername/TodoApp.git
```

2. Navigate to the project directory
```bash
cd TodoApp
```

3. Restore dependencies
```bash
dotnet restore
```

4. Update database
```bash
dotnet ef database update
```

5. Run the application
```bash
dotnet run
```

## Project Structure

- `TodoApp.Web/` - Main web application
  - `Controllers/` - MVC Controllers
  - `Models/` - Data models and ViewModels
  - `Views/` - Razor views
  - `wwwroot/` - Static files (CSS, JS, images)

## Contributing

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
