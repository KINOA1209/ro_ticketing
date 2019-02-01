using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Sieve.Attributes;

namespace ticketing_api.Models.Views
{
    public class MarketTaxView : MarketTax
    {
        [Sieve(CanSort = true, CanFilter = true)]
        public new Market MarketId { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public new Tax TaxId { get; set; }
    }
}
