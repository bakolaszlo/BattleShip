using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BattleShip.Data;
using BattleShip.Models;

namespace BattleShip.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LobbiesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public LobbiesController(ApplicationDbContext context)
        {
            _context = context;
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
        public async Task<IActionResult> PutLobby(int id, Lobby lobby)
        {
            if (id != lobby.Id)
            {
                return BadRequest();
            }

            _context.Entry(lobby).State = EntityState.Modified;

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

            return NoContent();
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
