using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Gift_of_the_givers.Data;
using Gift_of_the_givers.Models;

namespace Gift_of_the_givers.Controllers
{
    public class DonationController : Controller
    {
        private readonly AppDbContext _context;

        public DonationController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /Donation
        public async Task<IActionResult> Index()
        {
            var donations = await _context.Donations
                .Include(d => d.Disaster)
                .OrderByDescending(d => d.DonationDate)
                .ToListAsync();
            return View(donations);
        }

        // GET: /Donation/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Disasters = await _context.Disasters
                .Where(d => d.Status == "Active")
                .ToListAsync();
            return View();
        }

        // POST: /Donation/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Donation donation)
        {
            if (ModelState.IsValid)
            {
                donation.DonationDate = DateTime.UtcNow;
                donation.Status = "Pending";

                _context.Donations.Add(donation);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Donation submitted successfully!";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Disasters = await _context.Disasters
                .Where(d => d.Status == "Active")
                .ToListAsync();
            return View(donation);
        }

        // GET: /Donation/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var donation = await _context.Donations
                .Include(d => d.Disaster)
                .FirstOrDefaultAsync(d => d.DonationID == id);

            if (donation == null) return NotFound();

            return View(donation);
        }
    }
}