using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sieve.Models;
using Sieve.Services;
using ticketing_api.Data;
using ticketing_api.Infrastructure;
using ticketing_api.Models;
using Microsoft.EntityFrameworkCore;
using ticketing_api.Services;

namespace ticketing_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderLogsController : BaseController.BaseController
    {

        private readonly ILogger<OrderLogsController> _logger;
        private readonly LogService _logService;

        public OrderLogsController(ApplicationDbContext context, ILogger<OrderLogsController> logger, IEmailSender emailSender, ISieveProcessor sieveProcessor) : base(context, emailSender, sieveProcessor)
        {
            _logger = logger;
            _logService = new LogService(_context, sieveProcessor);
        }

        // GET: api/ViewOrderLog
        [HttpGet]
        public async Task<IActionResult> GetOrderlog([FromQuery]SieveModel sieveModel)
        {
            PagingResults<Log> orderLog = await _logService.GetOrderLogViewAsync(sieveModel);
            return Ok(orderLog);
        }

        // GET: api/ViewTicketLog
        [HttpGet("ViewTicketlog")]
        public async Task<IActionResult> GetTicketlog([FromQuery]SieveModel sieveModel)
        {
            PagingResults<Log> ticketLog = await _logService.GetTicketLogViewAsync(sieveModel);
            return Ok(ticketLog);
        }

        // GET: api/ViewCustomerLog
        [HttpGet("ViewCustomerlog")]
        public async Task<IActionResult> GetCustomerlog([FromQuery]SieveModel sieveModel)
        {
            PagingResults<Log> customerLog = await _logService.GetCustomerLogViewAsync(sieveModel);
            return Ok(customerLog);
        }

        // GET: api/ViewRigLocationLog
        [HttpGet("ViewRigLocationlog")]
        public async Task<IActionResult> GetRigLocationlog([FromQuery]SieveModel sieveModel)
        {
            PagingResults<Log> rigLocationLog = await _logService.GetRigLocationLogViewAsync(sieveModel);
            return Ok(rigLocationLog);
        }

        // DELETE: api/DeleteOrderLog
        [HttpDelete]
        public async Task<IActionResult> DeleteOrderlog()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var orderLog = _context.OrderLog.Where(c => c.IsTable == "Order").ToList();

            _context.OrderLog.RemoveRange(orderLog);
            await _context.SaveChangesAsync();

            return Ok();
        }

        // DELETE: api/DeleteTicketLog
        [HttpDelete("DeleteTicketlog")]
        public async Task<IActionResult> DeleteTicketlog()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var ticketLog = _context.OrderLog.Where(c => c.IsTable == "Ticket").ToList();

            _context.OrderLog.RemoveRange(ticketLog);
            await _context.SaveChangesAsync();

            return Ok();
        }

        // DELETE: api/DeleteCustomerLog
        [HttpDelete("DeleteCustomerlog")]
        public async Task<IActionResult> DeleteCustomerlog()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var customerLog = _context.OrderLog.Where(c => c.IsTable == "Customer").ToList();

            _context.OrderLog.RemoveRange(customerLog);
            await _context.SaveChangesAsync();

            return Ok();
        }

        // DELETE: api/DeleteRigLocationLog
        [HttpDelete("DeleteRigLocationlog")]
        public async Task<IActionResult> DeleteRigLocationlog()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var rigLocationLog = _context.OrderLog.Where(c => c.IsTable == "RigLocation").ToList();

            _context.OrderLog.RemoveRange(rigLocationLog);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
