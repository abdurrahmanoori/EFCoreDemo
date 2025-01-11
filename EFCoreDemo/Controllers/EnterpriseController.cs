using EFCoreDemo.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EFCoreDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnterpriseController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EnterpriseController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Enterprise
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ENTERPRISE>>> GetEnterprises( )
        {
            return await _context.ENTERPRISE.ToListAsync();
        }

        // GET: api/Enterprise/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ENTERPRISE>> GetEnterprise(long id)
        {
            var enterprise = await _context.ENTERPRISE.FindAsync(id);

            if (enterprise == null)
            {
                return NotFound();
            }

            return enterprise;
        }

        // POST: api/Enterprise
        [HttpPost]
        public async Task<ActionResult<ENTERPRISE>> CreateEnterprise(ENTERPRISE enterprise)
        {
            _context.ENTERPRISE.Add(enterprise);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEnterprise), new { id = enterprise.ENTERPRISE_NO }, enterprise);
        }

        // PUT: api/Enterprise/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEnterprise(long id, ENTERPRISE enterprise)
        {
            if (id != enterprise.ENTERPRISE_NO)
            {
                return BadRequest();
            }

            _context.Entry(enterprise).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EnterpriseExists(id))
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

        // DELETE: api/Enterprise/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEnterprise(long id)
        {
            var enterprise = await _context.ENTERPRISE.FindAsync(id);
            if (enterprise == null)
            {
                return NotFound();
            }

            _context.ENTERPRISE.Remove(enterprise);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool EnterpriseExists(long id)
        {
            return _context.ENTERPRISE.Any(e => e.ENTERPRISE_NO == id);
        }
    }
}
