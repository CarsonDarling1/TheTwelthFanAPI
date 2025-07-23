using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TheTwelthFan.Data;
using TheTwelthFan.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace TheTwelthFan.Controllers;

[ApiController]
[Route("api/draft")]
public class DraftController : ControllerBase
{
    private readonly TodoContext _context;

    public DraftController(TodoContext context)
    {
        _context = context;
    }

    [Authorize]
    [HttpPost("start")]
    public async Task<IActionResult> StartDraft([FromBody] int leagueId)
    {
        var teams = await _context.FantasyTeams
            .Where(t => t.fantasyleagueid == leagueId)
            .ToListAsync();

        if (teams.Count < 2)
            return BadRequest("Need at least two teams to start a draft.");

        var draft = new Draft
        {
            FantasyLeagueId = leagueId,
            PickNumber = 0,
            DraftOrder = new List<DraftOrderEntry>()
        };

        int pickNum = 0;

        // Assign pick numbers — shuffle if needed
        foreach (var team in teams.OrderBy(t => t.id)) // or use Random for fairness
        {
            draft.DraftOrder.Add(new DraftOrderEntry
            {
                PickNumber = pickNum++,
                FantasyTeamId = team.id
            });
        }

        _context.Drafts.Add(draft);
        await _context.SaveChangesAsync();

        return Ok(draft);
    }

    [HttpGet("current-turn")]
    public async Task<IActionResult> GetCurrentTurn([FromQuery] int leagueId)
    {
        var draft = await _context.Drafts
            .Include(d => d.DraftOrder)
            .FirstOrDefaultAsync(d => d.FantasyLeagueId == leagueId);

        if (draft == null)
            return NotFound("Draft not found.");

        if (draft.IsComplete)
            return Ok(new { IsComplete = true });

        var ordered = draft.DraftOrder.OrderBy(d => d.PickNumber).ToList();
        var index = draft.PickNumber % ordered.Count;
        var currentEntry = ordered[index];

        return Ok(new
        {
            TeamId = currentEntry.FantasyTeamId,
            PickNumber = draft.PickNumber,
            IsComplete = false
        });
    }

    [Authorize]
    [HttpPost("pick")]
    public async Task<IActionResult> MakeDraftPick([FromBody] DraftPickRequest request)
    {
Console.WriteLine($"DraftPickRequest received: " +
    $"LeagueId={request.LeagueId}, " +
    $"TeamId={request.TeamId}, " +
    $"UserId={request.UserId}, " +
    $"PlayerId={request.PlayerId}");        var draft = await _context.Drafts
    .Include(d => d.DraftOrder)
    .FirstOrDefaultAsync(d => d.FantasyLeagueId == request.LeagueId);

        if (draft == null || draft.IsComplete)
            return BadRequest("Draft not found or completed.");

        var ordered = draft.DraftOrder.OrderBy(d => d.PickNumber).ToList();

        if (draft.PickNumber >= ordered.Count)
            return BadRequest("All picks have been made.");

        var currentEntry = ordered[draft.PickNumber];

        if (currentEntry.FantasyTeamId != request.TeamId)
            return BadRequest("It's not your turn.");

        // Validate player
        var player = await _context.Players.FirstOrDefaultAsync(p =>
            p.id == request.PlayerId &&
            p.fantasyleagueid == request.LeagueId &&
            p.fantasyteamid == 0);

        if (player == null)
            return BadRequest("Player already drafted or invalid.");

        // Update player
        player.fantasyteamid = request.TeamId;
        player.userId = request.UserId;

        // Advance draft
        draft.PickNumber++;

        int totalPlayers = await _context.Players.CountAsync(p => p.fantasyleagueid == request.LeagueId);
        int draftedPlayers = await _context.Players.CountAsync(p => p.fantasyleagueid == request.LeagueId && p.fantasyteamid != 0);

        if (draftedPlayers >= totalPlayers)
        {
            draft.IsComplete = true;
            draft.CompletedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();
        return Ok();

    }

    [Authorize]
    [HttpGet("teams")]
    public async Task<IActionResult> GetDraftTeams([FromQuery] int leagueId)
    {
        var draft = await _context.Drafts
            .Include(d => d.DraftOrder)
            .FirstOrDefaultAsync(d => d.FantasyLeagueId == leagueId);

        if (draft == null)
            return NotFound("Draft not found");

        var teams = await _context.FantasyTeams
            .Where(t => t.fantasyleagueid == leagueId)
            .ToListAsync();

        var results = draft.DraftOrder
            .OrderBy(d => d.PickNumber)
            .Select(order =>
            {
                var team = teams.FirstOrDefault(t => t.id == order.FantasyTeamId);
                return new
                {
                    id = team.id,
                    pickNumber = order.PickNumber,
                    name = team.name,
                    userid = team.userid
                };
            });

        return Ok(results);
    }


}
