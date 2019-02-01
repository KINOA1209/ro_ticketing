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
using ticketing_api.Models.Views;
using ticketing_api.Services;

namespace ticketing_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountiesController : BaseController.BaseController
    {
        private readonly ILogger<CountiesController> _logger;

        public CountiesController(ApplicationDbContext context, ILogger<CountiesController> logger, IEmailSender emailSender, ISieveProcessor sieveProcessor) : base(context, emailSender, sieveProcessor)
        {
            _logger = logger;
        }

        // GET: api/Counties
        [HttpGet]
        public async Task<IActionResult> GetCountyAsync([FromQuery] SieveModel sieveModel)
        {
            var query = _context.County.AsQueryable();
            var data = await _sieveProcessor.GetPagingDataAsync(sieveModel, query);
            return Ok(data);
        }

        // GET: api/Counties
        [HttpGet("view")]
        public async Task<IActionResult> GetCountyViewAsync([FromQuery] SieveModel sieveModel)
        {
            var query = (from county in _context.County
                join market in _context.Market on county.MarketId equals market.Id into m
                from market in m.DefaultIfEmpty()
                select new CountyView()
                {
                    Id = county.Id,
                    MarketId = market,
                    Name = county.Name,
                    Description = county.Description,
                    IsVisible = county.IsVisible
                }).AsQueryable();
            var data = await _sieveProcessor.GetPagingDataAsync(sieveModel, query);
            return Ok(data);
        }

        // GET: api/Counties/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCounty([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var county = await _context.County.FindAsync(id);

            if (county == null)
            {
                return NotFound("County Id not found");
            }

            return Ok(county);
        }

        // POST: api/Counties
        [HttpPost]
        public async Task<IActionResult> PostCounty([FromBody] County county)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var countyNameExists = await _context.County.FirstOrDefaultAsync(m => m.Name.Equals(county.Name, StringComparison.OrdinalIgnoreCase));
            if (countyNameExists != null)
            {
                return BadRequest("County code already exists");
            }
            else
            {
                _context.County.Add(county);
                await _context.SaveChangesAsync();
            }

            return CreatedAtAction("GetCounty", new { id = county.Id }, county);
        }

        // PUT: api/Counties/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCounty([FromRoute] int id, [FromBody] County county)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != county.Id)
            {
                return BadRequest("Requested County Id does not match with QueryString Id");
            }

            try
            {
                var countyName = _context.County.Where(m => m.Id == county.Id).Select(m => new { m.Name });

                if (countyName.FirstOrDefault()?.Name == county.Name)
                {
                    _context.Entry(county).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                }
                else
                {
                    var countyNameExists = _context.County.Count(m => m.Id != county.Id && m.Name.Equals(county.Name, StringComparison.OrdinalIgnoreCase));
                    if (countyNameExists > 0)
                    {
                        return BadRequest("County Name already exists");
                    }

                    _context.Entry(county).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CountyExists(id))
                {
                    return NotFound("County Id not found");
                }
                else
                {
                    throw;
                }
            }

            return Ok(county);
        }

        // DELETE: api/Counties/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCounty([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var county = await _context.County.FindAsync(id);
            if (county == null)
            {
                return NotFound("County Id not found");
            }

            //check the County Id exist in Order 
            var countyExistOrder = _context.Order.Count(o => o.CountyId == id && !o.IsDeleted);

            if (countyExistOrder > 0)
            {
                return BadRequest("Not able to delete County as it has reference in Order table");
            }

            //// check the County Id exist in Tax
            //var countyExistTax = _context.Tax.Count(t => t.CountyId == id && !t.IsDeleted);

            //if (countyExistTax > 0)
            //{
            //    return BadRequest("Not able to delete County as it has reference in Tax table");
            //}

            //// check the County Id exist in ShippingPaper
            //var countyExistShippingPaper = _context.ShippingPaper.Count(sp => sp.CountyId == id && !sp.IsDeleted);

            //if(countyExistShippingPaper > 0)
            //{
            //    return BadRequest("Not able to delete County as it has reference in ShippingPaper table");
            //}

            //// check the County Id exist in TicketPaper
            //var countyExistTicketPaper = _context.TicketPaper.Count(tp => tp.CountyId == id && !tp.IsDeleted);

            //if (countyExistTicketPaper > 0)
            //{
            //    return BadRequest("Not able to delete County as it has reference in TicketPaper table");
            //}

            _context.County.Remove(county);
            await _context.SaveChangesAsync();

            return Ok(county);
        }

        // Check County Exist or Not
        private bool CountyExists(int id)
        {
            return _context.County.Any(e => e.Id == id);
        }
    }
}