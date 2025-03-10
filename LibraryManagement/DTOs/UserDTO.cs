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

    public class AddUserDTO
    {
        //public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password {get;set;}
        public string? Role{get;set;}
        //public List<BookDTO> BorrowedBooks { get; set; }
    }

    public class GetUserDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    
    }

    public class LoginUserDTO
    {
        public string Name { get; set; }
        public string Password { get; set; }
    
    }
}