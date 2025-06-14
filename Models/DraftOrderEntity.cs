namespace TheTwelthFan.Models;

public class DraftOrderEntry
{
    public int Id { get; set; }

    public int DraftId { get; set; } // FK to Draft
    public int PickNumber { get; set; }
    public int FantasyTeamId { get; set; } // FK to FantasyTeam
}