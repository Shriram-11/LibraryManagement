Got it! We'll refactor everything to use a one-to-many relationship where a User can have multiple Books, and the Book table will contain a UserId field to track who borrowed it.

This approach simplifies the design by removing the BorrowedBook table and directly managing book borrowing within the Book entity.


---

Steps to Set Up the Project

1. Create the .NET 8 Web API Project

dotnet new webapi -n LibraryManagement
cd LibraryManagement


2. Install Entity Framework Core & SQLite Provider

dotnet add package Microsoft.EntityFrameworkCore.Sqlite
dotnet add package Microsoft.EntityFrameworkCore.Design




---

Project Structure

LibraryManagement/
│── Controllers/
│   ├── BooksController.cs
│   ├── UsersController.cs
│── Data/
│   ├── AppDbContext.cs
│── DTOs/
│   ├── BookDTO.cs
│   ├── UserDTO.cs
│── Models/
│   ├── Book.cs
│   ├── User.cs
│── Program.cs
│── appsettings.json
│── LibraryManagement.csproj


---

1️⃣ Models

Book.cs

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryManagement.Models
{
    public class Book
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public string Title { get; set; }

        [Required]
        public string Author { get; set; }

        public string Publisher { get; set; }
        public string Genre { get; set; }
        
        public bool IsAvailable { get; set; } = true;

        // Foreign Key for User (Nullable because the book can be unassigned)
        public int? UserId { get; set; }

        [ForeignKey("UserId")]
        public User? User { get; set; }
    }
}


---

User.cs

using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace LibraryManagement.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Email { get; set; }

        // Navigation property: A User can have many Books
        public ICollection<Book> BorrowedBooks { get; set; } = new List<Book>();
    }
}


---

2️⃣ Data Layer

AppDbContext.cs

using LibraryManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

        public DbSet<Book> Books { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure One-to-Many relationship
            modelBuilder.Entity<User>()
                .HasMany(u => u.BorrowedBooks)
                .WithOne(b => b.User)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}


---

3️⃣ DTOs

BookDTO.cs

namespace LibraryManagement.DTOs
{
    public class BookDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Publisher { get; set; }
        public string Genre { get; set; }
        public bool IsAvailable { get; set; }
        public int? UserId { get; set; }
    }
}


---

UserDTO.cs

using LibraryManagement.Models;
using System.Collections.Generic;

namespace LibraryManagement.DTOs
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public List<BookDTO> BorrowedBooks { get; set; }
    }
}


---

4️⃣ Controllers

BooksController.cs

using LibraryManagement.Data;
using LibraryManagement.DTOs;
using LibraryManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryManagement.Controllers
{
    [Route("api/books")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BooksController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookDTO>>> GetBooks()
        {
            var books = await _context.Books
                .Select(b => new BookDTO
                {
                    Id = b.Id,
                    Title = b.Title,
                    Author = b.Author,
                    Publisher = b.Publisher,
                    Genre = b.Genre,
                    IsAvailable = b.IsAvailable,
                    UserId = b.UserId
                })
                .ToListAsync();

            return Ok(books);
        }

        [HttpPost("borrow/{bookId}/user/{userId}")]
        public async Task<IActionResult> BorrowBook(int bookId, int userId)
        {
            var book = await _context.Books.FindAsync(bookId);
            var user = await _context.Users.FindAsync(userId);

            if (book == null || user == null)
                return NotFound("Book or User not found");

            if (!book.IsAvailable)
                return BadRequest("Book is already borrowed");

            book.UserId = user.Id;
            book.IsAvailable = false;

            await _context.SaveChangesAsync();
            return Ok($"Book '{book.Title}' borrowed by {user.Name}");
        }

        [HttpPost("return/{bookId}")]
        public async Task<IActionResult> ReturnBook(int bookId)
        {
            var book = await _context.Books.FindAsync(bookId);

            if (book == null || book.UserId == null)
                return NotFound("This book is not currently borrowed.");

            book.UserId = null;
            book.IsAvailable = true;

            await _context.SaveChangesAsync();
            return Ok($"Book '{book.Title}' has been returned.");
        }
    }
}


---

UsersController.cs

using LibraryManagement.Data;
using LibraryManagement.DTOs;
using LibraryManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryManagement.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id}/borrowed-books")]
        public async Task<ActionResult<UserDTO>> GetUserWithBorrowedBooks(int id)
        {
            var user = await _context.Users
                .Include(u => u.BorrowedBooks)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null) return NotFound("User not found.");

            return new UserDTO
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                BorrowedBooks = user.BorrowedBooks.Select(b => new BookDTO
                {
                    Id = b.Id,
                    Title = b.Title,
                    Author = b.Author,
                    Publisher = b.Publisher,
                    Genre = b.Genre,
                    IsAvailable = b.IsAvailable,
                    UserId = b.UserId
                }).ToList()
            };
        }
    }
}


---

5️⃣ Migrations & Running

dotnet ef migrations add InitialCreate
dotnet ef database update
dotnet run


---

Now, borrowing and returning books is done directly in the Books table, keeping things simple. Let me know if you need any refinements!

