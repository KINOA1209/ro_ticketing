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
    public class UnitsController : BaseController.BaseController
    {
        private readonly ILogger<UnitsController> _logger;

        public UnitsController(ApplicationDbContext context, ILogger<UnitsController> logger, IEmailSender emailSender, ISieveProcessor sieveProcessor) : base(context, emailSender, sieveProcessor)
        {
            _logger = logger;
        }

        // GET: api/Units
        [HttpGet]
        public async Task<IActionResult> GetUnit([FromQuery]SieveModel sieveModel)
        {
            var query = _context.Unit.AsQueryable();
            var data = await _sieveProcessor.GetPagingDataAsync(sieveModel, query);
            return Ok(data);
        }

        // GET: api/Units/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUnit([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var units = await _context.Unit.FindAsync(id);

            if (units == null)
            {
                return NotFound("Unit Id not found");
            }

            return Ok(units);
        }

        // POST: api/Units
        [HttpPost]
        public async Task<IActionResult> PostUnit([FromBody] Unit unit)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var unitNameExists = await _context.Unit.FirstOrDefaultAsync(u => u.Name.Equals(unit.Name, StringComparison.OrdinalIgnoreCase));
            if (unitNameExists != null)
            {
                return BadRequest("Unit name already exists");
            }
            else
            {
                _context.Unit.Add(unit);
                await _context.SaveChangesAsync();
            }
            
            return CreatedAtAction("GetUnit", new { id = unit.Id }, unit);
        }

        // PUT: api/Units/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUnit([FromRoute] int id, [FromBody] Unit unit)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != unit.Id)
            {
                return BadRequest("Requested Unit Id does not match with QueryString Id");
            }
           
            try
            {
                var unitName = _context.Unit.Where(u => u.Id == unit.Id).Select(u => u.Name).Single();

                if (unitName == unit.Name)
                {
                    _context.Entry(unit).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                }
                else
                {
                    var unitNameExists = _context.Unit.Count(u => u.Id != unit.Id && u.Name.Equals(unit.Name, StringComparison.OrdinalIgnoreCase));
                    if (unitNameExists > 0)
                    {
                        return BadRequest("Unit name already exists");
                    }
                    else
                    {
                        _context.Entry(unit).State = EntityState.Modified;
                        await _context.SaveChangesAsync();
                    }
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UnitExists(id))
                {
                    return NotFound("Unit Id not found");
                }
                else
                {
                    throw;
                }
            }

            return Ok(unit);
        }

        // DELETE: api/Units/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUnit([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var unit = await _context.Unit.FindAsync(id);
            if (unit == null)
            {
                return NotFound("Unit Id not found");
            }

            //check the Unit Id exist in TicketProduct 
            var unitExistTicketProduct = _context.TicketProduct.Count(tp => tp.UnitId == id && !tp.IsDeleted);

            if (unitExistTicketProduct > 0)
            {
                return BadRequest("Not able to delete Unit as it has reference in TicketProduct table");
            }

            //check the Unit Id exist in Product 
            var unitExistProduct = _context.Product.Count(tp => tp.UnitId == id && !tp.IsDeleted);

            if(unitExistProduct > 0)
            {
                return BadRequest("Not able to delete Unit as it has reference in Product table");
            }

            _context.Unit.Remove(unit);
            await _context.SaveChangesAsync();

            return Ok(unit);
        }

        // Check unit Exist or Not
        private bool UnitExists(int id)
        {
            return _context.Unit.Any(e => e.Id == id);
        }
    }
}
