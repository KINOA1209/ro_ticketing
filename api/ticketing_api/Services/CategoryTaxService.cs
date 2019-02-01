using Microsoft.EntityFrameworkCore;
using Sieve.Models;
using Sieve.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ticketing_api.Data;
using ticketing_api.Models;
using ticketing_api.Models.Views;

namespace ticketing_api.Services
{
    public class CategoryTaxService
    {
        private readonly ApplicationDbContext _context;
        private readonly ApplicationSieveProcessor _sieveProcessor;

        public CategoryTaxService(ApplicationDbContext context, ISieveProcessor sieveProcessor)
        {
            _context = context;
            _sieveProcessor = (ApplicationSieveProcessor)sieveProcessor;
        }

        public async Task<PagingResults<CategoryTaxView>> GetCategoryTaxViewAsync(SieveModel sieveModel)
        {
            var query = from categorytax in _context.CategoryTax
                              join productCategory in _context.ProductCategory on categorytax.CategoryId equals productCategory.Id
                              join tax in _context.Tax on categorytax.TaxId equals tax.Id
                        where !categorytax.IsDeleted
                              select new CategoryTaxView { Id = categorytax.Id, ProductCategoryId = productCategory, TaxId = tax };

            var data = await _sieveProcessor.GetPagingDataAsync(sieveModel, query);

            return data;
        }

        public CategoryTaxView PostCategoryTax(CategoryTax categoryTax)
        {
            CategoryTaxView categoryTaxView = (from categorytax in _context.CategoryTax
                                               join productCategory in _context.ProductCategory on categorytax.CategoryId equals productCategory.Id into pcategory
                                               from productCategory in pcategory.DefaultIfEmpty()
                                               join tax in _context.Tax on categorytax.TaxId equals tax.Id into t
                                               from tax in t.DefaultIfEmpty()
                                               where categorytax.Id == categoryTax.Id && !categorytax.IsDeleted
                                               select new CategoryTaxView
                                               {
                                                   Id = categorytax.Id,
                                                   TaxId = tax,
                                                   ProductCategoryId = productCategory
                                               }).FirstOrDefault();

            return categoryTaxView;
        }
    }
}
