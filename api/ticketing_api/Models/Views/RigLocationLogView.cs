using Sieve.Attributes;

namespace ticketing_api.Models.Views
{
    public class RigLocationLogView
    {
        public int Id { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public string CustomerName { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public string Name { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public string Note { get; set; }
    }
}
