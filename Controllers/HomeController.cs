using Gift_of_the_givers.Data;
using Gift_of_the_givers.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace GiftOfTheGivers.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;

        public HomeController(ILogger<HomeController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var dashboard = new DashboardViewModel
            {
                // Count active disasters (status = "Active")
                ActiveDisasters = await _context.Disasters
                    .Where(d => d.Status == "Active")
                    .CountAsync(),

                // Count total approved volunteers
                TotalVolunteers = await _context.Volunteers
                    .Where(v => v.IsApproved)
                    .CountAsync(),

                // Sum all monetary donations
                TotalDonations = await _context.Donations
                    .Where(d => d.Amount.HasValue)
                    .SumAsync(d => d.Amount.Value),

                // Calculate lives impacted (you can modify this logic)
                LivesImpacted = await CalculateLivesImpacted(),

                // Get recent disasters (last 5)
                RecentDisasters = await _context.Disasters
                    .Where(d => d.Status == "Active")
                    .OrderByDescending(d => d.CreatedDate)
                    .Take(5)
                    .ToListAsync(),

                // Get recent donations (last 5)
                RecentDonations = await _context.Donations
                    .Include(d => d.Disaster)
                    .OrderByDescending(d => d.DonationDate)
                    .Take(5)
                    .ToListAsync()
            };

            return View(dashboard);
        }

        private async Task<int> CalculateLivesImpacted()
        {
            // This is a simplified calculation - adjust based on your business logic
            // Example: Assume each active disaster impacts 1000 people on average
            var activeDisasters = await _context.Disasters
                .Where(d => d.Status == "Active")
                .CountAsync();

            return activeDisasters * 1000; // Adjust this multiplier as needed
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

    public class ErrorViewModel
    {
        public string? RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}