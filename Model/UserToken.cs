public class UserToken
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime ExpiryDate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsRevoked { get; set; } = false;

    public required User User { get; set; }   
}
