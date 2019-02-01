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
    public class OrderDirectionsController : BaseController.BaseController
    {
        private readonly ILogger<OrderDirectionsController> _logger;

        public OrderDirectionsController(ApplicationDbContext context, ILogger<OrderDirectionsController> logger, IEmailSender emailSender, ISieveProcessor sieveProcessor) : base(context, emailSender, sieveProcessor)
        {
            _logger = logger;
        }

        // GET: api/OrderDirections
        [HttpGet]
        public async Task<IActionResult> GetOrderDirectionAsync([FromQuery]SieveModel sieveModel)
        {
            var query = _context.OrderDirection.AsQueryable();
            var data = await _sieveProcessor.GetPagingDataAsync(sieveModel, query);

            return Ok(data);
        }

        // GET: api/OrderDirections/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderDirection([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var orderDirection = await _context.OrderDirection.FindAsync(id);

            if (orderDirection == null)
            {
                return NotFound("Order Direction Id not found");
            }

            return Ok(orderDirection);
        }

        // GET: api/OrderDirections
        [HttpGet("latestdirection")]
        public async Task<IActionResult> GetLatestDirection()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var latestOrderDirection = await _context.OrderDirection.OrderByDescending(c => c.CreatedAt).FirstAsync();

            return Ok(latestOrderDirection);
        }

        // POST: api/OrderDirections
        [HttpPost]
        public async Task<IActionResult> PostOrderDirection([FromBody] OrderDirection orderDirection)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.OrderDirection.Add(orderDirection);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetOrderDirection", new { id = orderDirection.Id }, orderDirection);
        }

        // PUT: api/OrderDirections/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrderDirection([FromRoute] int id, [FromBody] OrderDirection orderDirection)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != orderDirection.Id)
            {
                return BadRequest("Requested OrderDirection Id does not match with QueryString Id");
            }

            try
            {
                _context.Entry(orderDirection).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderDirectionNoteExists(id))
                {
                    return NotFound("Order Direction Id not found");
                }
                else
                {
                    throw;
                }
            }
            return Ok(orderDirection);
        }

        // DELETE: api/OrderDirections/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrderDirection([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var orderDirection = await _context.OrderDirection.FindAsync(id);
            if (orderDirection == null)
            {
                return NotFound("Order Direction Id not found");
            }

            _context.OrderDirection.Remove(orderDirection);
            await _context.SaveChangesAsync();

            return Ok(orderDirection);
        }

        private bool OrderDirectionNoteExists(int id)
        {
            return _context.RigLocationNote.Any(e => e.Id == id);
        }
    }
}
