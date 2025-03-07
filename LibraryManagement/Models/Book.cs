using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Models;

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
}