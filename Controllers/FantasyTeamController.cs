using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TheTwelthFan.Data;
using TheTwelthFan.Models;
using TheTwelthFanAPI.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

[ApiController]
[Route("api/fantasyteam")]
public class FantasyTeamController : ControllerBase
{
    private readonly TodoContext _context;

    public FantasyTeamController(TodoContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<FantasyTeam>>> GetFantasyTeams()
    {
        return await _context.FantasyTeams.ToListAsync();
    }

    [Authorize]
    [HttpPost("create")]
    public async Task<ActionResult<User>> CreateFantasyTeam(FantasyTeam fantasyTeam)
    {
        try
        {
            // Check if the team exists in the database
            var teamCheck = await _context.FantasyTeams.SingleOrDefaultAsync(ft => ft.id == fantasyTeam.id);
            if(teamCheck != null){
                Console.WriteLine($"Team already exists: {fantasyTeam.name}");
                return Conflict(new { Message = $"Team already exists: {fantasyTeam.name}" });
            }

            // Log the user details for debugging
            Console.WriteLine($"Creating team with name: {fantasyTeam.name}");

            // Add the user to the database
            _context.FantasyTeams.Add(fantasyTeam);
            var result = await _context.SaveChangesAsync();

            if (result > 0)
            {
                // Log success
                Console.WriteLine($"Successfully created player: {fantasyTeam.name}");

                // Return the created user (excluding sensitive info like the password)
                return CreatedAtAction(nameof(GetFantasyTeams), new { id = fantasyTeam.id }, new 
                {
                    fantasyTeam.id,
                    fantasyTeam.name
                });
            }
            else
            {
                // Log if no rows were affected
                Console.WriteLine("No rows were affected during save.");
                return BadRequest("Player creation failed.");
            }
        }
        catch (Exception ex)
        {
            // Log the error details
            Console.WriteLine($"Error creating player: {ex.Message}");
            return StatusCode(500, "Internal server error");
        }
    }

    [Authorize]
    [HttpGet("get-user-team/{userId}")]
    public async Task<ActionResult<List<Player>>> GetTeamsByUser(int userId)
    {
        try
        {
            // Ensure the user exists
            var userExists = await _context.Users.AnyAsync(u => u.id == userId);
            if (!userExists)
            {
                return NotFound(new { Message = "User not found" });
            }

            // Fetch teams associated with the user
            var team = await _context.FantasyTeams
                                        .Where(p => p.userid == userId)
                                        .ToListAsync();

            if (team == null || team.Count == 0)
            {
                return NotFound(new { Message = "No teams found for this user" });
            }

            return Ok(team);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving players: {ex.Message}");
            return StatusCode(500, "Internal server error");
        }
    }

    [Authorize]
    [HttpGet("get-league-teams/{leagueId}")]
    public async Task<ActionResult<List<Player>>> GetTeamsByLeague(int leagueId)
    {
        try
        {
            // Ensure the user exists
            var userExists = await _context.FantasyLeagues.AnyAsync(u => u.id == leagueId);
            if (!userExists)
            {
                return NotFound(new { Message = "League not found" });
            }

            // Fetch teams associated with the user
            var team = await _context.FantasyTeams
                                        .Where(p => p.userid == leagueId)
                                        .ToListAsync();

            if (team == null || team.Count == 0)
            {
                return NotFound(new { Message = "No teams found for this user" });
            }

            return Ok(team);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving players: {ex.Message}");
            return StatusCode(500, "Internal server error");
        }
    }

    [Authorize]
    [HttpPut("update/{userId}/{teamId}")]
    public async Task<IActionResult> UpdatePlayer(int userId, int teamId, [FromBody] FantasyTeam request)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            return NotFound(new { Message = "User not found" });
        }

        var team = await _context.FantasyTeams.FindAsync(teamId);
        if (team == null)
        {
            return NotFound(new { Message = "Team not found" });
        }

        var usernameFromToken = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        if (usernameFromToken != user.username)
        {
            return Forbid();
        }

        // Update player fields
        team.name = request.name ?? team.name;

        // Save changes
        await _context.SaveChangesAsync();

        return Ok(new { Message = "Team name updated successfully" });
    }
    [Authorize]
    [HttpDelete("delete/{userId}/{teamId}")]
    public async Task<IActionResult> DeleteFantasyTeam(int userId, int teamId)
    {
        try
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound(new { Message = "User not found" });
            }

            var team = await _context.FantasyTeams.FindAsync(teamId);
            if (team == null)
            {
                return NotFound(new { Message = "Team not found" });
            }

            var usernameFromToken = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (usernameFromToken != user.username)
            {
                return Forbid();
            }

            _context.FantasyTeams.Remove(team);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Fantasy team deleted successfully" });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting fantasy team: {ex.Message}");
            return StatusCode(500, "Internal server error");
        }
    }
}
