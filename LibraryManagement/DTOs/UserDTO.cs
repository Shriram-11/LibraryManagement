namespace LibraryManagement.DTOs
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }

    public class CreateUserDTO
    {
        public string Name { get; set; }
        public string Email { get; set; }
    }
}
