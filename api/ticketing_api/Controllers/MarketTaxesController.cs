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
using ticketing_api.Models.Views;
using ticketing_api.Services;

namespace ticketing_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MarketTaxesController : BaseController.BaseController
    {
        private readonly ILogger<MarketTaxesController> _logger;
        private readonly MarketTaxService _marketTaxService;

        public MarketTaxesController(ApplicationDbContext context, ILogger<MarketTaxesController> logger, IEmailSender emailSender, ISieveProcessor sieveProcessor) : base(context, emailSender, sieveProcessor)
        {
            _logger = logger;
            _marketTaxService = new MarketTaxService(_context, sieveProcessor);
        }

        // GET: api/MarketTaxes
        [HttpGet]
        public async Task<IActionResult> GetMarketTax([FromQuery]SieveModel sieveModel)
        {
            PagingResults<MarketTaxView> marketTax = await _marketTaxService.GetMarketTaxViewAsync(sieveModel);
            return Ok(marketTax);
        }

        // GET: api/MarketTaxes/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetMarketTax([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var marketTax = await _context.MarketTax.FindAsync(id);

            if (marketTax == null)
            {
                return NotFound("Market Tax Id not found");
            }

            return Ok(marketTax);
        }

        // POST: api/MarketTaxes
        [HttpPost]
        public async Task<IActionResult> PostMarketTax([FromBody] MarketTax marketTax)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.MarketTax.Add(marketTax);
            await _context.SaveChangesAsync();

            MarketTaxView marketTaxView = _marketTaxService.PostMarketTax(marketTax);

            return CreatedAtAction("GetMarketTax", new { id = marketTax.Id }, marketTaxView);
        }

        // PUT: api/MarketTaxes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMarketTax([FromRoute] int id, [FromBody] MarketTax marketTax)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != marketTax.Id)
            {
                return BadRequest("Requested MarketTax Id does not match with QueryString Id");
            }

            _context.Entry(marketTax).State = EntityState.Modified;
             MarketTaxView marketTaxView;
            try
            {
                await _context.SaveChangesAsync();
                marketTaxView = _marketTaxService.PostMarketTax(marketTax);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MarketTaxExists(id))
                {
                    return NotFound("Market Tax Id not found");
                }
                else
                {
                    throw;
                }
            }

            return Ok(marketTaxView);
        }



        // DELETE: api/MarketTaxes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMarketTax([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var marketTax = await _context.MarketTax.FindAsync(id);
            if (marketTax == null)
            {
                return NotFound("Market Tax Id not found");
            }

            _context.MarketTax.Remove(marketTax);
            await _context.SaveChangesAsync();

            return Ok(marketTax);
        }


        private bool MarketTaxExists(int id)
        {
            return _context.MarketTax.Any(e => e.Id == id);
        }

        // Check Market Tax Exist or Not
        [HttpGet("Filter/{marketId}")]
        public IActionResult GetMarketTaxByMarketId([FromRoute] int marketId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IEnumerable<MarketTaxView> marketTax = _marketTaxService.GetMarketTaxByMarketId(marketId); 
                       
            return Ok(marketTax);
        }
    }
}