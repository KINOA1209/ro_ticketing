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
    public class ProductCategoriesController : BaseController.BaseController
    {
        private readonly ILogger<ProductCategoriesController> _logger;

        public ProductCategoriesController(ApplicationDbContext context, ILogger<ProductCategoriesController> logger, IEmailSender emailSender, ISieveProcessor sieveProcessor) : base(context, emailSender, sieveProcessor)
        {
            _logger = logger;
        }

        // GET: api/ProductCategories
        [HttpGet]
        public async Task<IActionResult> GetProductCatgory([FromQuery]SieveModel sieveModel)
        {
            var query = _context.ProductCategory.AsQueryable();
            var data = await _sieveProcessor.GetPagingDataAsync(sieveModel, query);
            return Ok(data);
        }

        // GET: api/ProductCategories/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductCatgory([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var ProductCategory = await _context.ProductCategory.FindAsync(id);

            if (ProductCategory == null)
            {
                return NotFound("Product Category Id not found");
            }

            return Ok(ProductCategory);
        }

        // POST: api/ProductCategories
        [HttpPost]
        public async Task<IActionResult> PostProductCatgory([FromBody] ProductCategory productCategory)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var productCategoryNameExists = await _context.ProductCategory.FirstOrDefaultAsync(c => c.Name.Equals(productCategory.Name, StringComparison.OrdinalIgnoreCase));
            if (productCategoryNameExists != null)
            {
                return BadRequest("Product Category name already exists");
            }
            else
            {
                _context.ProductCategory.Add(productCategory);
                await _context.SaveChangesAsync();
            }

            return CreatedAtAction("GetProductCatgory", new { id = productCategory.Id }, productCategory);
        }

        // PUT: api/ProductCategories/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProductCatgory([FromRoute] int id, [FromBody] ProductCategory productCategory)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != productCategory.Id)
            {
                return BadRequest("Requested ProductCategory Id does not match with QueryString Id");
            }

            try
            {
                var category = _context.ProductCategory.Where(c => c.Id == productCategory.Id).Select(c => c.Name).Single();
                if (category == productCategory.Name)
                {
                    _context.Entry(productCategory).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                }
                else
                {
                    var productCategoryNameExists = _context.ProductCategory.Count(c => c.Id != productCategory.Id && c.Name.Equals(productCategory.Name, StringComparison.OrdinalIgnoreCase));
                    if (productCategoryNameExists > 0)
                    {
                        return BadRequest("Product Category name already exists");
                    }
                    else
                    {
                        _context.Entry(productCategory).State = EntityState.Modified;
                        await _context.SaveChangesAsync();
                    }
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductCategoryExists(id))
                {
                    return NotFound("Product Category Id not found");
                }
                else
                {
                    throw;
                }
            }
            return Ok(productCategory);
        }

        // DELETE: api/ProductCategories/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var productCategory = await _context.ProductCategory.FindAsync(id);
            if (productCategory == null)
            {
                return NotFound("Product Category Id not found");
            }

            // check the ProductCategory Id exist in Product
            var productCategoryExistProduct = _context.Product.Count(p => p.ProductCategoryId == id && !p.IsDeleted);

            // check the ProductCategory Id exist in CategoryTax
            var productCategoryExistCategoryTax = _context.CategoryTax.Count(ct => ct.CategoryId == id && !ct.IsDeleted);

            if (productCategoryExistProduct > 0)
            {
                return BadRequest("Not able to delete ProductCategory as it has reference in Product table");
            }
            if(productCategoryExistCategoryTax > 0)
            {
                return BadRequest("Not able to delete ProductCategory as it has reference in CategoryTax table");
            }

            _context.ProductCategory.Remove(productCategory);
            await _context.SaveChangesAsync();

            return Ok(productCategory);
        }

        // Check ProductCategory Exist or Not
        private bool ProductCategoryExists(int id)
        {
            return _context.ProductCategory.Any(e => e.Id == id);
        }
    }
}
