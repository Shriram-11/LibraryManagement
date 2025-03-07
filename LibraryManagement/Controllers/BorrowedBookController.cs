using LibraryManagement.Data;
using LibraryManagement.DTOs;
using LibraryManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryManagement.Controllers
{
    [ApiController]
    [Route("api/borrowed-books")]
    public class BorrowedBooksController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BorrowedBooksController(AppDbContext context)
        {
            _context = context;
        }

        // Borrow a book
        [HttpPost("borrow")]
        public async Task<IActionResult> BorrowBook(BorrowBookDTO dto)
        {
            var book = await _context.Books.FindAsync(dto.BookId);
            var user = await _context.Users.FindAsync(dto.UserId);

            if (book == null || user == null) return NotFound();
            if (!book.IsAvailable) return BadRequest("Book is already borrowed");

            var borrowedBook = new BorrowedBook { Book = book, User = user };
            book.IsAvailable = false;

            _context.BorrowedBooks.Add(borrowedBook);
            await _context.SaveChangesAsync();
            return Ok();
        }

        // Return a book
        [HttpPost("return/{id}")]
        public async Task<IActionResult> ReturnBook(int id)
        {
            var borrowedBook = await _context.BorrowedBooks.Include(bb => bb.Book)
                .FirstOrDefaultAsync(bb => bb.Id == id && !bb.IsReturned);

            if (borrowedBook == null) return NotFound();
            
            borrowedBook.IsReturned = true;
            borrowedBook.ReturnDate = DateTime.UtcNow;
            borrowedBook.Book.IsAvailable = true;

            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
