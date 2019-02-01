using System;
using System.ComponentModel.DataAnnotations;

namespace ticketing_api.Models
{
    public class Audit
    {
        [Key]
        public int Id { get; set; }

        public string TableName { get; set; }
        public DateTime DateTime { get; set; }
        public string KeyValues { get; set; }
        public string OldValues { get; set; }
        public string NewValues { get; set; }
        public string ChangedBy { get; set; }
    }
}
