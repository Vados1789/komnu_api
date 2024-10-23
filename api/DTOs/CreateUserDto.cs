using System;
using System.ComponentModel.DataAnnotations;

namespace api.DTOs
{
    public class CreateUserDto
    {
        [Required]
        [StringLength(50)]
        public string Username { get; set; }

        [Required]
        [EmailAddress] // Additional validation to ensure a valid email format
        [StringLength(100)]
        public string Email { get; set; }

        [StringLength(255)]
        public string ProfilePicture { get; set; } // Optional

        public string Bio { get; set; } // Optional

        // Make DateOfBirth optional by using DateTime?
        public DateTime? DateOfBirth { get; set; } // Optional
    }
}
