using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Sieve.Attributes;
using ticketing_api.Data;

namespace ticketing_api.Models.Views
{
    public class ShippingPaperView : ShippingPaper
    {
        [Sieve(CanSort = true, CanFilter = true)]
        public new Market MarketId { get; set; }
    }
}
