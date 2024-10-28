namespace api.DTOs
{
    public class ProfileDto
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string ProfilePicture { get; set; } // Full URL for the profile picture
        public string Bio { get; set; }
        public DateTime DateOfBirth { get; set; }
    }
}
