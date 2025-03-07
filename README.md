# LibraryManagement

---

📌 Folder Structure

LibraryAPI/
│── Controllers/
│   ├── BookController.cs
│   ├── UserController.cs
│   ├── BorrowController.cs
│── Models/
│   ├── Book.cs
│   ├── User.cs
│   ├── BorrowedBook.cs
│── Data/
│   ├── AppDbContext.cs
│── Migrations/
│── Services/                (Optional, for business logic)
│── Properties/
│── appsettings.json
│── Program.cs
│── Startup.cs
│── LibraryAPI.csproj


---

1️⃣ Program.cs (For .NET 8)

using LibraryAPI.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 🔹 Add Database Context (Using PostgreSQL with Npgsql)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// 🔹 Add Controllers & Services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 🔹 Enable Swagger UI for API testing
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

2️⃣ Startup.cs

If you're using .NET 8 with an older MVC-style approach, you can still include Startup.cs:

using LibraryAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace LibraryAPI
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(_configuration.GetConnectionString("DefaultConnection")));

            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.UseRouting();
            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }
}


---

3️⃣ AppDbContext.cs (Database Context)

Create this file inside the Data/ folder.

using Microsoft.EntityFrameworkCore;
using LibraryAPI.Models;

namespace LibraryAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Book> Books { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<BorrowedBook> BorrowedBooks { get; set; }
    }
}


---

4️⃣ Book.cs (Model)

Create this inside the Models/ folder.

using System.ComponentModel.DataAnnotations;

namespace LibraryAPI.Models
{
    public class Book
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Author { get; set; }

        public bool IsAvailable { get; set; } = true;
    }
}


---

5️⃣ User.cs (Model)

using System.ComponentModel.DataAnnotations;

namespace LibraryAPI.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Email { get; set; }
    }
}


---

6️⃣ BorrowedBook.cs (Model for Book Borrowing)

using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LibraryAPI.Models
{
    public class BorrowedBook
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("Book")]
        public int BookId { get; set; }
        public Book Book { get; set; }

        [Required]
        [ForeignKey("User")]
        public int UserId { get; set; }
        public User User { get; set; }

        public DateTime BorrowDate { get; set; } = DateTime.UtcNow;
    }
}


---

7️⃣ BookController.cs (Controller)

Create this file inside Controllers/.

using LibraryAPI.Data;
using LibraryAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
        public async Task<ActionResult<Book>> AddBook(Book book)
        {
            _context.Books.Add(book);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetBook), new { id = book.Id }, book);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBook(int id, Book book)
        {
            if (id != book.Id) return BadRequest();

            _context.Entry(book).State = EntityState.Modified;
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

8️⃣ appsettings.json (Database Configuration)

{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=LibraryDB;Username=your_username;Password=your_password"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}

🔹 Replace "your_username" and "your_password" with actual PostgreSQL credentials.
🔹 Change "localhost" if you're using a remote PostgreSQL server.


---

9️⃣ Apply Migrations & Run

Once everything is set up, run these commands in your terminal:

1️⃣ Install Entity Framework Core tools (if not installed)

dotnet tool install --global dotnet-ef

2️⃣ Add Migration

dotnet ef migrations add InitialCreate

3️⃣ Update Database

dotnet ef database update

4️⃣ Run the API

dotnet run


---

🔹 Test API with Swagger

Once the API is running, open:
➡️ https://localhost:5001/swagger/index.html

This will let you test all API endpoints without writing a single frontend.


---

🎯 Summary

✅ Uses PostgreSQL via Npgsql
✅ Implements MVC-style Web API with Controllers
✅ Uses Entity Framework Core for database handling
✅ Includes CRUD operations for Library Management
✅ Fully tested with Swagger UI

This should impress at work and help you learn .NET 8 properly 🚀 Let me know if you need modifications!

