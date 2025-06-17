using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Tomomo7.Models;
using Tomomo7.ViewModels;

namespace Tomomo7.Controllers
{
    [Authorize(Roles = "customer")]
    public class SubscriptionController : Controller
    {
        private readonly Tomomo7Context _context;

        public SubscriptionController(Tomomo7Context context)
        {
            _context = context;
        }

        // GET: /Subscription/Index
        public async Task<IActionResult> Index()
        {
            var userId = int.Parse(User.FindFirstValue("UserId"));
            var today = DateOnly.FromDateTime(DateTime.Today);

            var viewModel = new SubscriptionViewModel
            {
                AvailablePlans = await _context.SubscriptionPlans
                                               .Where(p => p.IsActive == true)
                                               .OrderBy(p => p.Price)
                                               .ToListAsync(),
                CurrentSubscription = await _context.Subscriptions
                                                    .Include(s => s.Plan)
                                                    .FirstOrDefaultAsync(s => s.UserId == userId && s.Status == "active" && s.EndDate >= today)
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChoosePlan(int planId)
        {
            var userId = int.Parse(User.FindFirstValue("UserId"));
            var today = DateOnly.FromDateTime(DateTime.Today);

            var selectedPlan = await _context.SubscriptionPlans.FindAsync(planId);
            if (selectedPlan == null)
            {
                return NotFound();
            }

            var existingActiveSubscription = await _context.Subscriptions
                .FirstOrDefaultAsync(s => s.UserId == userId && s.Status == "active" && s.EndDate >= today);

            if (existingActiveSubscription != null)
            {
                existingActiveSubscription.Status = "cancelled";
                _context.Update(existingActiveSubscription);
            }

            var newSubscription = new Subscription
            {
                UserId = userId,
                PlanId = selectedPlan.PlanId,
                StartDate = today,
                EndDate = today.AddMonths(selectedPlan.DurationMonths),
                Status = "active",
                PaymentMethod = "Credit Card", 
                PurchaseDate = DateTime.Now
            };

            _context.Subscriptions.Add(newSubscription);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"You have successfully subscribed to the {selectedPlan.PlanName} plan!";
            return RedirectToAction("Index");
        }
    }
}
