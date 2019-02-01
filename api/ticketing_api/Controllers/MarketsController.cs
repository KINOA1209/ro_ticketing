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
    public class MarketsController : BaseController.BaseController
    {
        private readonly ILogger<MarketsController> _logger;

        public MarketsController(ApplicationDbContext context, ILogger<MarketsController> logger, IEmailSender emailSender, ISieveProcessor sieveProcessor) : base(context, emailSender, sieveProcessor)
        {
            _logger = logger;
        }

        // GET: api/Markets
        [HttpGet]
        public async Task<IActionResult> GetMarketAsync([FromQuery] SieveModel sieveModel)
        {
            var query = _context.Market.AsQueryable();
            var data = await _sieveProcessor.GetPagingDataAsync(sieveModel, query);
            return Ok(data);
        }

        // GET: api/Markets/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetMarket([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var market = await _context.Market.FindAsync(id);

            if (market == null)
            {
                return NotFound("Market Id not found");
            }

            return Ok(market);
        }

        // POST: api/Markets
        [HttpPost]
        public async Task<IActionResult> PostMarket([FromBody] Market market)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var marketNameExists = await _context.Market.FirstOrDefaultAsync(m => m.Name.Equals(market.Name, StringComparison.OrdinalIgnoreCase));
            if (marketNameExists != null)
            {
                return BadRequest("Market code already exists");
            }
            else
            {
                _context.Market.Add(market);
                await _context.SaveChangesAsync();
            }

            return CreatedAtAction("GetMarket", new { id = market.Id }, market);
        }

        // PUT: api/Markets/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMarket([FromRoute] int id, [FromBody] Market market)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != market.Id)
            {
                return BadRequest("Requested Market Id does not match with QueryString Id");
            }

            try
            {
                var marketName = _context.Market.Where(m => m.Id == market.Id).Select(m => new { m.Name });

                if (marketName.FirstOrDefault()?.Name == market.Name)
                {
                    _context.Entry(market).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                }
                else
                {
                    var marketNameExists = _context.Market.Count(m => m.Id != market.Id && m.Name.Equals(market.Name, StringComparison.OrdinalIgnoreCase));
                    if (marketNameExists > 0)
                    {
                        return BadRequest("Market Name already exists");
                    }

                    _context.Entry(market).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MarketExists(id))
                {
                    return NotFound("Market Id not found");
                }
                else
                {
                    throw;
                }
            }

            return Ok(market);
        }

        // DELETE: api/Markets/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMarket([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var market = await _context.Market.FindAsync(id);
            if (market == null)
            {
                return NotFound("Market Id not found");
            }

            //check the Market Id exist in Order 
            var marketExistOrder = _context.Order.Count(o => o.MarketId == id && !o.IsDeleted);

            if (marketExistOrder > 0)
            {
                return BadRequest("Not able to delete Market as it has reference in Order table");
            }

            // check the Market Id exist in Tax
            var marketExistTax = _context.Tax.Count(t => t.MarketId == id && !t.IsDeleted);

            if (marketExistTax > 0)
            {
                return BadRequest("Not able to delete Market as it has reference in Tax table");
            }

            // check the Market Id exist in ShippingPaper
            var marketExistShippingPaper = _context.ShippingPaper.Count(sp => sp.MarketId == id && !sp.IsDeleted);

            if(marketExistShippingPaper > 0)
            {
                return BadRequest("Not able to delete Market as it has reference in ShippingPaper table");
            }

            // check the Market Id exist in TicketPaper
            var marketExistTicketPaper = _context.TicketPaper.Count(tp => tp.MarketId == id && !tp.IsDeleted);

            if (marketExistTicketPaper > 0)
            {
                return BadRequest("Not able to delete Market as it has reference in TicketPaper table");
            }

            _context.Market.Remove(market);
            await _context.SaveChangesAsync();

            return Ok(market);
        }

        // Check Market Exist or Not
        private bool MarketExists(int id)
        {
            return _context.Market.Any(e => e.Id == id);
        }
    }
}