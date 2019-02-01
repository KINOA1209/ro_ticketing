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
using ticketing_api.Models.Views;
using ticketing_api.Services;

namespace ticketing_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryTaxesController : BaseController.BaseController
    {

        private readonly ILogger<CategoryTaxesController> _logger;
        private readonly CategoryTaxService _categoryTaxService;

        public CategoryTaxesController(ApplicationDbContext context, ILogger<CategoryTaxesController> logger, IEmailSender emailSender, ISieveProcessor sieveProcessor) : base(context, emailSender, sieveProcessor)
        {
            _logger = logger;
            _categoryTaxService = new CategoryTaxService(_context, sieveProcessor);
        }

        [HttpGet]
        public async Task<IActionResult> GetCategoryTaxAsync([FromQuery]SieveModel sieveModel)
        {
            var categoryTax = await _categoryTaxService.GetCategoryTaxViewAsync(sieveModel);
            return Ok(categoryTax);
        }

        // GET: api/CategoryTaxes/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryTax([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var categoryTax = await _context.CategoryTax.FindAsync(id);

            if (categoryTax == null)
            {
                return NotFound("Category Tax Id not found");
            }

            return Ok(categoryTax);
        }

        // POST: api/CategoryTaxes
        [HttpPost]
        public async Task<IActionResult> PostCategoryTax([FromBody] CategoryTax categoryTax)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            _context.CategoryTax.Add(categoryTax);
            await _context.SaveChangesAsync();

            var categoryTaxView = _categoryTaxService.PostCategoryTax(categoryTax);

            return CreatedAtAction("GetCategoryTax", new { id = categoryTax.Id }, categoryTaxView);
        }

        // PUT: api/CategoryTaxes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCategoryTax([FromRoute] int id, [FromBody] CategoryTax categoryTax)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != categoryTax.Id)
            {
                return BadRequest("Requested CategoryTax Id does not match with QueryString Id");
            }

            try
            {
                _context.Entry(categoryTax).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoryTaxExists(id))
                {
                    return NotFound("Category Tax Id not found");
                }
                else
                {
                    throw;
                }
            }

            var categoryTaxView = _categoryTaxService.PostCategoryTax(categoryTax);
            
            return Ok(categoryTaxView);
        }

        // DELETE: api/CategoryTaxes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategoryTax([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var categoryTax = await _context.CategoryTax.FindAsync(id);
            if (categoryTax == null)
            {
                return NotFound("Category Tax Id not found");
            }

            _context.CategoryTax.Remove(categoryTax);
            await _context.SaveChangesAsync();

            return Ok(categoryTax);
        }

        // Check CategoryTax Exist or Not
        private bool CategoryTaxExists(int id)
        {
            return _context.CategoryTax.Any(e => e.Id == id);
        }
    }
}
