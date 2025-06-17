using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;
using Tomomo7.Models;
using Tomomo7.ViewModels;
using Microsoft.AspNetCore.Authentication; 

namespace Tomomo7.Controllers
{
    [Authorize(Roles = "customer")]
    public class ProfileController : Controller
    {
        private readonly Tomomo7Context _context;

        public ProfileController(Tomomo7Context context)
        {
            _context = context;
        }

        // GET: /Profile/Index 
        public async Task<IActionResult> Index()
        {
            var userId = int.Parse(User.FindFirstValue("UserId"));
            var user = await _context.Users.FindAsync(userId);
            if (user == null) { return NotFound(); }
            var viewModel = new ProfileViewModel
            {
                Username = user.Username,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address
            };
            return View(viewModel);
        }

        // POST: /Profile/Index 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(ProfileViewModel model)
        {
            if (!ModelState.IsValid) { return View(model); }
            var userId = int.Parse(User.FindFirstValue("UserId"));
            var user = await _context.Users.FindAsync(userId);
            if (user == null) { return NotFound(); }
            user.Username = model.Username;
            user.PhoneNumber = model.PhoneNumber;
            user.Address = model.Address;
            user.UpdatedAt = DateTime.Now;
            if (!string.IsNullOrEmpty(model.OldPassword) && !string.IsNullOrEmpty(model.NewPassword))
            {
                if (!BCrypt.Net.BCrypt.Verify(model.OldPassword, user.PasswordHash))
                {
                    ModelState.AddModelError("OldPassword", "Current password is incorrect.");
                    return View(model);
                }
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
                TempData["SuccessMessage"] = "Profile and password updated successfully!";
            }
            else
            {
                TempData["SuccessMessage"] = "Profile updated successfully!";
            }
            _context.Update(user);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAccount(ProfileViewModel model)
        {
            var userId = int.Parse(User.FindFirstValue("UserId"));
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            if (string.IsNullOrEmpty(model.PasswordForDelete) || !BCrypt.Net.BCrypt.Verify(model.PasswordForDelete, user.PasswordHash))
            {
                TempData["DeleteErrorMessage"] = "Incorrect password. Account deletion failed.";
                return RedirectToAction("Index");
            }

            var subscriptions = _context.Subscriptions.Where(s => s.UserId == userId);
            _context.Subscriptions.RemoveRange(subscriptions);

            _context.Users.Remove(user);

            await _context.SaveChangesAsync();

            await HttpContext.SignOutAsync("MyCookieAuth");

            TempData["LoginMessage"] = "Your account has been permanently deleted.";
            return RedirectToAction("Login", "Account");
        }
    }
}
