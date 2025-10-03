using Gift_of_the_givers.Models;
using System.ComponentModel.DataAnnotations;

namespace Gift_of_the_givers.Models
{
    public class VolunteerAssignment
    {
        [Key]
        public int AssignmentID { get; set; }

        [Required]
        public int VolunteerID { get; set; }

        [Required]
        public int DisasterID { get; set; }

        [Required]
        [StringLength(100)]
        public string Task { get; set; }

        public DateTime AssignedDate { get; set; } = DateTime.UtcNow;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        [StringLength(20)]
        public string Status { get; set; } = "Assigned"; // Assigned, In Progress, Completed

        // Navigation properties
        public virtual Volunteer Volunteer { get; set; }
        public virtual Disaster Disaster { get; set; }
    }
}