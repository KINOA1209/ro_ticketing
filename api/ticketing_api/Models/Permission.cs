using System.ComponentModel.DataAnnotations;
using ticketing_api.Data;

namespace ticketing_api.Models
{
    public class Permission : IAuditable
    {
        [Key]
        public int Id { get; set; }

        [Required]public int ModuleId { get; set; }

        [Required]public string RoleId { get; set; }

        public bool IsRead { get; set; }

        public bool IsCreate { get; set; }

        public bool IsUpdate { get; set; }

        public bool IsDelete { get; set; }
}
}