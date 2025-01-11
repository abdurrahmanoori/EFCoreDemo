using AutoMapper;
using EFCoreDemo.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EFCoreDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaxPayerController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public TaxPayerController(AppDbContext context,IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }

        // GET: api/TaxPayer
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaxPayer>>> GetTaxPayers( )
        {
            return await _context.TaxPayers
                .Include(tp => tp.Enterprises) // Include related enterprises
                .ToListAsync();
        }

        // GET: api/TaxPayer/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TaxPayer>> GetTaxPayer(long id)
        {
            var taxPayer = await _context.TaxPayers
                .Include(tp => tp.Enterprises)
                .FirstOrDefaultAsync(tp => tp.TaxPayerId == id);

            if (taxPayer == null)
            {
                return NotFound();
            }

            return taxPayer;
        }

        // POST: api/TaxPayer
        [HttpPost]
        public async Task<ActionResult<TaxPayer>> CreateTaxPayer(TaxPayerDto taxPayerDto)
        {

            //var taxPayerDto = _mapper.Map<TaxPayerDto>(taxPayerEntity); // Entity to DTO
            var taxPayerEntity = _mapper.Map<TaxPayer>(taxPayerDto);   // DTO to Entity



            _context.TaxPayers.Add(taxPayerEntity);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTaxPayer), new { id = taxPayerEntity.TaxPayerId },taxPayerDto);
        }

        // PUT: api/TaxPayer/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTaxPayer(long id, TaxPayer taxPayer)
        {
            if (id != taxPayer.TaxPayerId)
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
            var taxPayer = await _context.TaxPayers.FindAsync(id);
            if (taxPayer == null)
            {
                return NotFound();
            }

            _context.TaxPayers.Remove(taxPayer);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TaxPayerExists(long id)
        {
            return _context.TaxPayers.Any(e => e.TaxPayerId == id);
        }
    }
}
