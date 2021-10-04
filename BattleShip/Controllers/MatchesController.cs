using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BattleShip.Data;
using BattleShip.Models;
using Microsoft.AspNetCore.SignalR;
using BattleShip.SignalR;

namespace BattleShip.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MatchesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        private readonly IHubContext<MessageHub> hubContext;

        public MatchesController(ApplicationDbContext context, IHubContext<MessageHub> hubContext)
        {
            _context = context;
            this.hubContext = hubContext;
        }

        // GET: api/Matches
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Match>>> GetMatch()
        {
            return await _context.Match.ToListAsync();
        }

        // GET: api/Matches/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Match>> GetMatch(int id)
        {
            var match = await _context.Match.FindAsync(id);

            if (match == null)
            {
                return NotFound();
            }

            return match;
        }

        // PUT: api/Matches/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut]
        public async Task<IActionResult> PutMatch(Match match)
        {
            var lobbyId = await _context.Lobby.FindAsync(match.LobbyId);
            if(lobbyId == null)
            {
               return BadRequest("Not found");
            }

            var originalMatch = await _context.Match.FirstAsync(x => x.LobbyId == match.LobbyId);
            if(originalMatch == null)
            {
                return BadRequest("Match not found");
            }
            originalMatch.GuestId = match.GuestId;
            originalMatch.GuestBoard = match.GuestBoard;
            originalMatch.HostHp = 17;
            originalMatch.GuestHp = 17;

            _context.Entry(originalMatch).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                hubContext.Clients.All.SendAsync("bothPlayersAreReady", originalMatch.HostId, originalMatch.GuestId);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MatchExists(match.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }


        [HttpPut("{index}")]
        public async Task<IActionResult> PutMatchAttack(AttackModel attack)
        {
            var lobbyId = await _context.Lobby.FindAsync(attack.LobbyId);
            if (lobbyId == null)
            {
                return BadRequest("Not found");
            }

            var originalMatch = await _context.Match.FirstAsync(x => x.LobbyId == attack.LobbyId);
            if (originalMatch == null)
            {
                return BadRequest("Match not found");
            }
            if(attack.GuestId != 0 && originalMatch.IsHostTurn)
            {
                return BadRequest("It's host turn.");
            }
            if(attack.HostId !=0 && !originalMatch.IsHostTurn)
            {
                return BadRequest("It's guest turn.");
            }

            if(originalMatch.IsHostTurn)
            {
                if (originalMatch.GuestBoard[attack.AttackIndex] != 'w')
                {
                    //emit hit
                    hubContext.Clients.All.SendAsync("hostHitOn", originalMatch.LobbyId,attack.AttackIndex);
                    var toEdit = originalMatch.GuestBoard.ToCharArray();
                    toEdit[attack.AttackIndex] = 'x';
                    originalMatch.GuestBoard = new string(toEdit);
                    originalMatch.GuestHp--;
                }
                else
                {
                    hubContext.Clients.All.SendAsync("hostAttackedOn", originalMatch.LobbyId, attack.AttackIndex);
                    var toEdit = originalMatch.GuestBoard.ToCharArray();
                    toEdit[attack.AttackIndex] = 'x';
                    originalMatch.GuestBoard = new string(toEdit);
                }
            }

            if (!originalMatch.IsHostTurn)
            {
                if (originalMatch.HostBoard[attack.AttackIndex] != 'w')
                {
                    //emit hit
                    hubContext.Clients.All.SendAsync("guestHitOn", originalMatch.LobbyId, attack.AttackIndex);
                    var toEdit = originalMatch.HostBoard.ToCharArray();
                    toEdit[attack.AttackIndex] = 'x';
                    originalMatch.HostBoard = new string(toEdit);
                    originalMatch.HostHp--;
                }
                else
                {
                    hubContext.Clients.All.SendAsync("guestAttackedOn", originalMatch.LobbyId, attack.AttackIndex);
                    var toEdit = originalMatch.HostBoard.ToCharArray();
                    toEdit[attack.AttackIndex] = 'x';
                    originalMatch.HostBoard = new string(toEdit);
                }
            }

            if(originalMatch.GuestHp == 0)
            {
                hubContext.Clients.All.SendAsync("winner", originalMatch.LobbyId, originalMatch.HostId);
                var lobby = _context.Lobby.First(x => x.Guest == originalMatch.GuestId && x.Host == originalMatch.HostId);
                if(lobby != null)
                {
                    lobby.IsOver = true;
                }
                _context.Entry(lobby).State = EntityState.Modified;
            }
            if(originalMatch.HostHp == 0)
            {
                hubContext.Clients.All.SendAsync("winner", originalMatch.LobbyId, originalMatch.GuestId);
            }

            originalMatch.IsHostTurn = !originalMatch.IsHostTurn;
            _context.Entry(originalMatch).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                hubContext.Clients.All.SendAsync("bothPlayersAreReady", originalMatch.HostId, originalMatch.GuestId);
            }
            catch (DbUpdateConcurrencyException)
            {
                return BadRequest("Error on db");
            }

            return NoContent();
        }

        [HttpPost]
        public async Task<IActionResult> PostMatch(Match match)
        {
            try
            {
                var toTest = await _context.Match.FirstAsync(x=> x.LobbyId == match.LobbyId);
                return await UpdateMatch(match);
            }
            catch
            {
                _context.Match.Add(match);
                await _context.SaveChangesAsync();
            }

            return Ok();
        }

        private async Task<IActionResult> UpdateMatch(Match match)
        {
            var lobbyId = await _context.Lobby.FindAsync(match.LobbyId);
            if (lobbyId == null)
            {
                return BadRequest("Not found");
            }

            var originalMatch = await _context.Match.FirstAsync(x => x.LobbyId == match.LobbyId);
            if (originalMatch == null)
            {
                return BadRequest("Match not found");
            }
            if (originalMatch.HostId != 0)
            {
                originalMatch.GuestId = match.GuestId;
                originalMatch.GuestBoard = match.GuestBoard;
                originalMatch.HostHp = 17;
                originalMatch.GuestHp = 17;
            }
            else
            {
                originalMatch.HostId = match.HostId;
                originalMatch.HostBoard = match.HostBoard;
                originalMatch.HostHp = 17;
                originalMatch.GuestHp = 17;
            }
            originalMatch.IsHostTurn = true;
            _context.Entry(originalMatch).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                hubContext.Clients.All.SendAsync("bothPlayersAreReady", originalMatch.HostId, originalMatch.GuestId);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MatchExists(match.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok();
        }

        // DELETE: api/Matches/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMatch(int id)
        {
            var match = await _context.Match.FindAsync(id);
            if (match == null)
            {
                return NotFound();
            }

            _context.Match.Remove(match);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MatchExists(int id)
        {
            return _context.Match.Any(e => e.Id == id);
        }
    }
}
