using System;
using System.Collections.Generic;
using System.Linq;
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

namespace ticketing_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentTermsController : BaseController.BaseController
    {
        private readonly ILogger<PaymentTermsController> _logger;

        public PaymentTermsController(ApplicationDbContext context, ILogger<PaymentTermsController> logger, IEmailSender emailSender, ISieveProcessor sieveProcessor) : base(context, emailSender, sieveProcessor)
        {
            _logger = logger;
        }

        // GET: api/PaymentTerms
        [HttpGet]
        public async Task<IActionResult> GetPaymentTermAsync([FromQuery]SieveModel sieveModel)
        {
            var query = _context.PaymentTerm.AsQueryable();
            var data = await _sieveProcessor.GetPagingDataAsync(sieveModel, query);
            return Ok(data);
        }

        // GET: api/PaymentTerms/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPaymentTerm([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var paymentTerms = await _context.PaymentTerm.FindAsync(id);

            if (paymentTerms == null)
            {
                return NotFound("PaymentTerm Id not found");
            }

            return Ok(paymentTerms);
        }

        // POST: api/PaymentTerms
        [HttpPost]
        public async Task<IActionResult> PostPaymentTermAsync([FromBody] PaymentTerm paymentTerm)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var paymentTermNameExists = await _context.PaymentTerm.FirstOrDefaultAsync(u => u.Name.Equals(paymentTerm.Name, StringComparison.OrdinalIgnoreCase));
            if (paymentTermNameExists != null)
                return BadRequest("PaymentTerm name already exists");

            _context.PaymentTerm.Add(paymentTerm);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPaymentTerm", new { id = paymentTerm.Id }, paymentTerm);
        }

        // PUT: api/PaymentTerms/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPaymentTermAsync([FromRoute] int id, [FromBody] PaymentTerm paymentTerm)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != paymentTerm.Id)
            {
                return BadRequest("Requested PaymentTerm Id does not match with QueryString Id");
            }

            try
            {
                var paymentTermName = _context.PaymentTerm.Where(u => u.Id == paymentTerm.Id).Select(u => u.Name).Single();

                if (paymentTermName == paymentTerm.Name)
                {
                    _context.Entry(paymentTerm).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                }
                else
                {
                    var paymentTermNameExists = _context.PaymentTerm.Count(u => u.Id != paymentTerm.Id && u.Name.Equals(paymentTerm.Name, StringComparison.OrdinalIgnoreCase));
                    if (paymentTermNameExists > 0)
                    {
                        return BadRequest("PaymentTerm name already exists");
                    }
                    _context.Entry(paymentTerm).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PaymentTermExists(id))
                {
                    return NotFound("PaymentTerm Id not found");
                }
                else
                {
                    throw;
                }
            }

            return Ok(paymentTerm);
        }

        // DELETE: api/PaymentTerms/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePaymentTermAsync([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var paymentTerm = await _context.PaymentTerm.FindAsync(id);
            if (paymentTerm == null)
            {
                return NotFound("PaymentTerm Id not found");
            }

            // check the PaymentTerm Id exist in Customer
            var paymentTermExistCustomer = _context.Customer.Count(c => c.PaymentTermId == id && !c.IsDeleted);

            if (paymentTermExistCustomer > 0)
            {
                return BadRequest("Not able to delete PaymentTerm as it has reference in Customer table");
            }

            _context.PaymentTerm.Remove(paymentTerm);
            await _context.SaveChangesAsync();

            return Ok(paymentTerm);
        }

        // Check paymentTerm Exist or Not
        private bool PaymentTermExists(int id)
        {
            return _context.PaymentTerm.Any(e => e.Id == id);
        }
    }
}
