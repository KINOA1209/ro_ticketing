using Sieve.Attributes;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using ticketing_api.Data;

namespace ticketing_api.Models
{
    public class Customer : IAuditable, ISoftDeletable
    {
        [Key] public int Id { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
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
        public string BillToAddress { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public string ShipToAddress { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public int PaymentTermId { get; set; }

        public decimal FuelSurchargeFee { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public string Note { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public bool IsVisible { get; set; } = true;

        public bool IsEnabled { get; set; } = true;

        [IgnoreDataMember] public bool IsDeleted { get; set; } = false;
    }
}