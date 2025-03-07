namespace LibraryManagement.DTOs
{
    public class BookDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Publisher { get; set; }
        public string Genre { get; set; }
        public bool IsAvailable { get; set; }
        public int? UserId { get; set; }
    }
    public class AddBookDTO
    {

        public string Title { get; set; }
        public string Author { get; set; }
        public string Publisher { get; set; }
        public string Genre { get; set; }
        public bool IsAvailable { get; set; }
        //public int? UserId { get; set; }
    }
}
