using System;
using Sieve.Attributes;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using ticketing_api.Data;

namespace ticketing_api.Models
{
    public class Product : IAuditable, ISoftDeletable
    {
        [Key] public int Id { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        [Required] public string Name { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public int ProductCategoryId { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public int UnitId { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public Decimal UnitCost { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public Decimal UnitPrice { get; set; }

        //[Sieve(CanSort = true, CanFilter = true)]
        //public bool IsIncludedInReport { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        [Required] public bool IsVisible { get; set; } = true;

        public bool IsEnabled { get; set; } = true;

        [IgnoreDataMember] public bool IsDeleted { get; set; } = false;
    }
}