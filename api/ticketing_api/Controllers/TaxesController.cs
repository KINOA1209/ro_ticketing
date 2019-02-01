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
    public class TaxesController : BaseController.BaseController
    {
        private readonly ILogger<TaxesController> _logger;
        private readonly TaxService _taxService;

        public TaxesController(ApplicationDbContext context, ILogger<TaxesController> logger, IEmailSender emailSender, ISieveProcessor sieveProcessor) : base(context, emailSender, sieveProcessor)
        {
            _logger = logger;
            _taxService = new TaxService(_context, sieveProcessor);
        }

        [HttpGet]
        public async Task<IActionResult> GetTaxAsync([FromQuery]SieveModel sieveModel)
        {
            PagingResults<TaxView> taxView = await _taxService.GetTaxViewAsync(sieveModel);
            return Ok(taxView);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTaxAsync([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var tax = await _context.Tax.FindAsync(id);

            if (tax == null)
            {
                return BadRequest("Tax Id not found");
            }

            return Ok(tax);
        }

        [HttpPost]
        public async Task<IActionResult> PostTaxAsync([FromBody] ProductTaxView productTaxView)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            //to insert Tax table records
            Tax tax = new Tax
            {
                Name = productTaxView.Name,
                MarketId = productTaxView.MarketId,
                TaxType = productTaxView.TaxType,
                TaxValue = productTaxView.TaxValue
            };
            _context.Tax.Add(tax);
            await _context.SaveChangesAsync();

            int taxId = tax.Id;

            //to insert ProductTax table records
            var productList = productTaxView.ProductId;
            if (productList.Count > 0)
            {
                for (int i = 0; i < productList.Count; i++)
                {
                    ProductTax productTax = new ProductTax { };

                    productTax.TaxId = taxId;
                    productTax.ProductId = productList[i];
                    _context.ProductTax.Add(productTax);
                    await _context.SaveChangesAsync();
                }
            }

            //to insert CategoryTax table records
            var categoryList = productTaxView.CategoryId;
            if (categoryList.Count > 0)
            {
                for (int i = 0; i < categoryList.Count; i++)
                {
                    CategoryTax categoryTax = new CategoryTax { };

                    categoryTax.TaxId = taxId;
                    categoryTax.CategoryId = categoryList[i];
                    _context.CategoryTax.Add(categoryTax);
                    await _context.SaveChangesAsync();
                }
            }

            return CreatedAtAction("GetTaxAsync", new { id = tax.Id }, _taxService.PostTax(tax.Id));
        }

        // PUT: api/Taxes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTaxAsync([FromRoute] int id, [FromBody] ProductTaxView productTaxView)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (id != productTaxView.Id)
            {
                return BadRequest("Requested Tax Id does not match with QueryString Id");
            }

            try
            {
                Tax tax = new Tax
                {
                    Id = productTaxView.Id,
                    Name = productTaxView.Name,
                    MarketId = productTaxView.MarketId,
                    TaxType = productTaxView.TaxType,
                    TaxValue = productTaxView.TaxValue
                };
                _context.Entry(tax).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                //update producttax
                if (productTaxView.ProductId.Count > 0)
                {
                    var productList = productTaxView.ProductId;
                    var productTaxList = (from p in _context.ProductTax
                                      where p.TaxId == productTaxView.Id
                                      select new ProductTax { Id = p.Id, TaxId = p.TaxId, ProductId = p.ProductId }).ToList();
                    if (productTaxList.Count > 0)
                    {
                        for (int i = 0; i < productTaxList.Count; i++)
                        {
                            _context.ProductTax.Remove(productTaxList[i]);
                            await _context.SaveChangesAsync();
                        }
                    }

                    for (int i = 0; i < productList.Count; i++)
                    {
                        ProductTax productTax = new ProductTax { };
                        productTax.TaxId = productTaxView.Id;
                        productTax.ProductId = productList[i];
                        _context.ProductTax.Add(productTax);
                        await _context.SaveChangesAsync();
                    }
                }

                //update categorytax
                if (productTaxView.CategoryId.Count > 0)
                {
                    var categoryList = productTaxView.CategoryId;
                    var categoryTaxList = (from c in _context.CategoryTax
                                       where c.TaxId == productTaxView.Id
                                       select new CategoryTax { Id = c.Id, TaxId = c.TaxId, CategoryId = c.CategoryId }).ToList();
                    if (categoryTaxList.Count > 0)
                    {
                        for (int i = 0; i < categoryTaxList.Count; i++)
                        {
                            _context.CategoryTax.Remove(categoryTaxList[i]);
                            await _context.SaveChangesAsync();
                        }
                    }
                    for (int i = 0; i < categoryList.Count; i++)
                    {
                        CategoryTax categoryTax = new CategoryTax { };
                        categoryTax.TaxId = productTaxView.Id;
                        _context.CategoryTax.Add(categoryTax);
                        categoryTax.CategoryId = categoryList[i];
                        await _context.SaveChangesAsync();
                    }
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TaxExists(id))
                {
                    return NotFound("Tax Id not found");
                }
                throw;
            }
            TaxView taxView = _taxService.PostTax(id);
            return Ok(taxView);
        }

        // DELETE: api/Taxes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTaxAsync([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var tax = await _context.Tax.FindAsync(id);
            if (tax == null)
            {
                return NotFound("Tax Id not found");
            }

            //check the Tax Id exist in TicketTax 
            var taxExistTicketTax = _context.TicketTax.Count(t => t.TaxId == id && !t.IsDeleted);

            if(taxExistTicketTax > 0)
            {
                return BadRequest("Not able to delete Tax as it has reference in TicketTax table");
            }

            _context.Tax.Remove(tax);
            await _context.SaveChangesAsync();

            //delete producttax
            List<ProductTax> productTaxList = new List<ProductTax> { };
            productTaxList = (from p in _context.ProductTax
                              where p.TaxId == id
                              select new ProductTax { Id = p.Id, TaxId = p.TaxId, ProductId = p.ProductId }).ToList();
            if (productTaxList.Count > 0)
            {
                for (int i = 0; i < productTaxList.Count; i++)
                {
                    productTaxList[i].IsDeleted = true;
                    _context.Entry(productTaxList[i]).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                }
            }

            //delete categorytax
            List<CategoryTax> categoryTaxList = new List<CategoryTax> { };
            categoryTaxList = (from c in _context.CategoryTax
                               where c.TaxId == id
                               select new CategoryTax { Id = c.Id, TaxId = c.TaxId, CategoryId = c.CategoryId }).ToList();
            if (categoryTaxList.Count > 0)
            {
                for (int i = 0; i < categoryTaxList.Count; i++)
                {
                    categoryTaxList[i].IsDeleted = true;
                    _context.Entry(categoryTaxList[i]).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                }
            }
            return Ok(tax);
        }

        // Check Tax Exist or Not
        private bool TaxExists(int id)
        {
            return _context.Tax.Any(e => e.Id == id);
        }
    }
}
