using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Sieve.Attributes;

namespace ticketing_api.Models.Views
{
    public class RigLocationView : RigLocation
    {
        [Sieve(CanSort = true, CanFilter = true)]
        public new Customer CustomerId { get; set; }
    }
}
