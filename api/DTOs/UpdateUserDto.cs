namespace api.DTOs
{
    public class UpdateUserDto
    {
        public string Username { get; set; } // Optional - can be updated if provided
        public string Email { get; set; } // Optional - can be updated if provided
        public string PhoneNumber { get; set; } // New field
        public string ProfilePicture { get; set; } // Optional - can be updated if provided
        public string Bio { get; set; } // Optional - can be updated if provided
        public DateTime? DateOfBirth { get; set; } // Optional and nullable
    }
}
