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
    public class RefineryAddressesController : BaseController.BaseController
    {
        private readonly ILogger<RefineryAddressesController> _logger;

        public RefineryAddressesController(ApplicationDbContext context, ILogger<RefineryAddressesController> logger, IEmailSender emailSender, ISieveProcessor sieveProcessor)
            : base(context, emailSender, sieveProcessor)
        {
            _logger = logger;
        }

        // GET: api/RefineryAddresss
        [HttpGet]
        public async Task<IActionResult> GetRefineryAddressesAsync([FromQuery] SieveModel sieveModel)
        {
            var query = _context.RefineryAddress.AsQueryable();
            var data = await _sieveProcessor.GetPagingDataAsync(sieveModel, query);
            return Ok(data);
        }

        // GET: api/RefineryAddresss/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRefineryAddress([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var refineryAddress = await _context.RefineryAddress.FindAsync(id);

            if (refineryAddress == null)
            {
                return NotFound("RefineryAddress Id not found");
            }

            return Ok(refineryAddress);
        }

        // POST: api/RefineryAddresss
        [HttpPost]
        public async Task<IActionResult> PostRefineryAddress([FromBody] RefineryAddress refineryAddress)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var refineryAddressNameExists = await _context.RefineryAddress.FirstOrDefaultAsync(t => t.Name.Equals(refineryAddress.Name, StringComparison.OrdinalIgnoreCase));
            if (refineryAddressNameExists != null)
            {
                return BadRequest("RefineryAddress already exists");
            }
            _context.RefineryAddress.Add(refineryAddress);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRefineryAddress", new { id = refineryAddress.Id }, refineryAddress);
        }

        // PUT: api/RefineryAddresss/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRefineryAddress([FromRoute] int id, [FromBody] RefineryAddress refineryAddress)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != refineryAddress.Id)
            {
                return BadRequest("Requested RefineryAddress Id does not match with QueryString Id");
            }

            try
            {
                var refineryAddressName = _context.RefineryAddress.Where(t => t.Id == refineryAddress.Id).Select(t => t.Name).Single();

                if (refineryAddressName == refineryAddress.Name)
                {
                    _context.Entry(refineryAddress).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                }
                else
                {
                    var refineryAddressNameExists = _context.RefineryAddress.Count(t => t.Name.Equals(refineryAddress.Name, StringComparison.OrdinalIgnoreCase) && t.Id != refineryAddress.Id);
                    if (refineryAddressNameExists > 0)
                    {
                        return BadRequest("RefineryAddress name already exists");
                    }
                    else
                    {
                        _context.Entry(refineryAddress).State = EntityState.Modified;
                        await _context.SaveChangesAsync();
                    }
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RefineryAddressExists(id))
                {
                    return NotFound("RefineryAddress Id not found");
                }
                else
                {
                    throw;
                }
            }

            return Ok(refineryAddress);
        }

        // DELETE: api/RefineryAddresss/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRefineryAddress([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var refineryAddress = await _context.RefineryAddress.FindAsync(id);
            if (refineryAddress == null)
            {
                return NotFound("RefineryAddress Id not found");
            }

            var refineryAddressExistOrder = _context.RefineryAddress.Count(o => o.Id == id && !o.IsDeleted);

            if (refineryAddressExistOrder > 0)
            {
                return BadRequest("Not able to delete RefineryAddress as it has reference in Order table");
            }

            _context.RefineryAddress.Remove(refineryAddress);
            await _context.SaveChangesAsync();

            return Ok(refineryAddress);
        }

        // Check RefineryAddresss Exist or Not
        private bool RefineryAddressExists(int id)
        {
            return _context.RefineryAddress.Any(e => e.Id == id);
        }
    }
}