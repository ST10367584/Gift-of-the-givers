using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Gift_of_the_givers.Data;
using Gift_of_the_givers.Models;

namespace Gift_of_the_givers.Controllers
{
    public class DisasterController : Controller
    {
        private readonly AppDbContext _context;

        public DisasterController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /Disaster
        public async Task<IActionResult> Index()
        {
            var disasters = await _context.Disasters
                .OrderByDescending(d => d.CreatedDate)
                .ToListAsync();
            return View(disasters);
        }

        // GET: /Disaster/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Disaster/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Disaster disaster)
        {
            if (ModelState.IsValid)
            {
                disaster.CreatedDate = DateTime.UtcNow;
                disaster.Status = "Active";

                _context.Disasters.Add(disaster);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Disaster reported successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(disaster);
        }

        // GET: /Disaster/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var disaster = await _context.Disasters
                .FirstOrDefaultAsync(d => d.DisasterID == id);

            if (disaster == null) return NotFound();

            return View(disaster);
        }
    }
}