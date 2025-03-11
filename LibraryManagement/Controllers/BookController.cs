using LibraryManagement.Data;
using LibraryManagement.DTOs;
using LibraryManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
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
        /// <summary>
        /// Get all books.
        /// </summary>
        /// <returns></returns>
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
        /// <summary>
        /// Get a book by ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDTO>> GetBook(int id)
        {
            var b = await _context.Books.FindAsync(id);
            if (b == null) return NotFound();

            return Ok(new BookDTO
                {
                    Id = b.Id,
                    Title = b.Title,
                    Author = b.Author,
                    Publisher = b.Publisher,
                    Genre = b.Genre,
                    IsAvailable = b.IsAvailable,
                    UserId = b.UserId
                });
        }
        /// <summary>
        ///     Borrow a book.
        /// </summary>
        /// <param name="bookId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpPut("borrow/{bookId}/user/{userId}")]
        [Authorize]
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

        /// <summary>
        /// Return a borrowed book.
        /// </summary>
        /// <param name="bookId"></param>
        /// <returns></returns>
        [HttpPut("return/{bookId}")]
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
        /// <summary>
        /// Add a new book.
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddBook(AddBookDTO dto)
        {
            var book = new Book{
                    Title=dto.Title,
                    Author = dto.Author,
                    Publisher = dto.Publisher,
                    Genre = dto.Genre,
                    IsAvailable = dto.IsAvailable,
        
            };
            _context.Books.Add(book);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetBooks),new {id=book.Id},book);

        }
        /// <summary>
        /// Remove a book.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
       [HttpDelete("{id}")]
       [Authorize(Roles = "Admin")]
       public async Task<ActionResult> DeleteBook(int id)
       {
            var book = await _context.Books.FindAsync(id);
            if (book == null) return NotFound();

            if (!book.IsAvailable) return BadRequest("Cannot Delete Borrowed Book. It must be returned First");

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
            return NoContent();
       }


    }
}