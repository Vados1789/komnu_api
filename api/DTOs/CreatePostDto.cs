using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace api.DTOs
{
    public class CreatePostDto
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public string Content { get; set; }

        public IFormFile Image { get; set; }
    }
}
