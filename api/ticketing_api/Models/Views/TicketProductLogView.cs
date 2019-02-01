using System;
using Sieve.Attributes;

namespace ticketing_api.Models.Views
{
    public class TicketProductLogView
    {
        public int Id { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public int OrderId { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public string ProductName { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public Decimal Quantity { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public string UnitName { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public Decimal Price { get; set; }
    }
}
