using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Sieve.Attributes;

namespace ticketing_api.Models.Views
{
    public class RigLocationNoteView : RigLocationNote
    {
        [Sieve(CanSort = true, CanFilter = true)]
        [Required] public new RigLocation RigLocationId{ get; set; }
    }
}
