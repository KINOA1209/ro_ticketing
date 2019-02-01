using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sieve.Models;
using Sieve.Services;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ticketing_api.Data;
using ticketing_api.Infrastructure;
using ticketing_api.Services;

namespace ticketing_api.Controllers
{
    /// <summary>
    /// Tickets
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public partial class TicketsController : BaseController.BaseController
    {
        private readonly ILogger<TicketsController> _logger;
        private readonly OrderService _orderService;
        private readonly LogService _ticketLogService;
        private readonly TicketService _ticketService;
        private readonly IHostingEnvironment _environment;

        public TicketsController(ApplicationDbContext context,
            ILogger<TicketsController> logger,
            IEmailSender emailSender,
            ISieveProcessor sieveProcessor,
            IHostingEnvironment environment)
            : base(context, emailSender, sieveProcessor)
        {
            _logger = logger;
            _orderService = new OrderService(_context, sieveProcessor);
            _ticketLogService = new LogService(_context, sieveProcessor);
            _ticketService = new TicketService(_context, sieveProcessor);
            _environment = environment;
        }

        /// <param name="sieveModel"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetTicketsAsync([FromQuery]SieveModel sieveModel)
        {
            //filter tickets by user
            var ticketView = await _orderService.GetOrderViewAsync(AppUser, sieveModel, 1);
            return Ok(ticketView);
        }

        /// <summary>
        /// Returns tickets and orders
        /// </summary>
        /// <param name="sieveModel"></param>
        /// <returns></returns>
        [HttpGet("History")]
        public async Task<IActionResult> GetTicketsHistoryAsync([FromQuery]SieveModel sieveModel)
        {
            //filter tickets by user
            var ticketView = await _orderService.GetOrderViewAsync(AppUser, sieveModel, 2);
            return Ok(ticketView);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTicketAsync([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var ticket = await _context.Order.FindAsync(id);
            var errorMsg = await _ticketService.CheckTicketAccessAsync(ticket, AppUser);
            if (errorMsg != null) return BadRequest(errorMsg);

            return Ok(ticket);
        }

        /// <summary>
        /// Change Ticket back to order
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        //[Authorize(Policy = "AdminOnly")]
        [HttpPost("{id}/order")]
        public async Task<IActionResult> PostTicketOrder([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var order = await _context.Order.FindAsync(id);

            if (order == null)
            {
                return NotFound("Ticket Id not found");
            }

            var orderStatusBefore = from o in _context.Order
                                    join status in _context.OrderStatus
                                    on o.OrderStatusId equals status.Id
                                    where o.Id == id
                                    select new { status.Name };

            string orderStatusBeforeName = orderStatusBefore.FirstOrDefault()?.Name;

            string orderStatusAfterName = await _orderService.CreateOrderFromTicketAsync(order);
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            await _ticketLogService.StoreLogInformationCreateOrderFromTicket(id, orderStatusBeforeName, orderStatusAfterName, this.ControllerContext.RouteData.Values["action"].ToString(), userId);
            return Ok();
        }

        /// <summary>
        /// Void ticket
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        //[Authorize(Policy = "AdminOnly")]
        [HttpPost("{id}/voidTicket")]
        public async Task<IActionResult> VoidTicket([FromRoute] int id)
        {
            var order = await _context.Order.FindAsync(id);

            if (order == null)
            {
                return NotFound("Ticket Id not found");
            }

            var orderStatusBefore = from o in _context.Order
                                    join status in _context.OrderStatus
                                    on o.OrderStatusId equals status.Id
                                    where o.Id == id
                                    select new { status.Name };

            string orderStatusBeforeName = orderStatusBefore.FirstOrDefault()?.Name;

            string orderStatusAfterName = await _orderService.CreateVoidTicketAsync(order);

            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            await _ticketLogService.StoreLogInformationCreateOrderFromTicket(id, orderStatusBeforeName, orderStatusAfterName, this.ControllerContext.RouteData.Values["action"].ToString(), userId);

            return Ok();
        }

        // DELETE: api/Tickets/5
        //[Authorize(Policy = "AdminOnly")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTicket([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var order = await _context.Order.FindAsync(id);
            if (order == null)
            {
                return NotFound("Ticket Id not found");
            }

            //check the Ticket Id exist in TicketProduct 
            //var ticketExistTicketProduct = _context.TicketProduct.Count(tp => tp.TicketId == id && !tp.IsDeleted);
            //if (ticketExistTicketProduct > 0)
            //{
            //    return BadRequest("Not able to delete Ticket as it has reference in TicketProduct table");
            //}

            ////check the Ticket Id exist in TicketTax 
            //var ticketExistTicketTax = _context.TicketTax.Count(t => t.TicketId == id && !t.IsDeleted);
            //if (ticketExistTicketTax > 0)
            //{
            //    return BadRequest("Not able to delete Ticket as it has reference in TicketTax table");
            //}

            _context.Order.Remove(order);
            await _context.SaveChangesAsync();

            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            await _ticketLogService.StoreLogInformationDelete(id, this.ControllerContext.RouteData.Values["action"].ToString(), userId);
            return Ok(order);
        }

        //// GET: api/Tickets/5/scan
        //[HttpGet("{id}/scan")]
        //public async Task<IActionResult> GetTicketScan([FromRoute] int id)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    var ticket = await _context.Order.FindAsync(id);

        //    if (ticket == null || ticket.OrderStatusId == AppConstants.OrderStatuses.Preticket)
        //    {
        //        return NotFound("Ticket Id not found");
        //    }

        //    var url = $"{Request.Scheme}://{Request.Host}";
        //    var imagePath = $"{url}/Images/TicketImage/";

        //    var ticketScanResult = (from order in _context.Order
        //                            where order.Id == id && order.OrderStatusId != AppConstants.OrderStatuses.Preticket
        //                            select new { order.Id, TicketImg = imagePath + order.TicketImg }).FirstOrDefault();

        //    return Ok(ticketScanResult);
        //}

    }
}