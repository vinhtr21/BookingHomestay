using Go2Hotel.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Go2Hotel.Controllers
{
    [EnableCors("AllowAllOrigins")]
    [Route("api/[controller]")]
    [ApiController]
    public class HomestaysController : ControllerBase
    {
        private readonly Booking_HomestayContext _context;

        public HomestaysController(Booking_HomestayContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<Homestay>>> GetListAvailableHomestay()
        {
            if (_context.Homestays == null)
            {
                return NotFound();
            }
            var homestay = await _context.Homestays.Where(x=>x.HomestayStatus==true).ToListAsync();

            if (homestay == null)
            {
                return NotFound();
            }

            return homestay;
        }

        [HttpGet("OwnerHomestay/{ownerId}")]
        public async Task<ActionResult<List<Homestay>>> GetListHomestayOfOwner(int ownerId)
        {
            if (_context.Homestays == null)
            {
                return NotFound();
            }
            var homestay = await _context.Homestays.Where(x => x.OwnerId == ownerId).ToListAsync();

            if (homestay == null)
            {
                return NotFound();
            }

            return homestay;
        }

        [HttpGet("getByTypeId")]
        public async Task<ActionResult<List<TypeHomestay>>> GetTypeHomestay(int id) 
        {
           var typeName = await _context.TypeHomestays.SingleOrDefaultAsync(x=>x.TypeId==id);
            return Ok(typeName);
        }


        [HttpGet("listAll")]
        public async Task<IActionResult> GetAllHomestay()
        {
            if (_context.Homestays == null)
            {
                return NotFound();
            }
            var homestay = await _context.Homestays.Include(x => x.Images)
                .Select(x => new
                {
                    x.HomestayId,
                    x.HomestayName,
                    x.HomestayType,
                    x.HomestaySodo,
                    x.HomestayCity,
                    x.HomestayDescription,
                    x.HomestayBedroom,
                    x.HomestayRegion, x.HomestayCountry,
                    x.HomestayStreet,
                    x.HomestayStatus,
                    Image = x.Images.FirstOrDefault()
                })
                .ToListAsync();

            if (homestay == null)
            {
                return NotFound();
            }

            return Ok(homestay);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<List<Homestay>>> GetHomestayById([FromRoute] long id)
        {
            if (_context.Homestays == null)
            {
                return NotFound();
            }
            var homestay = await _context.Homestays.Where(x => x.HomestayId == id).ToListAsync();

            if (homestay == null)
            {
                return NotFound();
            }

            return homestay;
        }

        [HttpPost("Create")]
        public async Task<ActionResult<Homestay>> CreateHomestay([FromBody] Homestay homestay)
        {
            try
            {
                var type = await _context.TypeHomestays.FirstOrDefaultAsync(x => x.TypeId == homestay.HomestayType);

                if (type == null)
                {
                    return BadRequest("Invalid Type Homestay!");
                }

                homestay.HomestayStatus = true;
                homestay.HomestaySodo = 0;
                _context.Add(homestay);
                _context.SaveChanges();
                return CreatedAtAction(nameof(GetHomestayById), new { id = homestay.HomestayId }, homestay);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Homestay>> UpdateHomestayInfor([FromRoute] int id, [FromBody] Homestay homestay)
        {
            var type = await _context.TypeHomestays.FirstOrDefaultAsync(x => x.TypeId == homestay.HomestayType);

            if (type == null)
            {
                return BadRequest("Invalid Type Homestay!");
            }

            homestay.HomestayId = id;
            _context.Entry(homestay).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HomestayExist(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return homestay;
        }

        [HttpPut("status/{id}")]
        public async Task<IActionResult> DisableHomestayStatus([FromRoute] int id)
        {
            if (_context.Homestays == null)
            {
                return NotFound();
            }
            var home = await _context.Homestays.FindAsync(id);

            if (home == null)
            {
                return NotFound();
            }

            if (home.HomestayStatus == true)
            {
                home.HomestayStatus = false;
            }
            else
            {
                home.HomestayStatus = true;
            }
            _context.Entry(home).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut("sodo/{id}")]
        public async Task<IActionResult> ChangeSodoHomestay([FromRoute] int id)
        {
            if (_context.Homestays == null)
            {
                return NotFound();
            }
            var home = await _context.Homestays.FindAsync(id);

            if (home == null)
            {
                return NotFound();
            }

            if (home.HomestaySodo == 1)
            {
                home.HomestaySodo = 0;
            }
            else
            {
                home.HomestaySodo = 1;
            }
            _context.Entry(home).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool HomestayExist(long id)
        {
            return (_context.Homestays?.Any(e => e.HomestayId == id)).GetValueOrDefault();
        }
    }
}

