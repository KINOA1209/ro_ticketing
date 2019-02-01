using System;

namespace ticketing_api.Models.Views
{
    public class OrderLogView
    {
        public int Id { get; set; }

        public DateTime OrderDate { get; set; }

        public DateTime RequestDate { get; set; }

        public string RequestTime { get; set; }

        public string OrderStatus { get; set; }

        public string JobTypeName { get; set; }

        public string CustomerName { get; set; }

        public string CustomerNotes { get; set; }

        public string RigLocationName { get; set; }

        public string RigLocationNotes { get; set; }

        public string WellName { get; set; }

        public string WellCode { get; set; }

        public string DriverName { get; set; }

        public string TruckName { get; set; }

        public string AFEPO { get; set; }

        public string PointOfContactName { get; set; }

        public string PointOfContactNumber { get; set; }

        public string PointOfContactEmail { get; set; }

        public string MarketId { get; set; }

        public string LoadOrigin { get; set; }

        public string SalesRepName { get; set; }

        public string OrderDescription { get; set; }

        public string WellDirection { get; set; }

        public string InternalNotes { get; set; }
    }
}
