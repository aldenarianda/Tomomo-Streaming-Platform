using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Tomomo7.Models;

namespace Tomomo7.Controllers
{
    [Authorize(Roles = "customer")]
    public class HistoryController : Controller
    {
        private readonly Tomomo7Context _context;

        public HistoryController(Tomomo7Context context)
        {
            _context = context;
        }

        // GET: /History/Index
        public async Task<IActionResult> Index()
        {
            var userId = int.Parse(User.FindFirstValue("UserId"));

            var subscriptionHistory = await _context.Subscriptions
                .Where(s => s.UserId == userId)
                .Include(s => s.Plan) 
                .OrderByDescending(s => s.PurchaseDate)
                .ToListAsync();

            return View(subscriptionHistory);
        }
    }
}
