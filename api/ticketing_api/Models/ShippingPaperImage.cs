using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using ticketing_api.Data;

namespace ticketing_api.Models
{
    public class ShippingPaperImage : IAuditable, ISoftDeletable
    {
        [Key] public int Id { get; set; }

        public int TicketId { get; set; }

        public string FileName { get; set; }

        public string FilePath { get; set; }

        public bool IsEnabled { get; set; } = true;

        public DateTime CreatedAt { get; set; }

        [IgnoreDataMember] public bool IsDeleted { get; set; } = false;
    }
}
