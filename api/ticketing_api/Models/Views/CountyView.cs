using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Sieve.Attributes;

namespace ticketing_api.Models.Views
{
    public class CountyView : County
    {
        [Sieve(CanSort = true, CanFilter = true)]
        public new Market MarketId { get; set; }
    }
}
