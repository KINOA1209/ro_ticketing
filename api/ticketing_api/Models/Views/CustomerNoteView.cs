using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Sieve.Attributes;
using ticketing_api.Data;

namespace ticketing_api.Models.Views
{
    public class CustomerNoteView : CustomerNote
    {
        [Sieve(CanSort = true, CanFilter = true)]
        [Required] public new Customer CustomerId { get; set; }
    }
}
