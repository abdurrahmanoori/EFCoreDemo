using EFCoreDemo.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace sigtasDemo.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EntActivityController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EntActivityController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/EntActivity
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ENT_ACTIVITY>>> GetEntActivities( )
        {
            return await _context.ENT_ACTIVITY
                .Include(ea => ea.ENT_BUS_ACT) // Include related business activities
                .ToListAsync();
        }

        // GET: api/EntActivity/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ENT_ACTIVITY>> GetEntActivity(byte id)
        {
            var entActivity = await _context.ENT_ACTIVITY
                .Include(ea => ea.ENT_BUS_ACT)
                .FirstOrDefaultAsync(ea => ea.ENT_ACTIVITY_NO == id);

            if (entActivity == null)
            {
                return NotFound();
            }

            return entActivity;
        }

        // POST: api/EntActivity
        [HttpPost]
        public async Task<ActionResult<ENT_ACTIVITY>> CreateEntActivity(ENT_ACTIVITY entActivity)
        {
            _context.ENT_ACTIVITY.Add(entActivity);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEntActivity), new { id = entActivity.ENT_ACTIVITY_NO }, entActivity);
        }

        // PUT: api/EntActivity/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEntActivity(byte id, ENT_ACTIVITY entActivity)
        {
            if (id != entActivity.ENT_ACTIVITY_NO)
            {
                return BadRequest();
            }

            _context.Entry(entActivity).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EntActivityExists(id))
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

        // DELETE: api/EntActivity/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEntActivity(byte id)
        {
            var entActivity = await _context.ENT_ACTIVITY.FindAsync(id);
            if (entActivity == null)
            {
                return NotFound();
            }

            _context.ENT_ACTIVITY.Remove(entActivity);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool EntActivityExists(byte id)
        {
            return _context.ENT_ACTIVITY.Any(e => e.ENT_ACTIVITY_NO == id);
        }
    }
}
