using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sieve.Models;
using Sieve.Services;
using ticketing_api.Data;
using ticketing_api.Infrastructure;
using ticketing_api.Models;
using Microsoft.EntityFrameworkCore;

namespace ticketing_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderStatusController : BaseController.BaseController
    {
        private readonly ILogger<OrderStatusController> _logger;

        public OrderStatusController(ApplicationDbContext context, ILogger<OrderStatusController> logger, IEmailSender emailSender, ISieveProcessor sieveProcessor)
          : base(context, emailSender, sieveProcessor)
        {
            _logger = logger;

        }

        // GET: api/OrderStatus
        [HttpGet]
        public async Task<IActionResult> GetOrderStatusAsync([FromQuery] SieveModel sieveModel)
        {
            var query = _context.OrderStatus.AsQueryable();
            var data = await _sieveProcessor.GetPagingDataAsync(sieveModel, query);
            return Ok(data);
        }

        // GET: api/OrderStatus/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderStatus([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var orderStatus = await _context.OrderStatus.FindAsync(id);

            if (orderStatus == null)
            {
                return NotFound("Order Status Id not found");
            }

            return Ok(orderStatus);
        }

        // POST: api/OrderStatus
        [HttpPost]
        public async Task<IActionResult> PostOrderStatus([FromBody] OrderStatus orderStatus)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var orderStatusNameExists = await _context.OrderStatus.SingleOrDefaultAsync(os => os.Name == orderStatus.Name && os.Name.Equals(orderStatus.Name, StringComparison.OrdinalIgnoreCase));
            if (orderStatusNameExists != null)
            {
                return BadRequest("OrderStatus name already exists");
            }
            else
            {
                _context.OrderStatus.Add(orderStatus);
                await _context.SaveChangesAsync();
            }

            return CreatedAtAction("GetOrderStatus", new { id = orderStatus.Id }, orderStatus);
        }

        // PUT: api/OrderStatus/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrderStatus([FromRoute] int id, [FromBody] OrderStatus orderStatus)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != orderStatus.Id)
            {
                return BadRequest("Requested OrderStatus Id does not match with QueryString Id");
            }

            try
            {
                var orderStatusName = _context.OrderStatus.Where(os => os.Id == orderStatus.Id).Select(os => new { os.Name });

                if (orderStatusName.FirstOrDefault()?.Name == orderStatus.Name)
                {
                    _context.Entry(orderStatus).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                }
                else
                {
                    var orderStatusNameExists = await _context.OrderStatus.SingleOrDefaultAsync(os => os.Id != orderStatus.Id && os.Name.Equals(orderStatus.Name, StringComparison.OrdinalIgnoreCase));
                    if (orderStatusNameExists != null)
                    {
                        return BadRequest("OrderStatus name already exists");
                    }
                    else
                    {
                        _context.Entry(orderStatus).State = EntityState.Modified;
                        await _context.SaveChangesAsync();
                    }
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderStatusExists(id))
                {
                    return NotFound("Order Status Id not found");
                }
                else
                {
                    throw;
                }
            }

            return Ok(orderStatus);
        }

        // DELETE: api/OrderStatus/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrderStatus([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var orderStatus = await _context.OrderStatus.FindAsync(id);
            if (orderStatus == null)
            {
                return NotFound("Order Status Id not found");
            }

            // check the OrderStatus Id exist in Order
            var orderStatusExistOrder = _context.Order.Count(o => o.OrderStatusId == id && !o.IsDeleted);

            if (orderStatusExistOrder > 0)
            {
                return BadRequest("Not able to delete OrderStatus as it has reference in Order table");
            }

            _context.OrderStatus.Remove(orderStatus);
            await _context.SaveChangesAsync();

            return Ok(orderStatus);
        }

        // Check orderstatus Exist or Not
        private bool OrderStatusExists(int id)
        {
            return _context.OrderStatus.Any(e => e.Id == id);
        }
    }
}
