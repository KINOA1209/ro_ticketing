using Sieve.Attributes;
using System;
using System.ComponentModel.DataAnnotations;

namespace ticketing_api.Models.Views
{
    public class OrderView
    {
        [Sieve(CanSort = true, CanFilter = true)]
        [Key] public int Id { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public DateTime OrderDate { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public DateTime RequestDate { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public string RequestTime { get; set; }

        public RequestDeliveryTimeSlot RequestDeliveryTimeSlotId { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public OrderStatus OrderStatusId { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public JobType JobTypeId { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public Customer CustomerId { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public string CustomerNotes { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public RigLocation RigLocationId { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public string RigLocationNotes { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public string WellName { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public Well WellId { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public string WellCode { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public DriverView DriverId { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public Truck TruckId { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public string AFEPO { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public string PointOfContactName { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public string PointOfContactNumber { get; set; }

        public string PointOfContactEmail { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public Market MarketId { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public County CountyId { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public LoadOrigin LoadOriginId { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public SalesRepView SalesRepId { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public string OrderDescription { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public string WellDirection { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public string InternalNotes { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public bool ShippingPaperNA { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public bool FuelSurchargeNA { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public bool ApprovedForBilling { get; set; }

        public bool ShippingPaperExists { get; set; }

        public bool TicketImageExists { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public string SpecialHandling { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public DateTime DeliveredDate { get; set; }

        public bool IsEnabled { get; set; }
    }
}
