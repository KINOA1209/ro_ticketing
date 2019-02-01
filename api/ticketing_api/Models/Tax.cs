using Sieve.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using ticketing_api.Data;

namespace ticketing_api.Models
{
    public class Tax : IAuditable, ISoftDeletable
    {
        [Key] public int Id { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public string Name { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public int MarketId { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public string TaxType { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public decimal TaxValue { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public bool IsEnabled { get; set; } = true;

        [IgnoreDataMember] public bool IsDeleted { get; set; } = false;
    }
}
