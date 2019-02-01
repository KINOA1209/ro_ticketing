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
    public class LoadOrigin : IAuditable, ISoftDeletable
    {
        [Key] public int Id { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        [Required] public string Name { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        [Required] public string Description { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        [Required] public bool IsVisible { get; set; } = true;

        public bool IsEnabled { get; set; } = true;

        [IgnoreDataMember] public bool IsDeleted { get; set; } = false;
    }
}
