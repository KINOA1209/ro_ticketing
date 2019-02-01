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
    public class TaxService
    {
        private readonly ApplicationDbContext _context;
        private readonly ApplicationSieveProcessor _sieveProcessor;

        public TaxService(ApplicationDbContext context, ISieveProcessor sieveProcessor)
        {
            _context = context;
            _sieveProcessor = (ApplicationSieveProcessor)sieveProcessor;
        }

        public async Task<PagingResults<TaxView>> GetTaxViewAsync(SieveModel sieveModel)
        {
            var query = (from tax in _context.Tax
                           join market in _context.Market on tax.MarketId equals market.Id into m
                           from market in m.DefaultIfEmpty()
                           select new TaxView
                           {
                               Id = tax.Id,
                               ProductId = (from Product in _context.Product
                                            join prodTax in _context.ProductTax on Product.Id equals prodTax.ProductId
                                            where tax.Id == prodTax.TaxId
                                            select new Product
                                            {
                                                Id = Product.Id,
                                                Name = Product.Name,
                                                IsVisible = Product.IsVisible
                                            }).ToList(),
                               CategoryId = (from ProductCategory in _context.ProductCategory
                                             join categoryTax in _context.CategoryTax on ProductCategory.Id equals categoryTax.CategoryId
                                             where tax.Id == categoryTax.TaxId
                                             select new ProductCategory
                                             {
                                                 Id = ProductCategory.Id,
                                                 Name = ProductCategory.Name,
                                                 IsVisible = ProductCategory.IsVisible
                                             }).ToList(),
                               MarketId = market,
                               Name = tax.Name,
                               TaxType = tax.TaxType,
                               TaxValue = tax.TaxValue,
                               IsEnabled = tax.IsEnabled
                           }).AsQueryable();

            var data = await _sieveProcessor.GetPagingDataAsync(sieveModel, query);

            return data;
        }

        public TaxView PostTax(int t)
        {
            var taxView = (from tax in _context.Tax
                           join market in _context.Market on tax.MarketId equals market.Id into m
                           from market in m.DefaultIfEmpty()
                           where tax.Id == t
                           select new TaxView
                           {
                               Id = tax.Id,
                               ProductId = (from Product in _context.Product
                                            join prodTax in _context.ProductTax on Product.Id equals prodTax.ProductId
                                            where tax.Id == prodTax.TaxId
                                            select new Product
                                            {
                                                Id = Product.Id,
                                                Name = Product.Name,
                                                IsVisible = Product.IsVisible
                                            }).ToList(),
                               CategoryId = (from ProductCategory in _context.ProductCategory
                                            join categoryTax in _context.CategoryTax on ProductCategory.Id equals categoryTax.CategoryId
                                            where tax.Id == categoryTax.TaxId
                                            select new ProductCategory
                                            {
                                                Id = ProductCategory.Id,
                                                Name = ProductCategory.Name,
                                                IsVisible = ProductCategory.IsVisible
                                            }).ToList(),
                               MarketId = market,
                               Name = tax.Name,
                               TaxType = tax.TaxType,
                               TaxValue = tax.TaxValue,
                               IsEnabled = tax.IsEnabled
                           }).FirstOrDefault();

            return taxView;
        }
    }
}

