namespace TheTwelthFan.Models;

public class DraftPickRequest
{
    public int LeagueId { get; set; }
    public int TeamId { get; set; }
    public int UserId { get; set; }
    public int PlayerId { get; set; }
}