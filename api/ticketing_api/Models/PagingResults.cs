using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ticketing_api.Models
{
    public class PagingResults<T>
    {
        public int Page { get; set; }

        public int? Limit { get; set; }

        public long Total { get; set; }

        public string Order { get; set; }

        public string Filters { get; set; }

        public IEnumerable<T> Items { get; set; }

    }
}
