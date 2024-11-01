using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace api.DTOs
{
    public class UpdatePostDto
    {
        [Required]
        public int UserId { get; set; }

        public string Content { get; set; }

        public IFormFile Image { get; set; }
    }
}
