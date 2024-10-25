namespace api.DTOs
{
    public class TwoFaResponseDto
    {
        public string Message { get; set; }
        public bool RequiresTwoFa { get; set; }
        public int UserId { get; set; }
    }
}
