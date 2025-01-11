using EFCoreDemo.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EFCoreDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaxPayer2Controller : ControllerBase
    {
        private readonly AppDbContext _context;

        public TaxPayer2Controller(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/TaxPayer
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TAX_PAYER>>> GetTaxPayers( )
        {
            return await _context.TAX_PAYER.ToListAsync();
        }

        // GET: api/TaxPayer/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TAX_PAYER>> GetTaxPayer(long id)
        {
            var taxPayer = await _context.TAX_PAYER.FindAsync(id);

            if (taxPayer == null)
            {
                return NotFound();
            }

            return taxPayer;
        }

        // POST: api/TaxPayer
        [HttpPost]
        public async Task<ActionResult<TAX_PAYER>> CreateTaxPayer(TAX_PAYER taxPayer)
        {
            _context.TAX_PAYER.Add(taxPayer);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTaxPayer), new { id = taxPayer.TAX_PAYER_NO }, taxPayer);
        }

        // PUT: api/TaxPayer/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTaxPayer(long id, TAX_PAYER taxPayer)
        {
            if (id != taxPayer.TAX_PAYER_NO)
            {
                return BadRequest();
            }

            _context.Entry(taxPayer).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TaxPayerExists(id))
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

        // DELETE: api/TaxPayer/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTaxPayer(long id)
        {
            var taxPayer = await _context.TAX_PAYER.FindAsync(id);
            if (taxPayer == null)
            {
                return NotFound();
            }

            _context.TAX_PAYER.Remove(taxPayer);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TaxPayerExists(long id)
        {
            return _context.TAX_PAYER.Any(e => e.TAX_PAYER_NO == id);
        }
    }
}
