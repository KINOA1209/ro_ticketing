using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Sieve.Extensions;
using Sieve.Models;
using Sieve.Services;
using ticketing_api.Models;

namespace ticketing_api.Services
{
    public class ApplicationSieveProcessor : SieveProcessor
    {
        public ApplicationSieveProcessor(
            IOptions<SieveOptions> options,
            ISieveCustomSortMethods customSortMethods,
            ISieveCustomFilterMethods customFilterMethods)
            : base(options, customSortMethods, customFilterMethods)
        {
        }

        public async Task<PagingResults<T>> GetPagingDataAsync<T>(SieveModel sieveModel, IQueryable<T> query)
        {
            query = Apply(sieveModel, query, applyPagination: false);
            var total = await query.CountAsync();
            query = Apply(sieveModel, query, applyFiltering: false, applySorting: false); // Only applies pagination
            var data = new PagingResults<T>
            {
                Page = sieveModel.Page ?? 0,
                Limit = sieveModel.PageSize,
                Total = total,
                Order = sieveModel.Sorts,
                Filters = sieveModel.Filters,
                Items = query
            };

            return data;
        }
    }
}
