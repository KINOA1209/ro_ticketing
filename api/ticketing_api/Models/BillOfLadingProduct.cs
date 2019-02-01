using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using ticketing_api.Data;

namespace ticketing_api.Models
{
    public class BillOfLadingProduct : IAuditable, ISoftDeletable
    {
        [Key] public int Id { get; set; }

        public int BillOfLadingId { get; set; }

        public int ProductId { get; set; }

        public Decimal Quantity { get; set; } = 1.00M;

        public int UnitId { get; set; }

        public Decimal Cost { get; set; }

        public Decimal NetGallons { get; set; }

        public Decimal GrossGallons { get; set; }

        public bool IsEnabled { get; set; } = true;

        [IgnoreDataMember] public bool IsDeleted { get; set; } = false;
    }
}
