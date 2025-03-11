using LibraryManagement.Data;
using LibraryManagement.DTOs;
using LibraryManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _config;

    public AuthController(AppDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }
    /// <summary>
    /// Register a new user.
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>

    [HttpPost("register")]
    public async Task<IActionResult> Register(AddUserDTO dto)
    {
        if (_context.Users.Any(u => u.Email == dto.Email))
            return BadRequest("Email already exists.");

        var user = new User
        {
            Name = dto.Name,
            Email = dto.Email,
            Password = dto.Password,
            Role = dto.Role ?? "User"  // Default to "User"
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return Ok("User registered.");
    }
    /// <summary>
    /// Login.
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginUserDTO dto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Password == dto.Password);
        if (user == null || !(dto.Password==user.Password))
            return Unauthorized("Invalid credentials.");

        var token = GenerateJwtToken(user);
        return Ok(new { Token = token });
    }
    /// <summary>
    /// Generate JWT token.
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    private string GenerateJwtToken(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var jwtKey = _config["JwtSettings:Key"];
        if (string.IsNullOrEmpty(jwtKey))
        {
            throw new InvalidOperationException("JWT key is not configured.");
        }
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var tokenDescriptor = new JwtSecurityToken(
            issuer: _config["JwtSettings:Issuer"],
            audience: _config["JwtSettings:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
    }
}
