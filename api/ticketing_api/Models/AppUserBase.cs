using System;
using Sieve.Attributes;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Runtime.Serialization;
using System.Threading;
using ticketing_api.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace ticketing_api.Models
{
    public class AppUserBase
    {
        [Sieve(CanSort = true, CanFilter = true)]
        [Required] public string FirstName { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public string MiddleName { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        [Required] public string LastName { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public string EmployeeCode { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public string PhoneNumber { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public string Position { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public string Username { get; set; }

        //[IgnoreDataMember]
        [NotMapped]
        public string Password { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        [Required]
        public DateTime EffectiveDate { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        //[Required]
        //[EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = "";

        [IgnoreDataMember]
        public string FullName => (FirstName ?? "").Trim() + " " + (MiddleName ?? "").Trim() + " " + (LastName ?? "").Trim();

        public void Clean()
        {
            //CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
            //TextInfo textInfo = cultureInfo.TextInfo;
            FirstName = (FirstName?.Trim() ?? "").ToUpper();
            MiddleName = (MiddleName?.Trim() ?? "").ToUpper();
            LastName = (LastName?.Trim() ?? "").ToUpper();
            Position = (Position?.Trim() ?? "").ToUpper();
            PhoneNumber = (PhoneNumber?.Trim() ?? "");
        }
    }
}