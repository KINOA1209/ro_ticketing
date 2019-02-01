using Sieve.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace ticketing_api.Models
{
    public class ProductTax
    {
        [Key] public int Id { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public int TaxId { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public int ProductId { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public bool IsEnabled { get; set; } = true;

        [IgnoreDataMember] public bool IsDeleted { get; set; } = false;
    }
}
