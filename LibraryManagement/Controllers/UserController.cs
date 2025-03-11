using LibraryManagement.Data;
using LibraryManagement.DTOs;
using LibraryManagement.Models;
using Microsoft.AspNetCore.Authorization;
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

        /// <summary>
        /// Get all users.
        /// </summary>
        /// <returns>List of users.</returns>
        [HttpGet]
        public async Task<ActionResult<GetUserDTO>> GetAll()
        {
            var users = await _context.Users
                .Select(b => new GetUserDTO
                {
                    Id = b.Id,
                    Name = b.Name,
                    Email = b.Email,
                    // BorrowedBooks = b.BorrowedBooks;
                })
                .ToListAsync();

            return Ok(users);
        }

        /// <summary>
        /// Get a user by ID.
        /// </summary>
        /// <param name="id">User ID.</param>
        /// <returns>User details.</returns>
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<UserDTO>> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            return Ok(new GetUserDTO { Id = user.Id, Name = user.Name, Email = user.Email });
        }

        /*
        /// <summary>
        /// Add a new user.
        /// </summary>
        /// <param name="dto">User data transfer object.</param>
        /// <returns>Action result.</returns>
        [HttpPost]
        public async Task<IActionResult> AddUser(AddUserDTO dto)
        {
            var user = new User{
                Name = dto.Name,
                Email = dto.Email
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        }
        */

        /// <summary>
        /// Get a user with their borrowed books.
        /// </summary>
        /// <param name="id">User ID.</param>
        /// <returns>User details with borrowed books.</returns>
        [HttpGet("{id}/borrowed-books")]
        [Authorize]
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

        /// <summary>
        /// Delete a user by ID.
        /// </summary>
        /// <param name="id">User ID.</param>
        /// <returns>Action result.</returns>
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            var hasBorrowedBooks = await _context.Books.AnyAsync(b => b.UserId == id && !b.IsAvailable);
            if (hasBorrowedBooks) return BadRequest("Must return all books before closing account");

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
