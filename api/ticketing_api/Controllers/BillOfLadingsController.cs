using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    public partial class BillOfLadingsController : BaseController.BaseController
    {
        private readonly ILogger<BillOfLadingsController> _logger;
        private readonly BillOfLadingService _billOfLadingService;

        public BillOfLadingsController(ApplicationDbContext context, ILogger<BillOfLadingsController> logger, IEmailSender emailSender, ISieveProcessor sieveProcessor)
            : base(context, emailSender, sieveProcessor)
        {
            _logger = logger;
            _billOfLadingService = new BillOfLadingService(_context, sieveProcessor);
        }

        [HttpGet]
        public async Task<IActionResult> GetBillOfLadingAsync([FromQuery] SieveModel sieveModel)
        {
            var query = _context.BillOfLading.AsQueryable();
            var data = await _sieveProcessor.GetPagingDataAsync(sieveModel, query);
            return Ok(data);
        }

        [HttpGet("View")]
        public async Task<IActionResult> ViewBillOfLadingAsync([FromQuery]SieveModel sieveModel)
        {
            PagingResults<BillOfLadingView> orderView = await _billOfLadingService.GetBillOfLadingViewAsync(AppUser, sieveModel, 0);
            return Ok(orderView);
        }

        // GET: api/BillOfLadings/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBillOfLading([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var billOfLading = await _context.BillOfLading.FindAsync(id);
            if (billOfLading == null)
            {
                return NotFound("BillOfLading Id not found");
            }

            return Ok(billOfLading);
        }

        // POST: api/BillOfLadings
        [HttpPost]
        public async Task<IActionResult> PostBillOfLading([FromBody] BillOfLading billOfLading)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.BillOfLading.Add(billOfLading);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBillOfLading", new { id = billOfLading.Id }, billOfLading);
        }
        // PUT: api/BillOfLadings/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBillOfLading([FromRoute] int id, [FromBody] BillOfLading billOfLading)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != billOfLading.Id)
                return BadRequest("Requested BillOfLading Id does not match with QueryString Id");

            _context.Entry(billOfLading).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(billOfLading);
        }


        // DELETE: api/BillOfLadings/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBillOfLading([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var billOfLading = await _context.BillOfLading.FindAsync(id);
            if (billOfLading == null)
                return NotFound("BillOfLading Id not found");

            _context.BillOfLading.Remove(billOfLading);
            await _context.SaveChangesAsync();

            return Ok(billOfLading);
        }
    }
}