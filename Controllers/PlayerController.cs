using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TheTwelthFan.Data;
using TheTwelthFan.Models;
using TheTwelthFanAPI.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

[ApiController]
[Route("api/players")]
public class PlayerController : ControllerBase
{
    private readonly TodoContext _context;
    public PlayerController(TodoContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Player>>> GetPlayers()
    {
        return await _context.Players.ToListAsync();
    }

    [Authorize]
    [HttpPost("create")]
    public async Task<ActionResult<User>> CreatePlayer(Player player)
    {
        try
        {
            // Check if the user exists in the database
            var userCheck = await _context.Users.SingleOrDefaultAsync(p => p.id == player.id);
            if(userCheck != null){
                Console.WriteLine($"Player already exists: {player.name}");
                return Conflict(new { Message = $"Player already exists: {player.name}" });
            }

            // Log the user details for debugging
            Console.WriteLine($"Creating player with name: {player.name}");

            // Add the user to the database
            _context.Players.Add(player);
            var result = await _context.SaveChangesAsync();

            if (result > 0)
            {
                // Log success
                Console.WriteLine($"Successfully created player: {player.name}");

                // Return the created user (excluding sensitive info like the password)
                return CreatedAtAction(nameof(GetPlayers), new { id = player.id }, new 
                {
                    player.id,
                    player.name
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
    [HttpGet("get-user-players/{userId}")]
    public async Task<ActionResult<List<Player>>> GetPlayersByUser(int userId)
    {
        try
        {
            // Ensure the user exists
            var userExists = await _context.Users.AnyAsync(u => u.id == userId);
            if (!userExists)
            {
                return NotFound(new { Message = "User not found" });
            }

            // Fetch players associated with the user
            var players = await _context.Players
                                        .Where(p => p.userId == userId)
                                        .ToListAsync();

            if (players == null || players.Count == 0)
            {
                return NotFound(new { Message = "No players found for this user" });
            }

            return Ok(players);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving players: {ex.Message}");
            return StatusCode(500, "Internal server error");
        }
    }

    [Authorize]
    [HttpGet("get-team-players/{fantasyLeagueTeamId}")]
    public async Task<ActionResult<List<Player>>> GetPlayersByTeam(int fantasyLeagueTeamId)
    {
        try
        {
            // Ensure the user exists
            var teamExists = await _context.FantasyTeams.AnyAsync(f => f.id == fantasyLeagueTeamId);
            if (!teamExists)
            {
                return NotFound(new { Message = "Team not found" });
            }

            // Fetch players associated with the user
            var players = await _context.Players
                                        .Where(p => p.fantasyteamid == fantasyLeagueTeamId)
                                        .ToListAsync();

            if (players == null || players.Count == 0)
            {
                return NotFound(new { Message = "No players found for this team" });
            }

            return Ok(players);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving players: {ex.Message}");
            return StatusCode(500, "Internal server error");
        }
    }

    [Authorize]
    [HttpPut("update/{userId}/{playerId}")]
    public async Task<IActionResult> UpdatePlayer(int userId, int playerId, [FromBody] Player request)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            return NotFound(new { Message = "User not found" });
        }

        var player = await _context.Players.FindAsync(playerId);
        if (player == null)
        {
            return NotFound(new { Message = "Player not found" });
        }

        var usernameFromToken = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        if (usernameFromToken != user.username)
        {
            return Forbid();
        }

        // Update player fields
        player.name = request.name ?? player.name;
        player.jerseynumber = request.jerseynumber;
        player.team = request.team ?? player.team;
        player.fantasyteamid = request.fantasyteamid;

        // Save changes
        await _context.SaveChangesAsync();

        return Ok(new { Message = "User updated successfully" });
    }

    [Authorize]
    [HttpDelete("delete/{userId}/{playerId}")]
    public async Task<IActionResult> DeletePlayer(int userId, int playerId)
    {
        try
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound(new { Message = "User not found" });
            }

            var player = await _context.Players.FindAsync(playerId);
            if (player == null)
            {
                return NotFound(new { Message = "Player not found" });
            }

            var usernameFromToken = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (usernameFromToken != user.username)
            {
                return Forbid();
            }

            _context.Players.Remove(player);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Player deleted successfully" });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting player: {ex.Message}");
            return StatusCode(500, "Internal server error");
        }
    }

}
