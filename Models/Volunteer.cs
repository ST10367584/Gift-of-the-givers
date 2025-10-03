using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gift_of_the_givers.Models
{
    public class Volunteer
    {
        [Key]
        public int VolunteerID { get; set; }

        [Required]
        public int UserID { get; set; }

        [StringLength(20)]
        public string Phone { get; set; } = string.Empty;

        [StringLength(100)]
        public string Address { get; set; } = string.Empty;

        [StringLength(50)]
        public string Skills { get; set; } = string.Empty;

        [StringLength(20)]
        public string Availability { get; set; } = string.Empty;

        public bool IsApproved { get; set; } = false; // This is the missing column

        public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("UserID")]
        public virtual User User { get; set; } = null!;

        public virtual ICollection<VolunteerAssignment> Assignments { get; set; } = new List<VolunteerAssignment>();
    }
}