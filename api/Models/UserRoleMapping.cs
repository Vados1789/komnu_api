using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.Models
{
    [Table("user_roles_mapping")]
    public class UserRoleMapping
    {
        [Key]
        [Column("user_role_mapping_id")]
        public int UserRoleMappingId { get; set; }

        [Column("user_id")]
        public int UserId { get; set; }

        [Column("role_id")]
        public int RoleId { get; set; }

        //public User User { get; set; }
        //public UserRole Role { get; set; }
    }
}
