using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sieve.Models;
using Sieve.Services;
using ticketing_api.Data;
using ticketing_api.Infrastructure;
using ticketing_api.Models;
using ticketing_api.Models.Views;
using ticketing_api.Services;

namespace ticketing_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RigLocationNotesController : BaseController.BaseController
    {
        private readonly ILogger<RigLocationNotesController> _logger;
        private readonly LogService _rigLocationNotesLogService;
        private readonly RigLocationNotesService _rigLocationNoteService;

        public RigLocationNotesController(ApplicationDbContext context, ILogger<RigLocationNotesController> logger, IEmailSender emailSender, ISieveProcessor sieveProcessor) : base(context, emailSender, sieveProcessor)
        {
            _logger = logger;
            _rigLocationNotesLogService = new LogService(_context, sieveProcessor);
            _rigLocationNoteService = new RigLocationNotesService(_context, sieveProcessor);
        }

        // GET: api/RigLocationNotes
        [HttpGet]
        public async Task<IActionResult> GetRigLocationNoteAsync([FromQuery]SieveModel sieveModel)
        {
            PagingResults<RigLocationNoteView> rigLocationNote = await _rigLocationNoteService.GetRigLocationNotesViewAsync(sieveModel);

            return Ok(rigLocationNote);        
        }

        // GET: api/RigLocationNotes/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRigLocationNote([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var rigLocationNote = await _context.RigLocationNote.FindAsync(id);

            if (rigLocationNote == null)
            {
                return NotFound("RigLocation Note Id not found");
            }
                               
            return Ok(rigLocationNote);
        }

        // GET: api/RigLocationNotes/5
        [HttpGet("latestNote/{rigLocationId}")]
        public async Task<IActionResult> GetLatestRigLocationNoteSelectedRigLocationid([FromRoute] int rigLocationId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var rigLocationNote = await _context.RigLocationNote.Where(rignote => rignote.RigLocationId == rigLocationId).ToListAsync();
            RigLocationNote latestRigLocationNote;
            if (rigLocationNote == null)
            {
                return BadRequest("RigLocation Note Id not found");
            }
            else
            {
                latestRigLocationNote = rigLocationNote.OrderByDescending(created => created.CreatedAt).FirstOrDefault();
            }

            return Ok(latestRigLocationNote);
        }
        // POST: api/RigLocationNotes
        [HttpPost]
        public async Task<IActionResult> PostRigLocationNote([FromBody] RigLocationNote rigLocationNote)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.RigLocationNote.Add(rigLocationNote);
            await _context.SaveChangesAsync();

            RigLocationNoteView rigLocationNoteResult = _rigLocationNoteService.PostRigLocationNote(rigLocationNote);

            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var orderId = rigLocationNoteResult.Id;
            // await _rigLocationNotesLogService.StoreRigLocationNotesLogInformation(orderId, "PostRigLocationNote", userId);

            return CreatedAtAction("GetRigLocationNote", new { id = rigLocationNote.Id }, rigLocationNoteResult);
        }

        // PUT: api/RigLocationNotes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRigLocationNote([FromRoute] int id, [FromBody] RigLocationNote rigLocationNote)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != rigLocationNote.Id)
            {
                return BadRequest("Requested RigLocationNote Id does not match with QueryString Id");
            }

            RigLocationNote rigLocationNoteUpdateBefore = _context.RigLocationNote.AsNoTracking().FirstOrDefault(o => o.Id == id);

            try
            {
                _context.Entry(rigLocationNote).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RigLocationNoteExists(id))
                {
                    return NotFound("RigLocation Note Id not found");
                }
                else
                {
                    throw;
                }
            }

            RigLocationNoteView rigLocationNoteResult = _rigLocationNoteService.PostRigLocationNote(rigLocationNote);
                     
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            //await _rigLocationNotesLogService.StoreRigLocationNoteLoginformationPut(rigLocationNoteResult, "PutRigLocationNote", userId, rigLocationNoteUpdateBefore);

            return Ok(rigLocationNoteResult);
        }

        // DELETE: api/RigLocationNotes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRigLocationNote([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var rigLocationNote = await _context.RigLocationNote.FindAsync(id);
            if (rigLocationNote == null)
            {
                return NotFound("RigLocation Note Id not found");
            }

            _context.RigLocationNote.Remove(rigLocationNote);
            await _context.SaveChangesAsync();
                      
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            //await _rigLocationNotesLogService.StoreRigLocationNotesLogInformation(id, "DeleteRigLocationNote", userId);
            return Ok(rigLocationNote);
        }

        private bool RigLocationNoteExists(int id)
        {
            return _context.RigLocationNote.Any(e => e.Id == id);
        }


    }
}
