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
    public class MarketSpecialHandlingsController : BaseController.BaseController
    {
        private readonly ILogger<MarketSpecialHandlingsController> _logger;

        public MarketSpecialHandlingsController(ApplicationDbContext context, ILogger<MarketSpecialHandlingsController> logger, IEmailSender emailSender, ISieveProcessor sieveProcessor) : base(context, emailSender, sieveProcessor)
        {
            _logger = logger;
        }

        // GET: api/MarketSpecialHandlings
        [HttpGet]
        public async Task<IActionResult> GetMarketSpecialHandlingAsync([FromQuery] SieveModel sieveModel)
        {
            var query = _context.MarketSpecialHandling.AsQueryable();
            var data = await _sieveProcessor.GetPagingDataAsync(sieveModel, query);

            return Ok(data);
        }
        // GET: api/MarketSpecialHandlings/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetMarketSpecialHandling([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var marketSpecialHandling = await _context.MarketSpecialHandling.FindAsync(id);

            if (marketSpecialHandling == null)
            {
                return NotFound("MarketSpecialHandling Id not found");
            }

            return Ok(marketSpecialHandling);
        }

        // POST: api/MarketSpecialHandlings
        [HttpPost]
        public async Task<IActionResult> PostMarketSpecialHandlingAsync([FromBody] MarketSpecialHandling marketSpecialHandling)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            MarketSpecialHandling marketSpecialHandlingNameExists = _context.MarketSpecialHandling.FirstOrDefault(ms => ms.MarketName.Equals(marketSpecialHandling.MarketName, StringComparison.OrdinalIgnoreCase));
            if (marketSpecialHandlingNameExists != null)
            {
                return BadRequest("MarketSpecialHandling Name already exists");
            }

            _context.MarketSpecialHandling.Add(marketSpecialHandling);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMarketSpecialHandling", new { id = marketSpecialHandling.Id }, marketSpecialHandling);
        }

        // PUT: api/MarketSpecialHandlings/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMarketSpecialHandlingAsync([FromRoute] int id, [FromBody] MarketSpecialHandling marketSpecialHandling)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != marketSpecialHandling.Id)
            {
                return BadRequest("Requested MarketSpecialHandling Id does not match with QueryString Id");
            }

            try
            {
                var marketSpecialHandlingName = _context.MarketSpecialHandling.Where(ms => ms.Id == marketSpecialHandling.Id).Select(ms => ms.MarketName).Single();

                if (marketSpecialHandlingName == marketSpecialHandling.MarketName)
                {
                    _context.Entry(marketSpecialHandling).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                }
                else
                {
                    var marketSpecialHandlingNameExists = _context.MarketSpecialHandling.Count(ms => ms.MarketName.Equals(marketSpecialHandling.MarketName, StringComparison.OrdinalIgnoreCase) && ms.Id != marketSpecialHandling.Id);
                    if (marketSpecialHandlingNameExists > 0)
                    {
                        return BadRequest("MarketSpecialHandling Name already exists");
                    }
                    else
                    {
                        _context.Entry(marketSpecialHandling).State = EntityState.Modified;
                        await _context.SaveChangesAsync();
                    }
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MarketSpecialHandlingExists(id))
                {
                    return NotFound("MarketSpecialHandling Id not found");
                }
                else
                {
                    throw;
                }
            }

            return Ok(marketSpecialHandling);
        }

        // DELETE: api/MarketSpecialHandling/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMarketSpecialHandlingAsync([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var marketSpecialHandling = await _context.MarketSpecialHandling.FindAsync(id);
            if (marketSpecialHandling == null)
            {
                return NotFound("MarketSpecialHandling Id not found");
            }

            _context.MarketSpecialHandling.Remove(marketSpecialHandling);
            await _context.SaveChangesAsync();

            return Ok(marketSpecialHandling);
        }

        // Check MarketSpecialHandling Exist or Not
        private bool MarketSpecialHandlingExists(int id)
        {
            return _context.MarketSpecialHandling.Any(e => e.Id == id);
        }
    }
}
