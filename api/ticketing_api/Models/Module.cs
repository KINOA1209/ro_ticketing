using Sieve.Attributes;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using ticketing_api.Data;

namespace ticketing_api.Models
{
    public class Module : IAuditable, ISoftDeletable
    {
        [Key]
        public int Id { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        [Required] public string ModuleName { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public string NormalizedName { get; set; }

        public bool IsEnabled { get; set; } = true;

        [IgnoreDataMember] public bool IsDeleted { get; set; } = false;

    }
}
