using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ticketing_api.Data;

namespace ticketing_api.Models
{
    public class UserRoles : IAuditable
    {
        [Key]
        public int UserId { get; set; }

        public int RoleId { get; set; }

    }
}
