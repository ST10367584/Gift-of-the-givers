using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gift_of_the_givers.Models
{
    public class Disaster
    {
        [Key]
        public int DisasterID { get; set; }

        [Required]
        [StringLength(50)]
        public string DisasterType { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Location { get; set; } = string.Empty;

        [Range(1, 5)]
        public int Severity { get; set; } = 1;

        [StringLength(20)]
        public string Status { get; set; } = "Active";

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedDate { get; set; }

        // Navigation properties - initialize as empty lists
        public virtual ICollection<VolunteerAssignment> VolunteerAssignments { get; set; } = new List<VolunteerAssignment>();
        public virtual ICollection<Donation> Donations { get; set; } = new List<Donation>();
        public virtual ICollection<ResourceAllocation> ResourceAllocations { get; set; } = new List<ResourceAllocation>();
        public virtual ICollection<ResourceRequest> ResourceRequests { get; set; } = new List<ResourceRequest>();
    }
}