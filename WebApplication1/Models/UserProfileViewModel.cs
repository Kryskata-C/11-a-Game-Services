using WebApplication1.Controllers;

public class UserProfileViewModel
{
    public string Username { get; set; }
    public DateTime UserSince { get; set; }
    public int TotalSpent { get; set; }
    public List<string> MostHiredPlayers { get; set; }
    public List<Review> Reviews { get; set; }
}
