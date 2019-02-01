using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Sieve.Attributes;

namespace ticketing_api.Models.Views
{
    public class ProductView : Product
    {
        [Sieve(CanSort = true, CanFilter = true)]
        public new ProductCategory ProductCategoryId { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public new Unit UnitId { get; set; }
    }
}
