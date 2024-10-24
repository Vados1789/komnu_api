public class CreateLoginDto
{
    public int UserId { get; set; }
    public string Password { get; set; }
    public bool IsTwoFaEnabled { get; set; }
    public string TwoFaMethod { get; set; }
}
