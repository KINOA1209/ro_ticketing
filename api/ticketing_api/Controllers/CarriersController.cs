using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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
    public class CarriersController : BaseController.BaseController
    {
        private readonly ILogger<CarriersController> _logger;

        public CarriersController(ApplicationDbContext context, ILogger<CarriersController> logger, IEmailSender emailSender, ISieveProcessor sieveProcessor)
            : base(context, emailSender, sieveProcessor)
        {
            _logger = logger;
        }

        // GET: api/Carriers
        [HttpGet]
        public async Task<IActionResult> GetCarrierAsync([FromQuery] SieveModel sieveModel)
        {
            var query = _context.Carrier.AsQueryable();
            var data = await _sieveProcessor.GetPagingDataAsync(sieveModel, query);
            return Ok(data);
        }

        // GET: api/Carriers/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCarrier([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var carrier = await _context.Carrier.FindAsync(id);

            if (carrier == null)
            {
                return NotFound("Carrier Id not found");
            }

            return Ok(carrier);
        }

        // POST: api/Carriers
        [HttpPost]
        public async Task<IActionResult> PostCarrier([FromBody] Carrier carrier)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var carrierNameExists = await _context.Carrier.FirstOrDefaultAsync(t => t.Name.Equals(carrier.Name, StringComparison.OrdinalIgnoreCase));
            if (carrierNameExists != null)
            {
                return BadRequest("Carrier name already exists");
            }
            _context.Carrier.Add(carrier);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCarrier", new { id = carrier.Id }, carrier);
        }

        // PUT: api/Carriers/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCarrier([FromRoute] int id, [FromBody] Carrier carrier)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != carrier.Id)
            {
                return BadRequest("Requested Carrier Id does not match with QueryString Id");
            }

            try
            {
                var carrierName = _context.Carrier.Where(t => t.Id == carrier.Id).Select(t => t.Name).Single();

                if (carrierName == carrier.Name)
                {
                    _context.Entry(carrier).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                }
                else
                {
                    var carrierNameExists = _context.Carrier.Count(t => t.Name.Equals(carrier.Name, StringComparison.OrdinalIgnoreCase) && t.Id != carrier.Id);
                    if (carrierNameExists > 0)
                    {
                        return BadRequest("Carrier name already exists");
                    }
                    else
                    {
                        _context.Entry(carrier).State = EntityState.Modified;
                        await _context.SaveChangesAsync();
                    }
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CarrierExists(id))
                {
                    return NotFound("Carrier Id not found");
                }
                else
                {
                    throw;
                }
            }

            return Ok(carrier);
        }

        // DELETE: api/Carriers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCarrier([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var carrier = await _context.Carrier.FindAsync(id);
            if (carrier == null)
            {
                return NotFound("Carrier Id not found");
            }

            var carrierExistOrder = _context.Carrier.Count(o => o.Id == id && !o.IsDeleted);

            if (carrierExistOrder > 0)
            {
                return BadRequest("Not able to delete Carrier as it has reference in Order table");
            }

            _context.Carrier.Remove(carrier);
            await _context.SaveChangesAsync();

            return Ok(carrier);
        }

        // Check Carriers Exist or Not
        private bool CarrierExists(int id)
        {
            return _context.Carrier.Any(e => e.Id == id);
        }
    }
}