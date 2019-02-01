using Sieve.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ticketing_api.Models
{
    public class Log
    {
        [Key]
        public int Id { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public int OrderId { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public string UserId { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public string EventType { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public string Description { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public string IsTable { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public DateTime CreatedTime { get; set; }
    }
}
