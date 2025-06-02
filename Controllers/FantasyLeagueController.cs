using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TheTwelthFan.Data;
using TheTwelthFan.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

[ApiController]
[Route("api/fantasyleague")]
public class FantasyLeagueController : ControllerBase
{
    private readonly TodoContext _context;

    public FantasyLeagueController(TodoContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<FantasyLeague>>> GetFantasyLeagues()
    {
        return await _context.FantasyLeagues.ToListAsync();
    }

    [HttpGet("by-owner/{ownerid}")]
    public async Task<ActionResult<IEnumerable<FantasyLeague>>> GetLeagueByOwnerId(int ownerid)
    {
    var leagues = await _context.FantasyLeagues
        .Where(fl => fl.owneruserid == ownerid)
        .ToListAsync();
        if (leagues == null)
        {
            return NotFound(new { Message = "League not found for this owner" });
        }
        return Ok(leagues);
    }

    [HttpGet("by-team/{teamId}")]
    public async Task<ActionResult<FantasyLeague>> GetLeagueByTeamId(int teamId)
    {
        var league = await _context.FantasyLeagues.SingleOrDefaultAsync(fl => fl.id == teamId);
        if (league == null)
        {
            return NotFound(new { Message = "League not found for this team" });
        }
        return Ok(league);
    }

    [HttpGet("by-user/{userId}")]
    public async Task<ActionResult<IEnumerable<FantasyLeague>>> GetLeaguesByUserId(int userId)
    {
        // Step 1: Get all teams belonging to the user
        var userTeams = await _context.FantasyTeams
            .Where(ft => ft.userid == userId)
            .Select(ft => ft.fantasyleagueid) // Extract league IDs
            .Distinct() // Ensure unique leagues
            .ToListAsync();

        Console.WriteLine(userTeams);

        if (!userTeams.Any())
        {
            return NotFound(new { Message = "No leagues found for this user" });
        }

        // Step 2: Get all leagues matching the extracted league IDs
        var leagues = await _context.FantasyLeagues
            .Where(fl => userTeams.Contains(fl.id))
            .ToListAsync();

        return Ok(leagues);
    }

    [Authorize]
    [HttpPost("create")]
    public async Task<ActionResult<FantasyLeague>> CreateFantasyLeague(FantasyLeague fantasyLeague)
    {
        try
        {
            var leagueCheck = await _context.FantasyLeagues.SingleOrDefaultAsync(fl => fl.id == fantasyLeague.id);
            if (leagueCheck != null)
            {
                return Conflict(new { Message = $"League already exists: {fantasyLeague.name}" });
            }

            _context.FantasyLeagues.Add(fantasyLeague);
            var result = await _context.SaveChangesAsync();

            if (result > 0)
            {
                return CreatedAtAction(nameof(GetFantasyLeagues), new { id = fantasyLeague.id }, new
                {
                    fantasyLeague.id,
                    fantasyLeague.name
                });
            }
            return BadRequest("League creation failed.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating league: {ex.Message}");
            return StatusCode(500, "Internal server error");
        }
    }

    [Authorize]
    [HttpPut("update/{userId}/{leagueId}")]
    public async Task<IActionResult> UpdateLeagueName(int userId, int leagueId, [FromBody] FantasyLeague request)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            return NotFound(new { Message = "User not found" });
        }

        var league = await _context.FantasyLeagues.FindAsync(leagueId);
        if (league == null)
        {
            return NotFound(new { Message = "League not found" });
        }

        var usernameFromToken = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        if (usernameFromToken != user.username)
        {
            return Forbid();
        }

        league.name = request.name ?? league.name;

        await _context.SaveChangesAsync();

        return Ok(new { Message = "League name updated successfully" });
    }

    [Authorize]
    [HttpDelete("delete/{ownerUserId}/{leagueId}")]
    public async Task<IActionResult> DeleteFantasyLeague(int ownerUserId, int leagueId)
    {
        try
        {
            var league = await _context.FantasyLeagues.FindAsync(leagueId);
            if (league == null)
            {
                return NotFound(new { Message = "League not found" });
            }

            // Ensure the requesting user is the owner of the league
            if (league.owneruserid != ownerUserId)
            {
                return Forbid();
            }

            // Validate the token matches the ownerUserId
            var userIdFromToken = int.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value ?? "0");
            if (userIdFromToken != ownerUserId)
            {
                return Forbid();
            }

            _context.FantasyLeagues.Remove(league);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Fantasy league deleted successfully" });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting fantasy league: {ex.Message}");
            return StatusCode(500, "Internal server error");
        }
    }

}
