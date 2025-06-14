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
            DraftOrder = new List<DraftOrderEntry>()
        };

        int pickNumber = 1;
        foreach (var team in teams)
        {
            draft.DraftOrder.Add(new DraftOrderEntry
            {
                PickNumber = pickNumber++,
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

        var currentPick = draft.DraftOrder
            .FirstOrDefault(d => d.PickNumber == draft.PickNumber);

        if (currentPick == null)
            return Ok(new { IsComplete = true });

        return Ok(new
        {
            TeamId = currentPick.FantasyTeamId,
            PickNumber = draft.PickNumber
        });
    }

    [Authorize]
    [HttpPost("pick")]
    public async Task<IActionResult> MakeDraftPick([FromBody] DraftPickRequest request)
    {
        var draft = await _context.Drafts
            .Include(d => d.DraftOrder)
            .FirstOrDefaultAsync(d => d.FantasyLeagueId == request.LeagueId);

        if (draft == null || draft.IsComplete)
            return BadRequest("Draft not found or completed.");

        var currentOrder = draft.DraftOrder
            .FirstOrDefault(d => d.PickNumber == draft.PickNumber);

        if (currentOrder == null || currentOrder.FantasyTeamId != request.TeamId)
            return BadRequest("It's not your turn.");

        var player = await _context.Players.FirstOrDefaultAsync(p =>
            p.id == request.PlayerId &&
            p.fantasyleagueid == request.LeagueId &&
            p.fantasyteamid == 0);

        if (player == null)
            return BadRequest("Player already drafted or invalid.");

        player.fantasyteamid = request.TeamId;
        player.userId = request.UserId;
        draft.PickNumber++;

        if (draft.PickNumber > draft.DraftOrder.Count)
        {
            draft.IsComplete = true;
            draft.CompletedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();

        return Ok();
    }
}
