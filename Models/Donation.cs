using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gift_of_the_givers.Models
{
    public class Donation
    {
        [Key]
        public int DonationID { get; set; }

        public int? DonorID { get; set; }
        public int? DisasterID { get; set; }

        [Required]
        [StringLength(50)]
        public string DonorName { get; set; } = string.Empty;

        [EmailAddress]
        public string DonorEmail { get; set; } = string.Empty;

        [StringLength(20)]
        public string DonorPhone { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string ResourceType { get; set; } = string.Empty;

        public decimal? Amount { get; set; }
        public int? Quantity { get; set; }

        [StringLength(100)]
        public string Description { get; set; } = string.Empty;

        [StringLength(20)]
        public string Status { get; set; } = "Pending";

        public DateTime DonationDate { get; set; } = DateTime.UtcNow;

        // Navigation properties - ADD THIS
        [ForeignKey("DisasterID")]
        public virtual Disaster? Disaster { get; set; }
    }
}