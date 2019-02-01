using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Sieve.Attributes;

namespace ticketing_api.Models.Views
{
    public class WellView : Well
    {
        [Sieve(CanSort = true, CanFilter = true)]
        public new RigLocation RigLocationId { get; set; }
    }
}
