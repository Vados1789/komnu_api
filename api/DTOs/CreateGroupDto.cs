using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace api.DTOs
{
    public class CreateGroupDto
    {
        [Required]
        public string GroupName { get; set; }

        [Required]
        public string Description { get; set; }

        public IFormFile Image { get; set; }

        [Required]
        public int UserId { get; set; }  // Added UserId field for the creator
    }
}
