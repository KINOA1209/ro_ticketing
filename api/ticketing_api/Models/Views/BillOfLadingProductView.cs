using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Sieve.Attributes;
using ticketing_api.Data;

namespace ticketing_api.Models.Views
{
    public class BillOfLadingProductView
    {
        [Key] public int Id { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public int BillOfLadingId { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public Product ProductId { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public Decimal Quantity { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public Unit UnitId { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public Decimal Cost { get; set; }

        public bool IsEnabled { get; set; }
    }
}
