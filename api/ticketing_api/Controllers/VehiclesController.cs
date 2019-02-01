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
    public class VehiclesController : BaseController.BaseController
    {
        private readonly ILogger<VehiclesController> _logger;

        public VehiclesController(ApplicationDbContext context, ILogger<VehiclesController> logger, IEmailSender emailSender, ISieveProcessor sieveProcessor) : base(context, emailSender, sieveProcessor)
        {
            _logger = logger;
        }

        // GET: api/Vehicles
        [HttpGet]
        public async Task<IActionResult> GetVehicleAsync([FromQuery] SieveModel sieveModel)
        {
            var query = _context.Vehicle.AsQueryable();
            var data = await _sieveProcessor.GetPagingDataAsync(sieveModel, query);
            return Ok(data);
        }

        // GET: api/Vehicles/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetVehicle([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var vehicle = await _context.Vehicle.FindAsync(id);

            if (vehicle == null)
            {
                return NotFound("Vehicle Id not found");
            }

            return Ok(vehicle);
        }

        // POST: api/Vehicles
        [HttpPost]
        public async Task<IActionResult> PostVehicle([FromBody] Vehicle vehicle)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var vehicleNameExists = await _context.Vehicle.SingleOrDefaultAsync(v => v.Name.Equals(vehicle.Name, StringComparison.OrdinalIgnoreCase));
            if (vehicleNameExists != null)
            {
                return BadRequest("vehicle name already exists");
            }
            else
            {
                _context.Vehicle.Add(vehicle);
                await _context.SaveChangesAsync();
            }

            return CreatedAtAction("GetVehicle", new { id = vehicle.Id }, vehicle);
        }
        // PUT: api/Vehicles/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutVehicle([FromRoute] int id, [FromBody] Vehicle vehicle)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != vehicle.Id)
            {
                return BadRequest("Requested Vehicle Id does not match with QueryString Id");
            }

            try
            {
               var vehicleName = _context.Vehicle.Where(v => v.Id == vehicle.Id).Select(v => v.Name).Single();
 
                if (vehicleName == vehicle.Name)
                {
                    _context.Entry(vehicle).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                }
                else
                {
                    var vehicleNameExists = await _context.Vehicle.SingleOrDefaultAsync(v => v.Id != vehicle.Id && v.Name.Equals(vehicle.Name, StringComparison.OrdinalIgnoreCase));
                    if (vehicleNameExists != null)
                    {
                        return BadRequest("Vehicle name already exists");
                    }
                    else
                    {
                        _context.Entry(vehicle).State = EntityState.Modified;
                        await _context.SaveChangesAsync();
                    }
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VehicleExists(id))
                {
                    return NotFound("Vehicle Id not found");
                }
                else
                {
                    throw;
                }
            }

            return Ok(vehicle); 
        }


        // DELETE: api/Vehicles/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVehicle([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var vehicle = await _context.Vehicle.FindAsync(id);
            if (vehicle == null)
            {
                return NotFound("Vehicle Id not found");
            }

            _context.Vehicle.Remove(vehicle);
            await _context.SaveChangesAsync();

            return Ok(vehicle);
        }

        // Check Vehicle Exist or Not
        private bool VehicleExists(int id)
        {
            return _context.Vehicle.Any(e => e.Id == id);
        }
    }
}
