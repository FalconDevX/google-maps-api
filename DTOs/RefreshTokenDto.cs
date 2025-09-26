namespace WebAPI.DTOs
{
    public class RefreshTokenDto
    {
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime ExpiryDate { get; set; }
    }
}
