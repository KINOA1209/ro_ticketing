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
    public class NotificationTemplate : IAuditable, ISoftDeletable
    {
        [Key] public int Id { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        [Required] public string Name { get; set; }

        [Required] public int NotificationTypeId { get; set; }

        [Required] public int TriggerId { get; set; }

        [Required] public string Recipients { get; set; }

        [Required] public string Subject { get; set; }

        [Required] public string Content { get; set; }

        public bool IsEnabled { get; set; } = true;

        [IgnoreDataMember] public bool IsDeleted { get; set; } = false;
    }
}
