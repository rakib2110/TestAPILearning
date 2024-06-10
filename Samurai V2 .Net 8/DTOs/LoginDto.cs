namespace Samurai_V2_.Net_8.DTOs
{
    public class LoginDto
    {
        public required string Username { get; set; }
        public required string Password { get; set; }
    }
    public class TokenResponseDto
    {
        public string? Token { get; set; }
        public DateTime Expiration { get; set; }
    }
}
