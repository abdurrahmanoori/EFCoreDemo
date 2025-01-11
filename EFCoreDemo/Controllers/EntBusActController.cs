//using EFCoreDemo.Data;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;

//namespace EFCoreDemo.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class EntBusActController : ControllerBase
//    {
//        private readonly AppDbContext _context;

//        public EntBusActController(AppDbContext context)
//        {
//            _context = context;
//        }

//        // GET: api/EntBusAct
//        [HttpGet]
//        public async Task<ActionResult<IEnumerable<ENT_BUS_ACT>>> GetEntBusActs( )
//        {
//            return await _context.ENT_BUS_ACT.ToListAsync();
//        }

//        // GET: api/EntBusAct/{entActivityNo}/{enterpriseNo}
//        [HttpGet("{entActivityNo}/{enterpriseNo}")]
//        public async Task<ActionResult<ENT_BUS_ACT>> GetEntBusAct(byte entActivityNo, long enterpriseNo)
//        {
//            var entBusAct = await _context.ENT_BUS_ACT.FindAsync(entActivityNo, enterpriseNo);

//            if (entBusAct == null)
//            {
//                return NotFound();
//            }

//            return entBusAct;
//        }

//        // POST: api/EntBusAct
//        [HttpPost]
//        public async Task<ActionResult<ENT_BUS_ACT>> CreateEntBusAct(ENT_BUS_ACT entBusAct)
//        {
//            _context.ENT_BUS_ACT.Add(entBusAct);
//            await _context.SaveChangesAsync();

//            return CreatedAtAction(nameof(GetEntBusAct), new { entActivityNo = entBusAct.ENT_ACTIVITY_NO, enterpriseNo = entBusAct.ENTERPRISE_NO }, entBusAct);
//        }

//        // PUT: api/EntBusAct/{entActivityNo}/{enterpriseNo}
//        [HttpPut("{entActivityNo}/{enterpriseNo}")]
//        public async Task<IActionResult> UpdateEntBusAct(byte entActivityNo, long enterpriseNo, ENT_BUS_ACT entBusAct)
//        {
//            if (entActivityNo != entBusAct.ENT_ACTIVITY_NO || enterpriseNo != entBusAct.ENTERPRISE_NO)
//            {
//                return BadRequest();
//            }

//            _context.Entry(entBusAct).State = EntityState.Modified;

//            try
//            {
//                await _context.SaveChangesAsync();
//            }
//            catch (DbUpdateConcurrencyException)
//            {
//                if (!EntBusActExists(entActivityNo, enterpriseNo))
//                {
//                    return NotFound();
//                }
//                else
//                {
//                    throw;
//                }
//            }

//            return NoContent();
//        }

//        // DELETE: api/EntBusAct/{entActivityNo}/{enterpriseNo}
//        [HttpDelete("{entActivityNo}/{enterpriseNo}")]
//        public async Task<IActionResult> DeleteEntBusAct(byte entActivityNo, long enterpriseNo)
//        {
//            var entBusAct = await _context.ENT_BUS_ACT.FindAsync(entActivityNo, enterpriseNo);
//            if (entBusAct == null)
//            {
//                return NotFound();
//            }

//            _context.ENT_BUS_ACT.Remove(entBusAct);
//            await _context.SaveChangesAsync();

//            return NoContent();
//        }

//        private bool EntBusActExists(byte entActivityNo, long enterpriseNo)
//        {
//            return _context.ENT_BUS_ACT.Any(e => e.ENT_ACTIVITY_NO == entActivityNo && e.ENTERPRISE_NO == enterpriseNo);
//        }
//    }
//}
