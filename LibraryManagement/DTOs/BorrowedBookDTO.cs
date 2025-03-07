namespace LibraryManagement.DTOs
{
    public class BorrowedBookDTO
    {
        public int Id { get; set; }
        public string BookTitle { get; set; }
        public string UserName { get; set; }
        public DateTime BorrowDate { get; set; }
        public bool IsReturned { get; set; }
        public DateTime? ReturnDate { get; set; }
    }

    public class BorrowBookDTO
    {
        public int Id{get; set;}
        public int BookId { get; set; }
        public int UserId { get; set; }
    }

    public class BorrowingBookDTO
    {
        //public int Id{get; set;}
        public int BookId { get; set; }
        public int UserId { get; set; }
    }
}

