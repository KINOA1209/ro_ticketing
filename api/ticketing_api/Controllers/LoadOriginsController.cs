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
    public class LoadOriginsController : BaseController.BaseController
    {
        private readonly ILogger<LoadOriginsController> _logger;

        public LoadOriginsController(ApplicationDbContext context, ILogger<LoadOriginsController> logger, IEmailSender emailSender, ISieveProcessor sieveProcessor) : base(context, emailSender, sieveProcessor)
        {
            _logger = logger;
        }

        // GET: api/LoadOrigins
        [HttpGet]
        public async Task<IActionResult> GetLoadOriginAsync([FromQuery] SieveModel sieveModel)
        {
            var query = _context.LoadOrigin.AsQueryable();
            var data = await _sieveProcessor.GetPagingDataAsync(sieveModel, query);

            return Ok(data);
        }

        // GET: api/LoadOrigins/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetLoadOrigin([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var loadOrigin = await _context.LoadOrigin.FindAsync(id);

            if (loadOrigin == null)
            {
                return NotFound("loadOrigin Id not found");
            }

            return Ok(loadOrigin);
        }

        // POST: api/LoadOrigins
        [HttpPost]
        public async Task<IActionResult> PostLoadOriginAsync([FromBody] LoadOrigin loadOrigin)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            LoadOrigin loadOriginNameExists = _context.LoadOrigin.FirstOrDefault(l => l.Name.Equals(loadOrigin.Name, StringComparison.OrdinalIgnoreCase));
            if (loadOriginNameExists != null)
            {
                return BadRequest("LoadOrigin Name already exists");
            }

            _context.LoadOrigin.Add(loadOrigin);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetLoadOrigin", new { id = loadOrigin.Id }, loadOrigin);
        }


        // PUT: api/LoadOrigins/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLoadOriginAsync([FromRoute] int id, [FromBody] LoadOrigin loadOrigin)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != loadOrigin.Id)
            {
                return BadRequest("Requested LoadOrigin Id does not match with QueryString Id");
            }

            try
            {
                var loadOriginName = _context.LoadOrigin.Where(l => l.Id == loadOrigin.Id).Select(l => l.Name).Single();

                if (loadOriginName == loadOrigin.Name)
                {
                    _context.Entry(loadOrigin).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                }
                else
                {
                    var loadOriginNameExists = _context.LoadOrigin.Count(l => l.Name.Equals(loadOrigin.Name, StringComparison.OrdinalIgnoreCase) && l.Id != loadOrigin.Id);
                    if (loadOriginNameExists > 0)
                    {
                        return BadRequest("LoadOrigin name already exists");
                    }
                    else
                    {
                        _context.Entry(loadOrigin).State = EntityState.Modified;
                        await _context.SaveChangesAsync();
                    }
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LoadOriginExists(id))
                {
                    return NotFound("LoadOrigin Id not found");
                }
                else
                {
                    throw;
                }
            }

            return Ok(loadOrigin);
        }

        // DELETE: api/LoadOrigins/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLoadOriginAsync([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var loadOrigin = await _context.LoadOrigin.FindAsync(id);
            if (loadOrigin == null)
            {
                return NotFound("LoadOrigin Id not found");
            }

            // check the LoadOrigin Id exist in Order
            var loadOriginExistOrder = _context.Order.Count(o => o.LoadOriginId == id && !o.IsDeleted);

            if (loadOriginExistOrder > 0)
            {
                return BadRequest("Not able to delete LoadOrigin as it has reference in Order table");
            }

            _context.LoadOrigin.Remove(loadOrigin);
            await _context.SaveChangesAsync();

            return Ok(loadOrigin);
        }

        // Check LoadOrigin Exist or Not
        private bool LoadOriginExists(int id)
        {
            return _context.LoadOrigin.Any(e => e.Id == id);
        }


    }
}
