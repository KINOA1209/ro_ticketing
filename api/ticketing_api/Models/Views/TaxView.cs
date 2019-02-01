using System.Collections.Generic;
using Sieve.Attributes;

namespace ticketing_api.Models.Views
{
    public class TaxView
    {
        public int Id { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public List<Product> ProductId { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public List<ProductCategory> CategoryId { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public Market MarketId { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public string Name { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public string TaxType { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public decimal TaxValue { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public bool IsEnabled { get; set; }
    }
}
