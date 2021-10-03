using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BattleShip.Data;
using BattleShip.Models;
using BattleShip.SignalR;
using Microsoft.AspNetCore.SignalR;

namespace BattleShip.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LobbiesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        private readonly IHubContext<MessageHub> hubContext;

        public LobbiesController(ApplicationDbContext context, IHubContext<MessageHub> hubContext)
        {
            _context = context;
            this.hubContext = hubContext;
            
        }

        // GET: api/Lobbies
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Lobby>>> GetLobby()
        {
            return await _context.Lobby.ToListAsync();
        }

        // GET: api/Lobbies/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Lobby>> GetLobby(int id)
        {
            var lobby = await _context.Lobby.FindAsync(id);

            if (lobby == null)
            {
                return NotFound();
            }

            return lobby;
        }

        // PUT: api/Lobbies/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLobby(int id, [FromBody] Lobby lobby)
        {
            var existingLobby = await _context.Lobby.FindAsync(id);
            if(existingLobby == null)
            {
                return BadRequest("There is no lobby with this id.");
            }
            existingLobby.Guest = lobby.Guest;
            
            _context.Entry(existingLobby).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LobbyExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            await hubContext.Clients.All.SendAsync("LobbyResponse", "LobbyAccepted");
            return Ok("Joined to lobby.");
        }

        // POST: api/Lobbies
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Lobby>> PostLobby(Lobby lobby)
        {
            _context.Lobby.Add(lobby);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetLobby", new { id = lobby.Id }, lobby);
        }

        // DELETE: api/Lobbies/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLobby(int id)
        {
            var lobby = await _context.Lobby.FindAsync(id);
            if (lobby == null)
            {
                return NotFound();
            }

            _context.Lobby.Remove(lobby);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool LobbyExists(int id)
        {
            return _context.Lobby.Any(e => e.Id == id);
        }
    }
}
