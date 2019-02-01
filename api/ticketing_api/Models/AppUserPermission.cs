using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using ticketing_api.Data;

namespace ticketing_api.Models
{
    public class AppUserPermission : IAuditable, ISoftDeletable
    {
        [Key]
        public int Id { get; set; }

        public int AppUserId { get; set; }

        public int PermissionId { get; set; }

        public bool IsEnabled { get; set; } = true;

        [IgnoreDataMember]
        public bool IsDeleted { get; set; } = false;
    }
}