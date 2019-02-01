using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Sieve.Attributes;
using ticketing_api.Data;

namespace ticketing_api.Models.Views
{
    public class TicketTicketProductView
    {
        public Order Ticket { get; set; }

        public IEnumerable<TicketProductView> TicketProducts { get; set; }
    }
}
