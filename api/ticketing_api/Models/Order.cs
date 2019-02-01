using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Sieve.Attributes;
using ticketing_api.Data;
using ticketing_api.Infrastructure;

namespace ticketing_api.Models
{
    public class Order : IAuditable, ISoftDeletable
    {
        [Sieve(CanSort = true, CanFilter = true)]
        [Key] public int Id { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        [Required] public DateTime OrderDate { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        [Required] public DateTime RequestDate { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        [Required] public string RequestTime { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        [Required]
        public int RequestDeliveryTimeSlotId { get; set; } = 0;

        [Sieve(CanSort = true, CanFilter = true)]
        [Required]
        public int CountyId { get; set; } = 0; //AppConstants.OrderStatus.Open;

        [Sieve(CanSort = true, CanFilter = true)]
        [Required]
        public byte OrderStatusId { get; set; } = 1; //AppConstants.OrderStatus.Open;

        [Sieve(CanSort = true, CanFilter = true)]
        public int JobTypeId { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Please enter a CustomerId greater than 0")]
        public int CustomerId { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public string CustomerNotes { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Please enter a RigLocationId greater than 0")]
        public int RigLocationId { get; set; } = 0;

        [Sieve(CanSort = true, CanFilter = true)]
        public string RigLocationNotes { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public string WellName { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public int WellId { get; set; } = 0;

        [Sieve(CanSort = true, CanFilter = true)]
        public string WellCode { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public int DriverId { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public int TruckId { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public string AFEPO { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public string PointOfContactName { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public string PointOfContactNumber { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public string PointOfContactEmail { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public int MarketId { get; set; } = 0;

        [Sieve(CanSort = true, CanFilter = true)]
        public int LoadOriginId { get; set; } = 0;

        [Sieve(CanSort = true, CanFilter = true)]
        public int SalesRepId { get; set; } = 0;

        [Sieve(CanSort = true, CanFilter = true)]
        [Required] public string OrderDescription { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public string WellDirection { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        [MaxLength(1000, ErrorMessage = "Notes cannot be longer than 1000 characters.")]
        public string InternalNotes { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public bool ShippingPaperNA { get; set; } = false;

        [Sieve(CanSort = true, CanFilter = true)]
        public bool FuelSurchargeNA { get; set; } = false;

        [Sieve(CanSort = true, CanFilter = true)]
        public bool ApprovedForBilling { get; set; } = false;

        public bool ShippingPaperExists { get; set; } = false;

        public bool TicketImageExists { get; set; } = false;

        [Sieve(CanSort = true, CanFilter = true)]
        public string SpecialHandling { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public DateTime DeliveredDate { get; set; }

        public bool IsEnabled { get; set; } = true;

        [IgnoreDataMember] public bool IsDeleted { get; set; } = false;
    }
}