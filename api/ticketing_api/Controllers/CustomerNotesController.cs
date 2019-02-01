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
    public class CustomerNotesController : BaseController.BaseController
    {
        private readonly ILogger<CustomerNotesController> _logger;
        private readonly LogService _customerNoteLogService;
        private readonly CustomerNotesService _customerNoteService;

        public CustomerNotesController(ApplicationDbContext context, ILogger<CustomerNotesController> logger, IEmailSender emailSender, ISieveProcessor sieveProcessor) : base(context, emailSender, sieveProcessor)
        {
            _logger = logger;
            _customerNoteLogService = new LogService(_context, sieveProcessor);
            _customerNoteService = new CustomerNotesService(_context, sieveProcessor);
        }

        // GET: api/CustomerNotes
        [HttpGet]
        public async Task<IActionResult> GetCustomerNotesAsync([FromQuery]SieveModel sieveModel)
        {
            var customerNote = await _customerNoteService.GetCustomerNotesViewAsync(sieveModel);
            return Ok(customerNote);
        }

        // GET: api/CustomerNotes/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCustomerNotes([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var customerNote = await _context.CustomerNote.FindAsync(id);

            if (customerNote == null)
            {
                return NotFound("Customer Note Id not found");
            }

            return Ok(customerNote);
        }

        // GET: api/CustomerNotes/5
        [HttpGet("latestNote/{customerId}")]
        public async Task<IActionResult> GetLatestCustomerNoteSelectedCustomerid([FromRoute] int customerId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var customerNote = await _context.CustomerNote.Where(x => x.CustomerId == customerId).ToListAsync();
            if (customerNote == null)
            {
                return Content("Customer Note Id not found");
            }

            var latestCustomerNote = customerNote.OrderByDescending(created => created.CreatedAt).FirstOrDefault();

            return Ok(latestCustomerNote);
        }

        // POST: api/CustomerNotes
        [HttpPost]
        public async Task<IActionResult> PostCustomerNotes([FromBody] CustomerNote customerNote)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            customerNote.CustomerNotes = customerNote.CustomerNotes.ToUpper();
            _context.CustomerNote.Add(customerNote);
            await _context.SaveChangesAsync();

            CustomerNoteView customerNoteResult = _customerNoteService.PostCustomerNote(customerNote);

            //var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            //var orderId = customerNote.Id;
            //await _customerNoteLogService.StoreCustomerNotesLogInformation(orderId, "PostCustomerNotes", userId);
            return CreatedAtAction("GetCustomerNotes", new { id = customerNote.Id }, customerNoteResult);
        }

        // PUT: api/CustomerNotes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCustomerNotes([FromRoute] int id, [FromBody] CustomerNote customerNote)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != customerNote.Id)
            {
                return BadRequest("Requested CustomerNote Id does not match with QueryString Id");
            }

            //var customerNoteUpdateBefore = _context.CustomerNote.AsNoTracking().FirstOrDefault(o => o.Id == id);

            try
            {
                customerNote.CustomerNotes = customerNote.CustomerNotes.ToUpper();
                _context.Entry(customerNote).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerNotesExists(id))
                {
                    return NotFound("Customer Note Id not found");
                }

                throw;
            }

            CustomerNoteView customerNoteResult = _customerNoteService.PostCustomerNote(customerNote);

            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            //await _customerNoteLogService.StoreCustomerNoteLogInformationPut(customerNoteResult, "PutCustomerNotes", userId, customerNoteUpdateBefore);

            return Ok(customerNoteResult);
        }

        // DELETE: api/CustomerNotes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomerNotes([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var customerNote = await _context.CustomerNote.FindAsync(id);
            if (customerNote == null)
            {
                return NotFound("Customer Note Id not found");
            }

            _context.CustomerNote.Remove(customerNote);
            await _context.SaveChangesAsync();

            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            // await _customerNoteLogService.StoreCustomerNotesLogInformation(id, "DeleteCustomerNotes", userId);
            return Ok(customerNote);
        }

        // Check CustomerNotes Exist or Not
        private bool CustomerNotesExists(int id)
        {
            return _context.CustomerNote.Any(e => e.Id == id);
        }


    }
}
