namespace WebAPI.DTOs
{
    public class UserSavedPlacesDto
    {
        public string Location { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<string> Tags { get; set; } = new();
        public string Image { get; set; } = string.Empty;
    }

}
