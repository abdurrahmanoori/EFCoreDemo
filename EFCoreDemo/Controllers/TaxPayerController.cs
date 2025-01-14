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

        public TaxPayerController(AppDbContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }

        // GET: api/TaxPayer
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaxPayerDto>>> GetTaxPayers( )
        {
            var taxPayers = await _context.TaxPayers
                .Include(tp => tp.Enterprises)
                .ThenInclude(x => x.EnterpriseBusinessActivities)// Include related enterprises
                .ToListAsync();

            var taxPayersDto = _mapper.Map<IEnumerable<TaxPayerDto>>(taxPayers);

            return Ok(taxPayersDto);
        }

        // GET: api/TaxPayer/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TaxPayerDto>> GetTaxPayer(long id)
        {
            var taxPayer = await _context.TaxPayers
                .Include(tp => tp.Enterprises)
                .ThenInclude(x => x.EnterpriseBusinessActivities)
                .ThenInclude(x=>x.collClasses)
                .FirstOrDefaultAsync(tp => tp.TaxPayerId == id);

            if (taxPayer == null)
            {
                return NotFound();
            }

            var taxpayerDto = _mapper.Map<TaxPayerDto>(taxPayer);

            return taxpayerDto;
        }

        // POST: api/TaxPayer
        [HttpPost]
        public async Task<ActionResult<TaxPayer>> CreateTaxPayer(TaxPayerDto taxPayerDto)
        {

            //var taxPayerDto = _mapper.Map<TaxPayerDto>(taxPayerEntity); // Entity to DTO
            var taxPayerEntity = _mapper.Map<TaxPayer>(taxPayerDto);   // DTO to Entity



            _context.TaxPayers.Add(taxPayerEntity);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTaxPayer), new { id = taxPayerEntity.TaxPayerId }, taxPayerDto);
        }

        // PUT: api/TaxPayer/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTaxPayer(long id, TaxPayerDto taxPayerDto)
        {
            if (id != taxPayerDto.TaxPayerId)
            {
                return BadRequest();
            }

            var taxPayer = await _context.TaxPayers
                .Include(x => x.Enterprises!).ThenInclude(x => x.EnterpriseBusinessActivities!)
                .Include(x => x.Enterprises!).ThenInclude(x => x.EnterpriseBusinessActivities!).ThenInclude(x=>x.collClasses)
                .FirstOrDefaultAsync(x => x.TaxPayerId == id);

            //ManualConvert(taxPayerDto, taxPayer);

            _mapper.Map(taxPayerDto, taxPayer);
            //taxPayer.Enterprises.FirstOrDefault().EnterpriseName = "dsfsd";

            var entities = GetChangeTracker(_context);

            await _context.SaveChangesAsync();
            return Ok();
            //_context.Entry(taxPayer).State = EntityState.Modified;

            //try
            //{
            //    await _context.SaveChangesAsync();
            //}
            //catch (DbUpdateConcurrencyException)
            //{
            //    if (!TaxPayerExists(id))
            //    {
            //        return NotFound();
            //    }
            //    else
            //    {
            //        throw;
            //    }
            //}

            return NoContent();
        }

        private void ManualConvert(TaxPayerDto taxPayerDto, TaxPayer? taxPayer)
        {
            if (taxPayer == null) return;

            // Map the main properties of TaxPayer
            taxPayer.TaxPayerId = taxPayerDto.TaxPayerId;
            taxPayer.TaxPayerName = taxPayerDto.TaxPayerName;

            // Check if Enterprises collection exists
            if (taxPayerDto.Enterprises != null)
            {
                taxPayer.Enterprises = new List<Enterprise>();

                foreach (var enterpriseDto in taxPayerDto.Enterprises)
                {
                    var enterprise = new Enterprise
                    {
                        EnterpriseId = enterpriseDto.EnterpriseId,
                        EnterpriseName = enterpriseDto.EnterpriseName,
                        TaxPayerId = enterpriseDto.TaxPayerId,
                        EnterpriseBusinessActivities = enterpriseDto.EnterpriseBusinessActivities?.Select(ebaDto => new EnterpriseBusinessActivity
                        {
                            ActivityId = ebaDto.ActivityId,
                            EnterpriseId = ebaDto.EnterpriseId,
                            MainActivityFlag = ebaDto.MainActivityFlag
                        }).ToList()
                    };

                    taxPayer.Enterprises.Add(enterprise);
                }
            }
        }

        private object GetChangeTracker(AppDbContext context)
        {
            var entities = context.ChangeTracker.Entries();
            return new
            {
                AddedEntities = context.ChangeTracker.Entries()
                    .Where(x => x.State == EntityState.Added)
                    .Select(e => new
                    {
                        Entity = e.Entity,
                        EntityName = e.Entity.GetType().Name,
                        State = e.State.ToString(),
                    }).ToList(),

                ModifiedEntities = context.ChangeTracker.Entries()
                    .Where(x => x.State == EntityState.Modified)
                    .Select(e => new
                    {
                        Entity = e.Entity,
                        EntityName = e.Entity.GetType().Name,
                        State = e.State.ToString(),
                        ChangedProperties = e.Properties
                            .Where(p => p.IsModified)
                            .Select(p => new
                            {
                                PropertyName = p.Metadata.Name,
                                OriginalValue = p.OriginalValue?.ToString() ?? "null",
                                CurrentValue = p.CurrentValue?.ToString() ?? "null",
                                IsModified = p.IsModified,
                            }).ToList(),
                    }).ToList(),

                DeletedEntities = context.ChangeTracker.Entries()
                    .Where(x => x.State == EntityState.Deleted)
                    .Select(e => new
                    {
                        Entity = e.Entity,
                        EntityName = e.Entity.GetType().Name,
                        State = e.State.ToString(),
                    }).ToList(),

                UnchangedEntities = context.ChangeTracker.Entries()
                    .Where(x => x.State == EntityState.Unchanged)
                    .Select(e => new
                    {
                        Entity = e.Entity,
                        EntityName = e.Entity.GetType().Name,
                        State = e.State.ToString(),
                    }).ToList(),

                DetachedEntities = context.ChangeTracker.Entries()
                    .Where(x => x.State == EntityState.Detached)
                    .Select(e => new
                    {
                        Entity = e.Entity,
                        State = e.State.ToString(),
                        EntityName = e.Entity.GetType().Name,
                    }).ToList(),
            };

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
            // await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TaxPayerExists(long id)
        {
            return _context.TaxPayers.Any(e => e.TaxPayerId == id);
        }
    }
}
