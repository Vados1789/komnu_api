public class CreateUserDto
{
    public string Username { get; set; }
    public string Email { get; set; }
    public string Bio { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public IFormFile? ProfilePicture { get; set; } // Ensure this is IFormFile
}
