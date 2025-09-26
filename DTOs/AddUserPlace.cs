namespace WebAPI.DTOs
{
    public class AddUserPlace
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string PlaceName { get; set; } = string.Empty;
    }
}
