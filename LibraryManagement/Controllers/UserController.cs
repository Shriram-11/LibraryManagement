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

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDTO>> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            return Ok(new GetUserDTO { Id = user.Id, Name = user.Name, Email = user.Email });
        }

        [HttpPost]
        public async Task<IActionResult> AddUser(AddUserDTO dto)
        {
            var book = new User{
                Name=dto.Name,
                Email=dto.Email
            };
            _context.Users.Add(book);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetUser),new {id=book.Id},book);

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
