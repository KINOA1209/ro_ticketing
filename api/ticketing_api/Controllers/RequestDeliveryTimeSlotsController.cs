using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sieve.Models;
using Sieve.Services;
using System;
using System.Linq;
using System.Threading.Tasks;
using ticketing_api.Data;
using ticketing_api.Infrastructure;
using ticketing_api.Models;
using ticketing_api.Services;

namespace ticketing_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RequestDeliveryTimeSlotsController : BaseController.BaseController
    {
        private readonly ILogger<RequestDeliveryTimeSlotsController> _logger;

        public RequestDeliveryTimeSlotsController(ApplicationDbContext context, ILogger<RequestDeliveryTimeSlotsController> logger, IEmailSender emailSender, ISieveProcessor sieveProcessor) 
            : base(context, emailSender, sieveProcessor)
        {
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetRequestDeliveryTimeSlotsAsync([FromQuery] SieveModel sieveModel)
        {
            var query = _context.RequestDeliveryTimeSlot.AsQueryable();
            var data = await _sieveProcessor.GetPagingDataAsync(sieveModel, query);
            return Ok(data);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRequestDeliveryTimeSlot([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var requestDeliveryTimeSlot = await _context.RequestDeliveryTimeSlot.FindAsync(id);

            if (requestDeliveryTimeSlot == null)
            {
                return NotFound("RequestDeliveryTimeSlot Id not found");
            }

            return Ok(requestDeliveryTimeSlot);
        }

        [HttpPost]
        public async Task<IActionResult> PostRequestDeliveryTimeSlot([FromBody] RequestDeliveryTimeSlot requestDeliveryTimeSlot)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var countyNameExists = await _context.RequestDeliveryTimeSlot.FirstOrDefaultAsync(m => m.Name.Equals(requestDeliveryTimeSlot.Name, StringComparison.OrdinalIgnoreCase));
            if (countyNameExists != null)
            {
                return BadRequest("RequestDeliveryTimeSlot code already exists");
            }
            else
            {
                _context.RequestDeliveryTimeSlot.Add(requestDeliveryTimeSlot);
                await _context.SaveChangesAsync();
            }

            return CreatedAtAction("GetRequestDeliveryTimeSlot", new { id = requestDeliveryTimeSlot.Id }, requestDeliveryTimeSlot);
        }

        // PUT: api/Counties/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRequestDeliveryTimeSlot([FromRoute] int id, [FromBody] RequestDeliveryTimeSlot requestDeliveryTimeSlot)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != requestDeliveryTimeSlot.Id)
            {
                return BadRequest("Requested RequestDeliveryTimeSlot Id does not match with QueryString Id");
            }

            try
            {
                var countyName = _context.RequestDeliveryTimeSlot.Where(m => m.Id == requestDeliveryTimeSlot.Id).Select(m => new { m.Name });

                if (countyName.FirstOrDefault()?.Name == requestDeliveryTimeSlot.Name)
                {
                    _context.Entry(requestDeliveryTimeSlot).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                }
                else
                {
                    var countyNameExists = _context.RequestDeliveryTimeSlot.Count(m => m.Id != requestDeliveryTimeSlot.Id && m.Name.Equals(requestDeliveryTimeSlot.Name, StringComparison.OrdinalIgnoreCase));
                    if (countyNameExists > 0)
                    {
                        return BadRequest("RequestDeliveryTimeSlot Name already exists");
                    }

                    _context.Entry(requestDeliveryTimeSlot).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CountyExists(id))
                {
                    return NotFound("RequestDeliveryTimeSlot Id not found");
                }
                else
                {
                    throw;
                }
            }

            return Ok(requestDeliveryTimeSlot);
        }

        // DELETE: api/Counties/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCounty([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var requestDeliveryTimeSlot = await _context.RequestDeliveryTimeSlot.FindAsync(id);
            if (requestDeliveryTimeSlot == null)
            {
                return NotFound("RequestDeliveryTimeSlot Id not found");
            }

            //check the RequestDeliveryTimeSlot Id exist in Order 
            var countyExistOrder = _context.Order.Count(o => o.RequestDeliveryTimeSlotId == id && !o.IsDeleted);

            if (countyExistOrder > 0)
            {
                return BadRequest("Not able to delete RequestDeliveryTimeSlot as it has reference in Order table");
            }

            //// check the RequestDeliveryTimeSlot Id exist in Tax
            //var countyExistTax = _context.Tax.Count(t => t.CountyId == id && !t.IsDeleted);

            //if (countyExistTax > 0)
            //{
            //    return BadRequest("Not able to delete RequestDeliveryTimeSlot as it has reference in Tax table");
            //}

            //// check the RequestDeliveryTimeSlot Id exist in ShippingPaper
            //var countyExistShippingPaper = _context.ShippingPaper.Count(sp => sp.CountyId == id && !sp.IsDeleted);

            //if(countyExistShippingPaper > 0)
            //{
            //    return BadRequest("Not able to delete RequestDeliveryTimeSlot as it has reference in ShippingPaper table");
            //}

            //// check the RequestDeliveryTimeSlot Id exist in TicketPaper
            //var countyExistTicketPaper = _context.TicketPaper.Count(tp => tp.CountyId == id && !tp.IsDeleted);

            //if (countyExistTicketPaper > 0)
            //{
            //    return BadRequest("Not able to delete RequestDeliveryTimeSlot as it has reference in TicketPaper table");
            //}

            _context.RequestDeliveryTimeSlot.Remove(requestDeliveryTimeSlot);
            await _context.SaveChangesAsync();

            return Ok(requestDeliveryTimeSlot);
        }

        // Check RequestDeliveryTimeSlot Exist or Not
        private bool CountyExists(int id)
        {
            return _context.RequestDeliveryTimeSlot.Any(e => e.Id == id);
        }
    }
}