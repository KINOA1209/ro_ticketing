﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using ticketing_api.Data;

namespace ticketing_api.Models
{
    public class RigNote : IAuditable, ISoftDeletable
    {
        [Key] public int Id { get; set; }

        [Required] public string Note { get; set; }

        public bool IsEnabled { get; set; } = true;

        [IgnoreDataMember] public bool IsDeleted { get; set; } = false;
    }
}
