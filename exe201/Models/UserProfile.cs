using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace exe201.Models
{
    public class UserProfile
    {
        [Key, ForeignKey("User")]
        public int UserId { get; set; }

        [MaxLength(255)]
        public string FullName { get; set; }

        [MaxLength(500)]
        public string Address { get; set; }

        [MaxLength(20)]
        public string Phone { get; set; }

        [MaxLength(500)]
        public string AvatarUrl { get; set; }

        public User User { get; set; }
    }

}
