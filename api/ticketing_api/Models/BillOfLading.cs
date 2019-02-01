using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Sieve.Attributes;
using ticketing_api.Data;

namespace ticketing_api.Models
{
    public class BillOfLading : IAuditable, ISoftDeletable
    {
        [Sieve(CanSort = true, CanFilter = true)]
        [Key] public int Id { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        [Required] public DateTime OrderDate { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        [Required] public DateTime RequestDate { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public DateTime LoadDate { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public DateTime DeliveredDate { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public int MarketId { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public int VendorId { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public int CarrierId { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public int RefineryId { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public int RefineryAddressId { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public int DriverId { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public int TruckId { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public int TerminalId { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public string BolNumber { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public int DestinationId { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public int FinalDestinationId { get; set; }

        public bool ReceivedBol { get; set; }

        public bool ImageExists { get; set; }

        public bool ReceivedInvoice { get; set; }

        public bool PriceCheck { get; set; }

        public bool TaxCheck { get; set; }

        public bool Paid { get; set; }

        public int BolStatusId { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public string Notes { get; set; }

        public bool IsEnabled { get; set; } = true;

        [IgnoreDataMember] public bool IsDeleted { get; set; } = false;
    }
}
