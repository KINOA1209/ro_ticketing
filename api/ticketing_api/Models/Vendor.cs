using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Sieve.Attributes;
using ticketing_api.Data;

namespace ticketing_api.Models
{
    public class Vendor : IAuditable, ISoftDeletable
    {
        [Key] public int Id { get; set; }

        [Required] public string Name { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public string ContactName { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public string Email { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public string ContactPhone { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public string FaxNumber { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public string Note { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public bool IsVisible { get; set; } = true;

        public bool IsEnabled { get; set; } = true;

        [IgnoreDataMember] public bool IsDeleted { get; set; } = false;
    }
}