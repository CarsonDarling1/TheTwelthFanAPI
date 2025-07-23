namespace TheTwelthFan.Models;

public class Player
{
    public int id { get; set; } // Primary Key
    public string name { get; set; } = string.Empty;
    public string position { get; set; } = string.Empty;
    public int jerseynumber { get; set; }
    public string team { get; set; } = string.Empty; // Optional: Store the player's team
    public int fantasyteamid { get; set; }
    public int fantasyleagueid { get; set; }
    public required int userId { get; set; }
    public int pointLastSeason { get; set; }
    public int passingYards { get; set; }
    public int passingTouchdowns { get; set; }
    public int interceptions { get; set; }
    public int rushingYards { get; set; }
    public int rushingTouchdowns { get; set; }
    public int receivingYards { get; set; }
    public int receivingTouchdowns { get; set; }
    }

