using Go2Hotel.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;

namespace Go2Hotel.Controllers
{
    [EnableCors("AllowAllOrigins")]
    [Route("api/[controller]")]
    [ApiController]
    public class FilterController : ControllerBase
    {
        private readonly Booking_HomestayContext _context;
        public FilterController(Booking_HomestayContext context)
        {
            _context = context;
        }

        [HttpGet("filterAll")]
        public IActionResult GetAllType()
        {
            if (_context.TypeHomestays == null)
            {
                return NotFound();
            }
            var types = _context.TypeHomestays.ToList();

            if (types == null)
            {
                return NotFound();
            }

            List<TypeForm> typeForms = new List<TypeForm>();
            foreach (var t in types)
            {
                TypeForm form = new TypeForm();
                form.TypeId = t.TypeId;
                form.TypeName = t.TypeName;
                switch (t.TypeId)
                {
                    case 1:
                        form.TypeSize = ">80 m2";
                        form.TypeBedRoom = ">4 phòng";
                        form.TypeService = "Wifi, TV, Bồn Tắm, Nhà Bếp...";
                        form.TypeImg = "img/room/room-b1.jpg";
                        break;
                    case 2:
                        form.TypeSize = ">60 m2";
                        form.TypeBedRoom = ">3 phòng";
                        form.TypeService = "Wifi, TV, Bồn Tắm, Nhà Bếp...";
                        form.TypeImg = "img/room/room-b2.jpg";
                        break;
                    case 3:
                        form.TypeSize = "> 40 m2";
                        form.TypeBedRoom = ">2 phòng";
                        form.TypeService = "Thiết bị hỗ trợ quay, chụp hình ,...";
                        form.TypeImg = "img/room/room-b3.jpg";
                        break;
                    case 4:
                        form.TypeSize = ">50 m2";
                        form.TypeBedRoom = ">2 phòng";
                        form.TypeService = "Wifi, TV, Bồn Tắm, Nhà Bếp...";
                        form.TypeImg = "img/room/room-b4.jpg";
                        break;
                }
                typeForms.Add(form);
            }

            return Ok(typeForms);
        }

        [HttpGet("filterType/{id}")]
        public async Task<ActionResult<List<Homestay>>> GetListAvailableHomestay(int id)
        {
            if (_context.Homestays == null)
            {
                return NotFound();
            }
            var homestay = await _context.Homestays.Where(x => x.HomestayStatus == true && x.HomestayType == id).ToListAsync();

            if (homestay == null)
            {
                return NotFound();
            }

            return homestay;
        }

        [HttpPost("Search")]
        public IActionResult searchHomestays([FromBody] SearchForm searchCriteria)
        {
            var homestays = _context.Homestays.Where(x => x.HomestayStatus == true).ToList();

            var filteredHomestays = homestays.FindAll(homestay =>
            (string.IsNullOrEmpty(searchCriteria.HomestayCountry) || homestay.HomestayCountry.Equals(searchCriteria.HomestayCountry, StringComparison.OrdinalIgnoreCase)) &&
            (string.IsNullOrEmpty(searchCriteria.HomestayCity) || homestay.HomestayCity.Equals(searchCriteria.HomestayCity, StringComparison.OrdinalIgnoreCase)) &&
            (string.IsNullOrEmpty(searchCriteria.HomestayRegion) || homestay.HomestayRegion.Equals(searchCriteria.HomestayRegion, StringComparison.OrdinalIgnoreCase)) &&
            (searchCriteria.HomestayBedroom == 0 || homestay.HomestayBedroom >= searchCriteria.HomestayBedroom)
        );
            if(filteredHomestays.Any())
            {
                return Ok(filteredHomestays);
            }
            else
            {
                return NotFound();
            }
        }
    }
}   
