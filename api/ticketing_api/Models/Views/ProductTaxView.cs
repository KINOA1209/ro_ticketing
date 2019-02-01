using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ticketing_api.Models.Views
{
    public class ProductTaxView : Tax
    {
        public List<int> ProductId { get; set; }

        public List<int> CategoryId { get; set; }
    }
}
