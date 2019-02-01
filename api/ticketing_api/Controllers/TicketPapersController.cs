using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sieve.Models;
using Sieve.Services;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using ticketing_api.Data;
using ticketing_api.Infrastructure;
using ticketing_api.Models;
using ticketing_api.Services;

namespace ticketing_api.Controllers
{
    /// <summary>
    /// Manage ticket paper content
    /// </summary>
    //[Authorize(Policy = "AdminOnly")]
    [Route("api/[controller]")]
    [ApiController]
    public class TicketPapersController : BaseController.BaseController
    {
        private readonly ILogger<TicketPapersController> _logger;
        private readonly TicketPaperService _ticketPaperService;

        public TicketPapersController(ApplicationDbContext context, ILogger<TicketPapersController> logger, IEmailSender emailSender, ISieveProcessor sieveProcessor)
            : base(context, emailSender, sieveProcessor)
        {
            _logger = logger;
             _ticketPaperService = new TicketPaperService(_context, _sieveProcessor);
        }

        //[Authorize(Policy = "AdminOnly")]
        [HttpGet]
        public async Task<IActionResult> GetTicketPaper([FromQuery] SieveModel sieveModel)
        {
            var data = await _ticketPaperService.GetListViewAsync(sieveModel);
            return Ok(data);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTicketPaper([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var ticketPaper = await _context.TicketPaper.FindAsync(id);

            if (ticketPaper == null)
            {
                return NotFound("Ticket paper id not found");
            }

            return Ok(ticketPaper);
        }

        [HttpPost]
        public async Task<IActionResult> PostTicketPaper([FromBody] TicketPaper ticketPaper)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //check if ticket paper for market already exists

            _context.TicketPaper.Add(ticketPaper);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTicketPaper", new { id = ticketPaper.Id }, ticketPaper);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutTicketPaper([FromRoute] int id, [FromBody] TicketPaper ticketPaper)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != ticketPaper.Id)
            {
                return BadRequest("Requested market id does not match with query id");
            }

            _context.Entry(ticketPaper).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(ticketPaper);
        }

        //[Authorize(Policy = "AdminOnly")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTicketPaper([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var ticketPaper = await _context.TicketPaper.FindAsync(id);
            if (ticketPaper == null)
            {
                return NotFound("Ticket paper id not found");
            }

            _context.TicketPaper.Remove(ticketPaper);
            await _context.SaveChangesAsync();

            return Ok(ticketPaper);
        }
    }
}