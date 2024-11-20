using Microsoft.AspNetCore.Http;

namespace api.DTOs
{
    public class GroupPostDto
    {
        public int GroupId { get; set; }
        public int UserId { get; set; }
        public string Content { get; set; }
        public IFormFile Image { get; set; } // Optional image
    }
}
