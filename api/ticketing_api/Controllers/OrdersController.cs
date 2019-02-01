using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Sieve.Models;
using Sieve.Services;
using ticketing_api.Data;
using ticketing_api.Infrastructure;
using ticketing_api.Models;
using ticketing_api.Models.Views;
using ticketing_api.Services;

namespace ticketing_api.Controllers
{
    ////[Authorize(Policy = "AdminOnly")]
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : BaseController.BaseController
    {
        private readonly ILogger<OrdersController> _logger;
        private readonly OrderService _orderService;
        private readonly LogService _orderLogService;

        public OrdersController(ApplicationDbContext context, ILogger<OrdersController> logger, IEmailSender emailSender, ISieveProcessor sieveProcessor)
            : base(context, emailSender, sieveProcessor)
        {
            _logger = logger;
            _orderService = new OrderService(_context, sieveProcessor);
            _orderLogService = new LogService(_context, sieveProcessor);
        }

        // GET: api/Orders
        [HttpGet]
        public async Task<IEnumerable<Order>> GetOrderAsync([FromQuery]SieveModel sieveModel)
        {
            var query = _context.Order.Where(x => x.OrderStatusId == AppConstants.OrderStatuses.Preticket).AsQueryable();
            query = _sieveProcessor.Apply(sieveModel, query);
            return query.ToList();
        }

        [HttpGet("View")]
        public async Task<IActionResult> ViewOrderAsync([FromQuery]SieveModel sieveModel)
        {
            PagingResults<OrderView> orderView = await _orderService.GetOrderViewAsync(AppUser,sieveModel, 0);
            return Ok(orderView);
        }

        // GET: api/Orders/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrder([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IQueryable<Order> query;
            var role = AppUser.Role.ToUpper();
            switch (role)
            {
                case "DRIVER":
                    var driver = await _context.Driver.FirstAsync(x => x.AppUserId == AppUser.Id);
                    query = _context.Order.Where(x => x.Id == id && x.OrderStatusId == AppConstants.OrderStatuses.Preticket && x.DriverId == driver.Id).AsQueryable();
                    break;
                case "SALES":
                    var sales = await _context.SalesRep.FirstAsync(x => x.AppUserId == AppUser.Id);
                    query = _context.Order.Where(x => x.Id == id && x.OrderStatusId == AppConstants.OrderStatuses.Preticket && x.SalesRepId == sales.Id).AsQueryable();
                    break;
                default:
                    query = _context.Order.Where(x => x.Id == id && x.OrderStatusId == AppConstants.OrderStatuses.Preticket).AsQueryable();
                    break;
            }

            var order = await query.FirstOrDefaultAsync();

            if (order == null)
            {
                return NotFound("Order id not found");
            }

            return Ok(order);
        }

        // POST: api/Orders
        [HttpPost]
        public async Task<IActionResult> PostOrderAsync([FromBody] Order order)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var role = AppUser.Role.ToUpper();
            switch (role)
            {
                case "DRIVER":
                    return BadRequest("Driver does not have permission to create orders");
                case "SALES":
                    var sales = await _context.SalesRep.FirstAsync(x => x.AppUserId == AppUser.Id);
                    if (order.SalesRepId == 0)
                        order.SalesRepId = sales.Id;
                    else if(order.SalesRepId != sales.Id)
                        return BadRequest("SalesRepId does not match the SalesRepId for this user");
                    break;
            }

            if (!string.IsNullOrEmpty(order.RequestTime))
            {
                order.RequestTime = order.RequestTime.PadLeft(5, '0');
            }

            //order.AFE_PO = (new SettingService(_context)).GetNewAfePo();
            order.OrderStatusId = AppConstants.OrderStatuses.Preticket;
            _context.Order.Add(order);
            await _context.SaveChangesAsync();
            OrderView orderView = _orderService.PostOrderView(order);

            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            await _orderLogService.StoreLogInformationPost(orderView, order, this.ControllerContext.RouteData.Values["action"].ToString(), userId);

            //send email notification
            _emailSender.SendOrderNotification(AppUser, orderView);

            return CreatedAtAction("GetOrder", new { id = order.Id }, orderView);
        }

        // PUT: api/Orders/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrderAsync([FromRoute] int id, [FromBody] Order order)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != order.Id)
            {
                return BadRequest("Requested Order Id does not match with QueryString Id");
            }

            //if (order.OrderStatusId != AppConstants.OrderStatuses.Preticket)
            //{
            //    return BadRequest("This order is not in preticket status");
            //}


            if (!string.IsNullOrEmpty(order.RequestTime))
            {
                order.RequestTime = order.RequestTime.PadLeft(5, '0');
            }

            Order orderUpdateBefore = _context.Order.AsNoTracking().FirstOrDefault(o => o.Id == id);
            OrderView orderUpdateBeforeView = _orderService.PostOrderView(orderUpdateBefore);
            OrderView orderUpdateAfter;
            try
            {
                _context.Entry(order).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                orderUpdateAfter = _orderService.PostOrderView(order);

                var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                await _orderLogService.StoreLogInformationPut(orderUpdateAfter, order, this.ControllerContext.RouteData.Values["action"].ToString(), userId, orderUpdateBefore, orderUpdateBeforeView);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(id))
                {
                    return NotFound("Order id not found");
                }

                throw;
            }

            return Ok(orderUpdateAfter);
        }


        // DELETE: api/Orders/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrderAsync([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var order = await _context.Order.FindAsync(id);
            if (order == null || order.OrderStatusId != AppConstants.OrderStatuses.Preticket)
            {
                return NotFound("Order id not found");
            }

            _context.Order.Remove(order);
            await _context.SaveChangesAsync();

            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            await _orderLogService.StoreLogInformationDelete(id, this.ControllerContext.RouteData.Values["action"].ToString(), userId);

            return Ok(order);
        }

        // Check order Exist or Not
        private bool OrderExists(int id)
        {
            return _context.Order.Any(e => e.Id == id);
        }

        // POST: api/Orders
        /// <summary>
        /// Create Ticket from Order. This will update Order status.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("{id}/ticket")]
        public async Task<IActionResult> PostOrderTicketAsync([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var order = await _context.Order.FindAsync(id);

            if (order == null)
            {
                return NotFound("Order id not found");
            }

            var orderStatusBefore = from o in _context.Order
                                    join status in _context.OrderStatus
                                    on o.OrderStatusId equals status.Id
                                    where o.Id == id
                                    select new { status.Name };

            string orderStatusBeforeName = orderStatusBefore.FirstOrDefault()?.Name;

            string orderStatusAfterName = await _orderService.CreateTicketFromOrderAsync(order);

            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            await _orderLogService.StoreLogInformationCreateOrderFromTicket(id, orderStatusBeforeName, orderStatusAfterName, this.ControllerContext.RouteData.Values["action"].ToString(), userId);

            //send email notification
            //await _emailSender.SendNotificationAsync($"Order id {id} converted to a ticket.",
            //    $"Order id {id} converted to a ticket.");

            return Ok();
        }
    }
}