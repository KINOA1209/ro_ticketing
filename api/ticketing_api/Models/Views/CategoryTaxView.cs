using System.ComponentModel.DataAnnotations;
using Sieve.Attributes;

namespace ticketing_api.Models.Views
{
    public class CategoryTaxView : CategoryTax
    {
        [Sieve(CanSort = true, CanFilter = true)]
        public ProductCategory ProductCategoryId { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public new Tax TaxId { get; set; }
    }
}
