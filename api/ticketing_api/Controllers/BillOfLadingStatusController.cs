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
    public class BillOfLadingStatusController : BaseController.BaseController
    {
        private readonly ILogger<BillOfLadingStatusController> _logger;

        public BillOfLadingStatusController(ApplicationDbContext context, ILogger<BillOfLadingStatusController> logger, IEmailSender emailSender, ISieveProcessor sieveProcessor)
          : base(context, emailSender, sieveProcessor)
        {
            _logger = logger;

        }

        // GET: api/BillOfLadingStatus
        [HttpGet]
        public async Task<IActionResult> GetBillOfLadingStatusAsync([FromQuery] SieveModel sieveModel)
        {
            var query = _context.BillOfLadingStatus.AsQueryable();
            var data = await _sieveProcessor.GetPagingDataAsync(sieveModel, query);
            return Ok(data);
        }

        // GET: api/BillOfLadingStatus/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBillOfLadingStatus([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var billOfLadingStatus = await _context.BillOfLadingStatus.FindAsync(id);

            if (billOfLadingStatus == null)
            {
                return NotFound("Order Status Id not found");
            }

            return Ok(billOfLadingStatus);
        }

        // POST: api/BillOfLadingStatus
        [HttpPost]
        public async Task<IActionResult> PostBillOfLadingStatus([FromBody] BillOfLadingStatus billOfLadingStatus)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var billOfLadingStatusNameExists = await _context.BillOfLadingStatus.SingleOrDefaultAsync(os => os.Name == billOfLadingStatus.Name && os.Name.Equals(billOfLadingStatus.Name, StringComparison.OrdinalIgnoreCase));
            if (billOfLadingStatusNameExists != null)
            {
                return BadRequest("BillOfLadingStatus name already exists");
            }
            else
            {
                _context.BillOfLadingStatus.Add(billOfLadingStatus);
                await _context.SaveChangesAsync();
            }

            return CreatedAtAction("GetBillOfLadingStatus", new { id = billOfLadingStatus.Id }, billOfLadingStatus);
        }

        // PUT: api/BillOfLadingStatus/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBillOfLadingStatus([FromRoute] int id, [FromBody] BillOfLadingStatus billOfLadingStatus)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != billOfLadingStatus.Id)
            {
                return BadRequest("Requested BillOfLadingStatus Id does not match with QueryString Id");
            }

            try
            {
                var billOfLadingStatusName = _context.BillOfLadingStatus.Where(os => os.Id == billOfLadingStatus.Id).Select(os => new { os.Name });

                if (billOfLadingStatusName.FirstOrDefault()?.Name == billOfLadingStatus.Name)
                {
                    _context.Entry(billOfLadingStatus).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                }
                else
                {
                    var billOfLadingStatusNameExists = await _context.BillOfLadingStatus.SingleOrDefaultAsync(os => os.Id != billOfLadingStatus.Id && os.Name.Equals(billOfLadingStatus.Name, StringComparison.OrdinalIgnoreCase));
                    if (billOfLadingStatusNameExists != null)
                    {
                        return BadRequest("BillOfLadingStatus name already exists");
                    }
                    else
                    {
                        _context.Entry(billOfLadingStatus).State = EntityState.Modified;
                        await _context.SaveChangesAsync();
                    }
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BillOfLadingStatusExists(id))
                {
                    return NotFound("Order Status Id not found");
                }
                else
                {
                    throw;
                }
            }

            return Ok(billOfLadingStatus);
        }

        // DELETE: api/BillOfLadingStatus/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBillOfLadingStatus([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var billOfLadingStatus = await _context.BillOfLadingStatus.FindAsync(id);
            if (billOfLadingStatus == null)
            {
                return NotFound("Order Status Id not found");
            }

            // check the BillOfLadingStatus Id exist in Order
            var billOfLadingStatusExistOrder = _context.BillOfLading.Count(o => o.BolStatusId == id && !o.IsDeleted);

            if (billOfLadingStatusExistOrder > 0)
            {
                return BadRequest("Not able to delete BillOfLadingStatus as it has reference in Order table");
            }

            _context.BillOfLadingStatus.Remove(billOfLadingStatus);
            await _context.SaveChangesAsync();

            return Ok(billOfLadingStatus);
        }

        // Check orderstatus Exist or Not
        private bool BillOfLadingStatusExists(int id)
        {
            return _context.BillOfLadingStatus.Any(e => e.Id == id);
        }
    }
}
