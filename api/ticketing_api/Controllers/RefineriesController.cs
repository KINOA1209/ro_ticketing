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
    public class RefineriesController : BaseController.BaseController
    {
        private readonly ILogger<RefineriesController> _logger;

        public RefineriesController(ApplicationDbContext context, ILogger<RefineriesController> logger, IEmailSender emailSender, ISieveProcessor sieveProcessor)
            : base(context, emailSender, sieveProcessor)
        {
            _logger = logger;
        }

        // GET: api/Refinerys
        [HttpGet]
        public async Task<IActionResult> GetRefineryAsync([FromQuery] SieveModel sieveModel)
        {
            var query = _context.Refinery.AsQueryable();
            var data = await _sieveProcessor.GetPagingDataAsync(sieveModel, query);
            return Ok(data);
        }

        // GET: api/Refinerys/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRefinery([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var refinery = await _context.Refinery.FindAsync(id);

            if (refinery == null)
            {
                return NotFound("Refinery Id not found");
            }

            return Ok(refinery);
        }

        // POST: api/Refinerys
        [HttpPost]
        public async Task<IActionResult> PostRefinery([FromBody] Refinery refinery)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var refineryNameExists = await _context.Refinery.FirstOrDefaultAsync(t => t.Name.Equals(refinery.Name, StringComparison.OrdinalIgnoreCase));
            if (refineryNameExists != null)
            {
                return BadRequest("Refinery name already exists");
            }
            _context.Refinery.Add(refinery);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRefinery", new { id = refinery.Id }, refinery);
        }

        // PUT: api/Refinerys/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRefinery([FromRoute] int id, [FromBody] Refinery refinery)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != refinery.Id)
            {
                return BadRequest("Requested Refinery Id does not match with QueryString Id");
            }

            try
            {
                var refineryName = _context.Refinery.Where(t => t.Id == refinery.Id).Select(t => t.Name).Single();

                if (refineryName == refinery.Name)
                {
                    _context.Entry(refinery).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                }
                else
                {
                    var refineryNameExists = _context.Refinery.Count(t => t.Name.Equals(refinery.Name, StringComparison.OrdinalIgnoreCase) && t.Id != refinery.Id);
                    if (refineryNameExists > 0)
                    {
                        return BadRequest("Refinery name already exists");
                    }
                    else
                    {
                        _context.Entry(refinery).State = EntityState.Modified;
                        await _context.SaveChangesAsync();
                    }
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RefineryExists(id))
                {
                    return NotFound("Refinery Id not found");
                }
                else
                {
                    throw;
                }
            }

            return Ok(refinery);
        }

        // DELETE: api/Refinerys/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRefinery([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var refinery = await _context.Refinery.FindAsync(id);
            if (refinery == null)
            {
                return NotFound("Refinery Id not found");
            }

            var refineryExistOrder = _context.Refinery.Count(o => o.Id == id && !o.IsDeleted);

            if (refineryExistOrder > 0)
            {
                return BadRequest("Not able to delete Refinery as it has reference in Order table");
            }

            _context.Refinery.Remove(refinery);
            await _context.SaveChangesAsync();

            return Ok(refinery);
        }

        // Check Refinerys Exist or Not
        private bool RefineryExists(int id)
        {
            return _context.Refinery.Any(e => e.Id == id);
        }
    }
}