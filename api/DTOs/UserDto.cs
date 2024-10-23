namespace api.DTOs
{
    public class UserDto
    {
        public int UserId { get; set; }
        public string? Username { get; set; } // Nullable
        public string? Email { get; set; } // Nullable
        public string? PhoneNumber { get; set; } // New field
        public string? ProfilePicture { get; set; } // Nullable
        public string? Bio { get; set; } // Nullable
        public DateTime? DateOfBirth { get; set; } // Nullable
    }
}
