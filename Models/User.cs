using System.ComponentModel.DataAnnotations;

namespace Gift_of_the_givers.Models
{
    public class User
    {
        [Key]
        public int UserID { get; set; }

        [Required]
        [StringLength(50)]
        public string Username { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        public string PasswordSalt { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(100)]
        public string FullName { get; set; }

        [StringLength(20)]
        public string Role { get; set; } = "Volunteer"; // Volunteer, Admin, Donor, Coordinator

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual ICollection<Volunteer> Volunteers { get; set; }
        public virtual ICollection<Donation> Donations { get; set; }
    }
}