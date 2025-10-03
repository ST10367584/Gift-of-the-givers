using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gift_of_the_givers.Models
{
    public class ResourceRequest
    {
        [Key]
        public int RequestID { get; set; }

        [Required]
        public int DisasterID { get; set; }

        [Required]
        [StringLength(50)]
        public string ResourceType { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string ItemName { get; set; } = string.Empty;

        [Required]
        public int QuantityNeeded { get; set; }

        public int QuantityFulfilled { get; set; } = 0;

        [Required]
        [StringLength(20)]
        public string Priority { get; set; } = "Medium";

        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        [StringLength(20)]
        public string Status { get; set; } = "Open";

        public DateTime RequestDate { get; set; } = DateTime.UtcNow;
        public DateTime? Deadline { get; set; }

        // Navigation properties - FIX THIS (remove = new Disaster())
        [ForeignKey("DisasterID")]
        public virtual Disaster Disaster { get; set; } = null!; // Use null! to avoid initialization
    }
}