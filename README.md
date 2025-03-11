# Library Management System

This is a .NET Web API project for managing a library system. It allows users to manage books, users, and their borrowed books.

## Technologies Used

- .NET Core
- Entity Framework Core
- ASP.NET Core MVC
- Microsoft SQL Server

## Getting Started

### Prerequisites

- .NET Core SDK
- SQL Server

### Installation

1. Clone the repository:
    ```bash
    git clone https://github.com/yourusername/LibraryManagement.git
    cd LibraryManagement
    ```

2. Set up the database:
    - Update the connection string in `appsettings.json` to point to your SQL Server instance.
    - Run the following command to apply migrations and create the database:
        ```bash
        dotnet ef database update
        ```

3. Run the application:
    ```bash
    dotnet run
    ```

## Contributing

Contributions are welcome! Please open an issue or submit a pull request for any changes.

## License

This project is licensed under the MIT License.
