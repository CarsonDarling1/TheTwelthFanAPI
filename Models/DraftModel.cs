namespace TheTwelthFan.Models;

public class Draft
{
    public int Id { get; set; } // Primary Key

    public int FantasyLeagueId { get; set; } // FK to FantasyLeague
    public int CurrentTeamPickingId { get; set; } // FK to FantasyTeam

    public int PickNumber { get; set; } // 1-based index for current pick
    public bool IsComplete { get; set; } = false;

    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }

    public List<DraftOrderEntry> DraftOrder { get; set; } = new();
}
