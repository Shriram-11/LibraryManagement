📌 Switching to SQLite & Using DTOs in .NET 8 Web API

We'll be building a Library Management System using SQLite and DTOs (Data Transfer Objects) for better data handling.


---

📌 Step 1: Install Required Packages

Run the following command in the terminal to install the necessary dependencies:

dotnet add package Microsoft.EntityFrameworkCore.Sqlite
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Swashbuckle.AspNetCore


---

📌 Step 2: Create the Database (SQLite)

We'll modify appsettings.json to use SQLite.

appsettings.json

{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=library.db"
  }
}


---

📌 Step 3: Define the Models

We'll create Book.cs and User.cs inside the Models/ folder.

📂 Models/Book.cs

using System.ComponentModel.DataAnnotations;

namespace LibraryAPI.Models
{
    public class Book
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Author { get; set; } = string.Empty;

        [Required]
        public bool IsAvailable { get; set; } = true;
    }
}


---

📂 Models/User.cs

using System.ComponentModel.DataAnnotations;

namespace LibraryAPI.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;
    }
}


---

📌 Step 4: Create DTOs

DTOs help in decoupling the database model from the API responses.

📂 DTOs/BookDTO.cs

namespace LibraryAPI.DTOs
{
    public class BookDTO
    {
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
    }
}


---

📂 DTOs/UserDTO.cs

namespace LibraryAPI.DTOs
{
    public class UserDTO
    {
        public string Name { get; set; } = string.Empty;
    }
}


---

📌 Step 5: Configure the Database Context

We'll create AppDbContext.cs in the Data/ folder.

📂 Data/AppDbContext.cs

using Microsoft.EntityFrameworkCore;
using LibraryAPI.Models;

namespace LibraryAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Book> Books { get; set; }
        public DbSet<User> Users { get; set; }
    }
}


---

📌 Step 6: Create the Controllers

We'll have two controllers:

1. BookController → Manages books.


2. UserController → Manages users.




---

📂 Controllers/BookController.cs

using Microsoft.AspNetCore.Mvc;
using LibraryAPI.Data;
using LibraryAPI.Models;
using LibraryAPI.DTOs;
using Microsoft.EntityFrameworkCore;

namespace LibraryAPI.Controllers
{
    [ApiController]
    [Route("api/books")]
    public class BookController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BookController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooks()
        {
            return await _context.Books.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Book>> GetBook(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null) return NotFound();
            return book;
        }

        [HttpPost]
        public async Task<ActionResult<Book>> AddBook(BookDTO bookDto)
        {
            var book = new Book
            {
                Title = bookDto.Title,
                Author = bookDto.Author,
                IsAvailable = true
            };

            _context.Books.Add(book);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetBook), new { id = book.Id }, book);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBook(int id, BookDTO bookDto)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null) return NotFound();

            book.Title = bookDto.Title;
            book.Author = bookDto.Author;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null) return NotFound();

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}


---

📂 Controllers/UserController.cs

using Microsoft.AspNetCore.Mvc;
using LibraryAPI.Data;
using LibraryAPI.Models;
using LibraryAPI.DTOs;
using Microsoft.EntityFrameworkCore;

namespace LibraryAPI.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UserController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<User>> AddUser(UserDTO userDto)
        {
            var user = new User
            {
                Name = userDto.Name
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetUsers), new { id = user.Id }, user);
        }
    }
}


---

📌 Step 7: Configure Program.cs

📂 Program.cs

using LibraryAPI.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();


---

📌 Step 8: Run Migrations & Start API

Run the following commands in the terminal:

dotnet ef migrations add InitialCreate
dotnet ef database update
dotnet run


---

📌 Step 9: Test the API

Open Swagger: https://localhost:5001/swagger/index.html

Use Postman or Swagger UI to:

Add users via POST /api/users

Add books via POST /api/books

Get all books via GET /api/books

Get a specific book via GET /api/books/{id}

Update a book via PUT /api/books/{id}

Delete a book via DELETE /api/books/{id}




---

📌 Summary

✔ Uses SQLite instead of PostgreSQL.
✔ Implements DTOs to separate models from API requests.
✔ Follows Clean Architecture (Controllers, Models, DTOs, DbContext).
✔ CRUD API for Library Management System.

Let me know if you need modifications or explanations! 🚀

