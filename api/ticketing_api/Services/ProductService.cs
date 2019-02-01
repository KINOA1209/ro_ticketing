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
    public class ProductService
    {
        private readonly ApplicationDbContext _context;
        private readonly ApplicationSieveProcessor _sieveProcessor;

        public ProductService(ApplicationDbContext context, ISieveProcessor sieveProcessor)
        {
            _context = context;
            _sieveProcessor = (ApplicationSieveProcessor)sieveProcessor;
        }

        public async Task<PagingResults<ProductView>> GetProductViewAsync(SieveModel sieveModel)
        {
            var query = from p in _context.Product
                          join productcategory in _context.ProductCategory on p.ProductCategoryId equals productcategory.Id into pc
                          from productcategory in pc.DefaultIfEmpty()
                          join unit in _context.Unit on p.UnitId equals unit.Id into u
                          from unit in u.DefaultIfEmpty()
                          select new ProductView
                          {
                              Id = p.Id,
                              Name = p.Name,
                              ProductCategoryId = productcategory,
                              UnitId = unit,
                              //UnitCost = p.UnitCost,
                              UnitPrice = p.UnitPrice,
                              //IsIncludedInReport = p.IsIncludedInReport,
                              IsVisible = p.IsVisible,
                              IsEnabled = p.IsEnabled,
                              IsDeleted = p.IsDeleted
                          };

            var data = await _sieveProcessor.GetPagingDataAsync(sieveModel, query);
            return data;
        }

        public ProductView PostProduct(Product product)
        {
            ProductView productView = (from p in _context.Product
                                      join productcategory in _context.ProductCategory on p.ProductCategoryId equals productcategory.Id
                                      join unit in _context.Unit on p.UnitId equals unit.Id
                                      where p.Id == product.Id
                                      select new ProductView
                                      {
                                          Id = p.Id,
                                          Name = p.Name,
                                          ProductCategoryId = productcategory,
                                          UnitId = unit,
                                          UnitCost = p.UnitCost,
                                          UnitPrice = p.UnitPrice,
                                          //IsIncludedInReport = p.IsIncludedInReport,
                                          IsEnabled = p.IsEnabled,
                                          IsDeleted = p.IsDeleted,
                                          IsVisible = p.IsVisible
                                      }).FirstOrDefault();

            return productView;
        }
    }
}
