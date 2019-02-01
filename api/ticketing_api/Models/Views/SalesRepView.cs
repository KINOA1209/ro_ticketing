using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Sieve.Attributes;

namespace ticketing_api.Models.Views
{
    public class SalesRepView : AppUser
    {
        public int AppUserId { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public string Notes { get; set; }

      //  public bool IsVisible { get; set; }

       // public bool IsEnabled { get; set; }
    }
}
