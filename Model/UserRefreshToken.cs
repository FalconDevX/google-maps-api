public class UserRefreshToken
{
    public int Id { get; set; }
    public required int UserId { get; set; }
    public required string RefreshToken { get; set; }
    public DateTime ExpiryDate { get; set; }
    public DateTime CreatedAt{ get; set; }
    public bool IsRevoked { get; set; }
}