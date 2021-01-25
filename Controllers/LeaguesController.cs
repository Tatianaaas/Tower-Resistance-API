using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JogoApi.Model;
using JogoApi.Model.Game;

namespace JogoApi.Controllers
{
    [Route("api/TowerResistance")]
    [ApiController]
    public class LeaguesController : ControllerBase
    {
        private readonly JogoContext _context;

        public LeaguesController(JogoContext context)
        {
            _context = context;
        }

        /// <summary>
        ///List of all leagues data 
        /// </summary>
        // GET: api/Leagues
        [HttpGet("Leagues")]
        public async Task<ActionResult<IEnumerable<Leagues>>> GetLeagues()
        {
            return await _context.Leagues.ToListAsync();
        }
        /// <summary>
        ///List of data from one user data 
        /// </summary>
         ///<param name="id"></param>
        // GET: api/Leagues/5
        [HttpGet("Leagues/{id}")]
        public async Task<ActionResult<Leagues>> GetLeagues(int id)
        {
            var leagues = await _context.Leagues.FindAsync(id);

            if (leagues == null)
            {
                return NotFound();
            }

            return leagues;
        }
        /// <summary>
        ///Update data from user league
        /// </summary>
         ///<param name="id"></param>
          ///<param name="leagues"></param>
        // PUT: api/Leagues/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("Leagues/{id}")]
        public async Task<IActionResult> PutLeagues(int id, Leagues leagues)
        {
            if (id != leagues.Id)
            {
                return BadRequest();
            }

            _context.Entry(leagues).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LeaguesExists(id))
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
        /// <summary>
        ///Poat data from user league
        /// </summary>
        ///<param name="leagues"></param>
        // POST: api/Leagues
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("Leagues/Set")]
        public async Task<ActionResult<Leagues>> PostLeagues(Leagues leagues)
        {
            _context.Leagues.Add(leagues);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetLeagues", new { id = leagues.Id }, leagues);
        }

        /// <summary>
        ///Delete data from user league
        /// </summary>
        ///<param name="id"></param>
        // DELETE: api/Leagues/5
        [HttpDelete("Leagues/Delete/{id}")]
        public async Task<IActionResult> DeleteLeagues(int id)
        {
            var leagues = await _context.Leagues.FindAsync(id);
            if (leagues == null)
            {
                return NotFound();
            }

            _context.Leagues.Remove(leagues);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool LeaguesExists(int id)
        {
            return _context.Leagues.Any(e => e.Id == id);
        }
    }
}
