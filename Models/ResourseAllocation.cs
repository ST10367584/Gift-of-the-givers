using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gift_of_the_givers.Models
{
    public class ResourceAllocation
    {
        [Key]
        public int AllocationID { get; set; }

        [Required]
        public int DisasterID { get; set; }

        [Required]
        [StringLength(50)]
        public string ResourceType { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string ItemName { get; set; } = string.Empty;

        [Required]
        public int Quantity { get; set; }

        [Required]
        [StringLength(50)]
        public string Unit { get; set; } = "units";

        [StringLength(20)]
        public string Status { get; set; } = "Available";

        [StringLength(500)]
        public string Notes { get; set; } = string.Empty;

        public DateTime AllocationDate { get; set; } = DateTime.UtcNow;
        public DateTime? DistributionDate { get; set; }

        // Navigation properties - FIX THIS
        [ForeignKey("DisasterID")]
        public virtual Disaster Disaster { get; set; } = null!; // Use null! to avoid initialization
    }
}