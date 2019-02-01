using Sieve.Attributes;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using ticketing_api.Data;

namespace ticketing_api.Models
{
    public class Driver : IAuditable, ISoftDeletable
    {
        [Key] public int Id { get; set; }

        public int AppUserId { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public bool IsVisible { get; set; } = true;

        public bool IsEnabled { get; set; } = true;

        [IgnoreDataMember] public bool IsDeleted { get; set; } = false;

    }
}