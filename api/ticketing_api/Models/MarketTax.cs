using Sieve.Attributes;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using ticketing_api.Data;

namespace ticketing_api.Models
{
    public class MarketTax : IAuditable, ISoftDeletable
    {
        [Key] public int Id { get; set; }
     
        [Sieve(CanSort = true, CanFilter = true)]
        public int MarketId { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public int TaxId { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public string Locality{ get; set; }
      
        [Sieve(CanSort = true, CanFilter = true)]
        public bool IsEnabled { get; set; } = true;

        [IgnoreDataMember] public bool IsDeleted { get; set; } = false;
    }
}