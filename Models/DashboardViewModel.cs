using Gift_of_the_givers.Models;
using System.Collections.Generic;

namespace Gift_of_the_givers.Models
{
    public class DashboardViewModel
    {
        public int ActiveDisasters { get; set; }
        public int TotalVolunteers { get; set; }
        public decimal TotalDonations { get; set; }
        public int LivesImpacted { get; set; }
        public List<Disaster> RecentDisasters { get; set; } = new List<Disaster>();
        public List<Donation> RecentDonations { get; set; } = new List<Donation>();
    }
}