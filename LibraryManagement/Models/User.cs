using System.ComponentModel.DataAnnotations;

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

        public string Password {get;set;}

        public string Role {get;set;}="User";

        // Navigation property: A User can have many Books
        public ICollection<Book> BorrowedBooks { get; set; } = new List<Book>();
    }
}
