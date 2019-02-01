using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace ticketing_api.Models.Views
{
    public class DriverView : AppUser
    {
        //[Key] public int Id { get; set; }

        public int AppUserId { get; set; }

       // public bool IsVisible { get; set; }

       // public bool IsEnabled { get; set; }
    }
}
