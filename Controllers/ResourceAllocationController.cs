using Gift_of_the_givers.Data;
using Gift_of_the_givers.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Gift_of_the_givers.Controllers
{
    public class ResourceAllocationController : Controller
    {
        private readonly AppDbContext _context;

        public ResourceAllocationController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /ResourceAllocation
        public async Task<IActionResult> Index(int? disasterId, string resourceType, string status)
        {
            var allocations = _context.ResourceAllocations
                .Include(ra => ra.Disaster)
                .AsQueryable();

            // Apply filters
            if (disasterId.HasValue)
            {
                allocations = allocations.Where(ra => ra.DisasterID == disasterId.Value);
            }

            if (!string.IsNullOrEmpty(resourceType))
            {
                allocations = allocations.Where(ra => ra.ResourceType == resourceType);
            }

            if (!string.IsNullOrEmpty(status))
            {
                allocations = allocations.Where(ra => ra.Status == status);
            }

            var result = await allocations.OrderByDescending(ra => ra.AllocationDate).ToListAsync();

            // Pass disasters for filter dropdown
            ViewBag.Disasters = await _context.Disasters
                .Where(d => d.Status == "Active")
                .OrderBy(d => d.Title)
                .ToListAsync();

            return View(result);
        }

        // GET: /ResourceAllocation/Allocate
        public async Task<IActionResult> Allocate()
        {
            ViewBag.Disasters = await _context.Disasters
                .Where(d => d.Status == "Active")
                .OrderBy(d => d.Title)
                .ToListAsync();

            return View();
        }

        // POST: /ResourceAllocation/Allocate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Allocate(ResourceAllocation allocation)
        {
            if (ModelState.IsValid)
            {
                allocation.AllocationDate = DateTime.UtcNow;
                allocation.Status = "Available";

                _context.ResourceAllocations.Add(allocation);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Resources allocated successfully!";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Disasters = await _context.Disasters
                .Where(d => d.Status == "Active")
                .OrderBy(d => d.Title)
                .ToListAsync();

            return View(allocation);
        }

        // GET: /ResourceAllocation/Requests
        public async Task<IActionResult> Requests(int? disasterId, string priority, string status)
        {
            var requests = _context.ResourceRequests
                .Include(rr => rr.Disaster)
                .AsQueryable();

            // Apply filters
            if (disasterId.HasValue)
            {
                requests = requests.Where(rr => rr.DisasterID == disasterId.Value);
            }

            if (!string.IsNullOrEmpty(priority))
            {
                requests = requests.Where(rr => rr.Priority == priority);
            }

            if (!string.IsNullOrEmpty(status))
            {
                requests = requests.Where(rr => rr.Status == status);
            }

            var result = await requests.OrderByDescending(rr => rr.RequestDate).ToListAsync();

            ViewBag.Disasters = await _context.Disasters
                .Where(d => d.Status == "Active")
                .OrderBy(d => d.Title)
                .ToListAsync();

            return View(result);
        }

        // GET: /ResourceAllocation/CreateRequest
        public async Task<IActionResult> CreateRequest()
        {
            ViewBag.Disasters = await _context.Disasters
                .Where(d => d.Status == "Active")
                .OrderBy(d => d.Title)
                .ToListAsync();

            return View();
        }

        // POST: /ResourceAllocation/CreateRequest
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRequest(ResourceRequest request)
        {
            if (ModelState.IsValid)
            {
                request.RequestDate = DateTime.UtcNow;
                request.Status = "Open";
                request.QuantityFulfilled = 0;

                _context.ResourceRequests.Add(request);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Resource request created successfully!";
                return RedirectToAction(nameof(Requests));
            }

            ViewBag.Disasters = await _context.Disasters
                .Where(d => d.Status == "Active")
                .OrderBy(d => d.Title)
                .ToListAsync();

            return View(request);
        }

        // POST: /ResourceAllocation/FulfillRequest
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FulfillRequest(int requestId, int quantity)
        {
            var request = await _context.ResourceRequests.FindAsync(requestId);
            if (request == null)
            {
                return NotFound();
            }

            request.QuantityFulfilled += quantity;

            if (request.QuantityFulfilled >= request.QuantityNeeded)
            {
                request.Status = "Fulfilled";
            }
            else if (request.QuantityFulfilled > 0)
            {
                request.Status = "Partially Fulfilled";
            }

            _context.ResourceRequests.Update(request);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Request updated! {quantity} units fulfilled.";
            return RedirectToAction(nameof(Requests));
        }

        // GET: /ResourceAllocation/Statistics
        public async Task<IActionResult> Statistics()
        {
            var stats = new ResourceStatisticsViewModel
            {
                TotalResourcesAllocated = await _context.ResourceAllocations.SumAsync(ra => ra.Quantity),
                TotalRequests = await _context.ResourceRequests.CountAsync(),
                OpenRequests = await _context.ResourceRequests.CountAsync(rr => rr.Status == "Open"),
                CriticalRequests = await _context.ResourceRequests.CountAsync(rr => rr.Priority == "Critical"),
                ResourcesByType = await _context.ResourceAllocations
                    .GroupBy(ra => ra.ResourceType)
                    .Select(g => new ResourceTypeSummary
                    {
                        ResourceType = g.Key,
                        TotalQuantity = g.Sum(ra => ra.Quantity),
                        Count = g.Count()
                    })
                    .ToListAsync(),
                RequestsByDisaster = await _context.ResourceRequests
                    .Include(rr => rr.Disaster)
                    .GroupBy(rr => rr.Disaster.Title)
                    .Select(g => new DisasterRequestSummary
                    {
                        DisasterName = g.Key,
                        RequestCount = g.Count(),
                        TotalNeeded = g.Sum(rr => rr.QuantityNeeded),
                        TotalFulfilled = g.Sum(rr => rr.QuantityFulfilled)
                    })
                    .ToListAsync()
            };

            return View(stats);
        }

        // POST: /ResourceAllocation/UpdateStatus
        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int allocationId, string status)
        {
            var allocation = await _context.ResourceAllocations.FindAsync(allocationId);
            if (allocation == null)
            {
                return NotFound();
            }

            allocation.Status = status;
            if (status == "Distributed")
            {
                allocation.DistributionDate = DateTime.UtcNow;
            }

            _context.ResourceAllocations.Update(allocation);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Status updated successfully!" });
        }
    }

    // View Models for Statistics
    public class ResourceStatisticsViewModel
    {
        public int TotalResourcesAllocated { get; set; }
        public int TotalRequests { get; set; }
        public int OpenRequests { get; set; }
        public int CriticalRequests { get; set; }
        public List<ResourceTypeSummary> ResourcesByType { get; set; } = new List<ResourceTypeSummary>();
        public List<DisasterRequestSummary> RequestsByDisaster { get; set; } = new List<DisasterRequestSummary>();
    }

    public class ResourceTypeSummary
    {
        public string ResourceType { get; set; } = string.Empty;
        public int TotalQuantity { get; set; }
        public int Count { get; set; }
    }

    public class DisasterRequestSummary
    {
        public string DisasterName { get; set; } = string.Empty;
        public int RequestCount { get; set; }
        public int TotalNeeded { get; set; }
        public int TotalFulfilled { get; set; }
    }
}