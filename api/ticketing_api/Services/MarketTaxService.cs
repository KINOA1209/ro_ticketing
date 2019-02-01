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
    public class MarketTaxService
    {
        private readonly ApplicationDbContext _context;
        private readonly ApplicationSieveProcessor _sieveProcessor;

        public MarketTaxService(ApplicationDbContext context, ISieveProcessor sieveProcessor)
        {
            _context = context;
            _sieveProcessor = (ApplicationSieveProcessor)sieveProcessor;
        }

        public async Task<PagingResults<MarketTaxView>> GetMarketTaxViewAsync(SieveModel sieveModel)
        {
            var query = from markettax in _context.MarketTax
                            join market in _context.Market on markettax.MarketId equals market.Id into m
                            from market in m.DefaultIfEmpty()
                            join tax in _context.Tax on markettax.TaxId equals tax.Id into t
                            from tax in t.DefaultIfEmpty()
                            select new MarketTaxView { Id = markettax.Id, MarketId = market, Locality = markettax.Locality, TaxId = tax, IsEnabled = markettax.IsEnabled, IsDeleted = markettax.IsDeleted };
            var data = await _sieveProcessor.GetPagingDataAsync(sieveModel, query);

            return data;
        }

        public MarketTaxView PostMarketTax(MarketTax marketTax)
        {
            MarketTaxView marketTaxView = (from markettax in _context.MarketTax
                                           join market in _context.Market on markettax.MarketId equals market.Id
                                           join tax in _context.Tax on markettax.TaxId equals tax.Id
                                           where markettax.Id == marketTax.Id
                                           select new MarketTaxView { Id = markettax.Id, MarketId = market, TaxId = tax, Locality = markettax.Locality, IsEnabled = markettax.IsEnabled, IsDeleted = markettax.IsDeleted }).FirstOrDefault();

            return marketTaxView;
        }

        public IEnumerable<MarketTaxView> GetMarketTaxByMarketId(int marketId)
        {
            IEnumerable<MarketTaxView> marketTaxView = from markettax in _context.MarketTax
                                                       join market in _context.Market on markettax.MarketId equals market.Id
                                                       join tax in _context.Tax on markettax.TaxId equals tax.Id
                                                       where markettax.MarketId == marketId
                                                       select new MarketTaxView { Id = markettax.Id, MarketId = market, TaxId = tax };

            return marketTaxView;
        }
    }
}
