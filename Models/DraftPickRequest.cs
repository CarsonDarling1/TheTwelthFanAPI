namespace TheTwelthFan.Models;

public class DraftPickRequest
{
    public required int LeagueId { get; set; }
    public required int TeamId { get; set; }
    public required int UserId { get; set; }
    public required int PlayerId { get; set; }
}