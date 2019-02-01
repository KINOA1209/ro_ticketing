using Sieve.Attributes;
using System;
using System.ComponentModel.DataAnnotations;
using ticketing_api.Data;

namespace ticketing_api.Models
{
    public class AppUser : AppUserBase, IAuditable, ISoftDeletable, IComparable<AppUser>
    {
        [Key]
        public int Id { get; set; }

        public string AspNetUserId { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public int ClientId { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        [Required]
        public bool IsVisible { get; set; } = true;

        [Sieve(CanSort = true, CanFilter = true)]
        [Required]
        public string Role { get; set; }

        public bool IsEnabled { get; set; } = true;

        //todo: this will be stored else where, but for now stored here
        public string SetPasswordToken { get; set; }

        public bool IsDeleted { get; set; } = false;

        public int CompareTo(AppUser other)
        {
            return string.Compare(FullName ?? "", other.FullName ?? "", StringComparison.OrdinalIgnoreCase);
        }
    }
}
