using Go2Hotel.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Go2Hotel.Controllers
{
    [EnableCors("AllowAllOrigins")]
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly Booking_HomestayContext _context;

        public CommentController(Booking_HomestayContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<List<Comment>>> GetListCommentByGuestId([FromRoute] long id)
        {
            if (_context.Comments == null)
            {
                return NotFound();
            }
            var listComment = await _context.Comments.Where(x => x.GuestId == id).ToListAsync();

            if (listComment == null)
            {
                return NotFound();
            }

            return listComment;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateComment([FromBody] Comment comment)
        {
            try
            {
                var guestComment = await _context.Guests.FirstOrDefaultAsync(x => x.GuestId == comment.GuestId);

                if (guestComment == null)
                {
                    return BadRequest("Not Found!");
                }

                comment.CommentDate = DateTime.Now;
                _context.Add(comment);
                _context.SaveChanges();
                return RedirectToPage("./Index"); // chuyển về comment page
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
    }
}
