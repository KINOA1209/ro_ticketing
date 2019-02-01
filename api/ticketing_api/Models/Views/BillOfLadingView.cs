using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Sieve.Attributes;

namespace ticketing_api.Models.Views
{
    public class BillOfLadingView
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
        public Market MarketId { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public Vendor VendorId { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public Carrier CarrierId { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public Refinery RefineryId { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public RefineryAddress RefineryAddressId { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public DriverView DriverId { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public Truck TruckId { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public Terminal TerminalId { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public string BolNumber { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public LoadOrigin DestinationId { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public LoadOrigin FinalDestinationId { get; set; }

        public bool ReceivedBol { get; set; }

        public bool ImageExists { get; set; }

        public bool ReceivedInvoice { get; set; }

        public bool PriceCheck { get; set; }

        public bool TaxCheck { get; set; }

        public bool Paid { get; set; }

        public BillOfLadingStatus BolStatusId { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public string Notes { get; set; }

        public bool IsEnabled { get; set; } = true;
    }
}
