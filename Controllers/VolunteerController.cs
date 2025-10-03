using Gift_of_the_givers.Data;
using Gift_of_the_givers.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Gift_of_the_givers.Controllers
{
    public class VolunteerController : Controller
    {
        private readonly AppDbContext _context;

        public VolunteerController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /Volunteer
        public async Task<IActionResult> Index()
        {
            var volunteers = await _context.Volunteers
                .Include(v => v.User)
                .ToListAsync();
            return View(volunteers);
        }

        // GET: /Volunteer/Register
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Volunteer/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(VolunteerRegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Create User first
                    var user = new User
                    {
                        Username = model.Email, // Using email as username for simplicity
                        Email = model.Email,
                        FullName = model.FullName,
                        Role = "Volunteer",
                        PasswordHash = "temp_hash", // You'll need to implement proper auth
                        PasswordSalt = "temp_salt"
                    };

                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();

                    // Create Volunteer linked to the User
                    var volunteer = new Volunteer
                    {
                        UserID = user.UserID,
                        Phone = model.Phone,
                        Address = model.Address,
                        Skills = model.Skills,
                        Availability = model.Availability,
                        IsApproved = false,
                        RegistrationDate = DateTime.UtcNow
                    };

                    _context.Volunteers.Add(volunteer);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Volunteer registration submitted successfully! Awaiting approval.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "An error occurred during registration. Please try again.");
                    // Log the exception
                }
            }
            return View(model);
        }

        // GET: /Volunteer/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var volunteer = await _context.Volunteers
                .Include(v => v.User)
                .FirstOrDefaultAsync(v => v.VolunteerID == id);

            if (volunteer == null) return NotFound();

            return View(volunteer);
        }

        // POST: /Volunteer/Approve/5
        [HttpPost]
        public async Task<IActionResult> Approve(int id)
        {
            var volunteer = await _context.Volunteers.FindAsync(id);
            if (volunteer == null) return NotFound();

            volunteer.IsApproved = true;
            await _context.SaveChangesAsync();

            TempData["Success"] = "Volunteer approved successfully!";
            return RedirectToAction(nameof(Index));
        }
    }

    // View Model for Volunteer Registration
    public class VolunteerRegisterViewModel
    {
        [Required(ErrorMessage = "Full name is required")]
        [StringLength(100, ErrorMessage = "Full name cannot be longer than 100 characters")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; } = string.Empty;

        [StringLength(20)]
        public string Phone { get; set; } = string.Empty;

        [StringLength(100)]
        public string Address { get; set; } = string.Empty;

        [StringLength(500)]
        public string Skills { get; set; } = string.Empty;

        [Required(ErrorMessage = "Availability is required")]
        [StringLength(20)]
        public string Availability { get; set; } = string.Empty;
    }
}