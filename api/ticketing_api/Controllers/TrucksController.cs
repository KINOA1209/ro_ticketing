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
    public class TrucksController : BaseController.BaseController
    {
        private readonly ILogger<TrucksController> _logger;

        public TrucksController(ApplicationDbContext context, ILogger<TrucksController> logger, IEmailSender emailSender, ISieveProcessor sieveProcessor) 
            : base(context, emailSender, sieveProcessor)
        {
            _logger = logger;
        }

        // GET: api/Trucks
        [HttpGet]
        public async Task<IActionResult> GetTruckAsync([FromQuery] SieveModel sieveModel)
        {
            var query = _context.Truck.AsQueryable();
            var data = await _sieveProcessor.GetPagingDataAsync(sieveModel, query);
            return Ok(data);
        }

        // GET: api/Trucks/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTruck([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var truck = await _context.Truck.FindAsync(id);

            if (truck == null)
            {
                return NotFound("Truck Id not found");
            }

            return Ok(truck);
        }

        // POST: api/Trucks
        [HttpPost]
        public async Task<IActionResult> PostTruck([FromBody] Truck truck)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var truckNameExists = await _context.Truck.FirstOrDefaultAsync(t => t.Name.Equals(truck.Name, StringComparison.OrdinalIgnoreCase));
            if (truckNameExists != null)
            {
                return BadRequest("Truck name already exists");
            }
            else
            {
                _context.Truck.Add(truck);
                await _context.SaveChangesAsync();
            }

            return CreatedAtAction("GetTruck", new { id = truck.Id }, truck);
        }

        // PUT: api/Trucks/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTruck([FromRoute] int id, [FromBody] Truck truck)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != truck.Id)
            {
                return BadRequest("Requested Truck Id does not match with QueryString Id");
            }

            try
            {
                var truckName = _context.Truck.Where(t => t.Id == truck.Id).Select(t => t.Name).Single();

                if (truckName == truck.Name)
                {
                    _context.Entry(truck).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                }
                else
                {
                    var truckNameExists = _context.Truck.Count(t => t.Name.Equals(truck.Name, StringComparison.OrdinalIgnoreCase) && t.Id != truck.Id);
                    if (truckNameExists > 0)
                    {
                        return BadRequest("Truck name already exists");
                    }
                    else
                    {
                        _context.Entry(truck).State = EntityState.Modified;
                        await _context.SaveChangesAsync();
                    }
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TruckExists(id))
                {
                    return NotFound("Truck Id not found");
                }
                else
                {
                    throw;
                }
            }

            return Ok(truck);
        }

        // DELETE: api/Trucks/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTruck([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var truck = await _context.Truck.FindAsync(id);
            if (truck == null)
            {
                return NotFound("Truck Id not found");
            }

            var truckExistOrder = _context.Order.Count(o => o.TruckId == id && !o.IsDeleted);

            if (truckExistOrder > 0)
            {
                return BadRequest("Not able to delete Truck as it has reference in Order table");
            }

            _context.Truck.Remove(truck);
            await _context.SaveChangesAsync();

            return Ok(truck);
        }

        // Check Trucks Exist or Not
        private bool TruckExists(int id)
        {
            return _context.Truck.Any(e => e.Id == id);
        }
    }
}