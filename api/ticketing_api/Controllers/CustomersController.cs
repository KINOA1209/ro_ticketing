using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sieve.Models;
using Sieve.Services;
using ticketing_api.Data;
using ticketing_api.Infrastructure;
using ticketing_api.Models;
using ticketing_api.Services;

namespace ticketing_api.Controllers
{ 
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : BaseController.BaseController
    {
        private readonly ILogger<CustomersController> _logger;
        private readonly LogService _customerLogService;

        public CustomersController(ApplicationDbContext context, ILogger<CustomersController> logger, IEmailSender emailSender, ISieveProcessor sieveProcessor) : base(context, emailSender, sieveProcessor)
        {
            _logger = logger;
            _customerLogService = new LogService(_context, sieveProcessor);
        }

        // GET: api/Customers
        [HttpGet]
        public async Task<IActionResult> GetCustomerAsync([FromQuery] SieveModel sieveModel)
        {
            var query = _context.Customer.AsQueryable();
            var data = await _sieveProcessor.GetPagingDataAsync(sieveModel, query);

            return Ok(data);
        }

        // GET: api/Customers/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCustomer([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var customer = await _context.Customer.FindAsync(id);

            if (customer == null)
            {
                return NotFound("Customer Id not found");
            }

            return Ok(customer);
        }

        // POST: api/Customers
        [HttpPost]
        public async Task<IActionResult> PostCustomerAsync([FromBody] Customer customer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Customer customerNameExists = await _context.Customer.FirstOrDefaultAsync(c => c.Name.Equals(customer.Name, StringComparison.OrdinalIgnoreCase));
            if (customerNameExists != null)
            {
                return BadRequest("Customer Name already exists");
            }

            if (!string.IsNullOrEmpty(customer.Email))
            {
                Customer emailIdExists = await _context.Customer.FirstOrDefaultAsync(c => c.Email.Equals(customer.Email, StringComparison.OrdinalIgnoreCase));

                if (emailIdExists != null)
                {
                    return BadRequest("Customer Email already exists");
                }
            }

            _context.Customer.Add(customer);
            await _context.SaveChangesAsync();

            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            await _customerLogService.StoreCustomerLogInformationPost(customer, this.ControllerContext.RouteData.Values["action"].ToString(), userId);

            return CreatedAtAction("GetCustomer", new { id = customer.Id }, customer);
        }


        // PUT: api/Customers/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCustomerAsync([FromRoute] int id, [FromBody] Customer customer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != customer.Id)
            {
                  return BadRequest("Requested Customer Id does not match with QueryString Id");
            }

            Customer customerUpdateBefore;
            try
            {
                customerUpdateBefore = _context.Customer.AsNoTracking().FirstOrDefault(c => c.Id == id);

                if (customerUpdateBefore?.Name == customer.Name && customerUpdateBefore?.Email == customer.Email)
                {
                    _context.Entry(customer).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                }
                else
                {
                    var customerNameExists = _context.Customer.Count(c => c.Name.Equals(customer.Name, StringComparison.OrdinalIgnoreCase) && c.Id != customer.Id);
                    var customerEmailIdExists = _context.Customer.Count(c => c.Email.Equals(customer.Email, StringComparison.OrdinalIgnoreCase) && c.Id != customer.Id);

                    if (customerNameExists >= 1 && customerEmailIdExists == 0)
                    {
                        return BadRequest("Customer Name already exists");
                    }

                    if (customerNameExists == 0 && customerEmailIdExists >= 1)
                    {
                        return BadRequest("Customer Email already exists");
                    }

                    if (customerNameExists >= 1 && customerEmailIdExists >= 1)
                    {
                        return BadRequest("Customer Username and Email already exists");
                    }

                    _context.Entry(customer).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerExists(id))
                {
                    return NotFound("Customer Id not found");
                }

                throw;
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            await _customerLogService.StoreCustomerLogInformationPut(customer, this.ControllerContext.RouteData.Values["action"].ToString(), userId, customerUpdateBefore);
            return Ok(customer);
        }

        // DELETE: api/Customers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomerAsync([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var customer = await _context.Customer.FindAsync(id);
            if (customer == null)
            {
                return NotFound("Customer Id not found");
            }

            // check the Customer Id exist in order
             var customerExistOrder = _context.Order.Count(o => o.CustomerId == id && !o.IsDeleted);

            if (customerExistOrder > 0)
            {
                return BadRequest("Not able to delete Customer as it has reference in Order table");
            }

            // check the Customer Id exist in rigLocation
            var customerExistRigLocation = _context.RigLocation.Count(r => r.CustomerId == id && !r.IsDeleted);

            if(customerExistRigLocation > 0)
            {
                return BadRequest("Not able to delete Customer as it has reference in RigLocation table");
            }

            _context.Customer.Remove(customer);
            await _context.SaveChangesAsync();

            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            await _customerLogService.StoreLogInformationDelete(id, this.ControllerContext.RouteData.Values["action"].ToString(), userId);

            return Ok(customer);
        }

        // Check Customer Exist or Not
        private bool CustomerExists(int id)
        {
            return _context.Customer.Any(e => e.Id == id);
        }

    }
}