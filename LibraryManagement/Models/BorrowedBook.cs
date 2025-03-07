using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace LibraryManagement.Models;

public class BorrowedBook
{
    [Key]
    public int Id { get; set; }
    [ForeignKey("BookId")]
    public Book Book { get; set; }
    [ForeignKey("UserId")]
    public User User { get; set; }
    public DateTime BorrowDate { get; set; }=DateTime.Now;
    public bool IsReturned { get; set; } = false;
    public DateTime ReturnDate { get; set; }
}