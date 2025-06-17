using Microsoft.AspNetCore.Mvc;
using Tomomo7.Models;
using Tomomo7.ViewModels;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore; 

namespace Tomomo7.Controllers
{
    public class AccountController : Controller
    {
        private readonly Tomomo7Context _context;

        public AccountController(Tomomo7Context context)
        {
            _context = context;
        }

        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View(new LoginViewModel());
        }

        // POST: /Account/Login (Memproses data login)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // 1. Cari pengguna berdasarkan email
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);

                // 2. Cek apakah user ada dan password cocok
                if (user != null && BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
                {
                    // 3. Buat "klaim" (identitas) untuk pengguna
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.Username),
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim("UserId", user.UserId.ToString()),
                        new Claim(ClaimTypes.Role, user.Role) // Klaim peran sangat penting!
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, "MyCookieAuth");
                    var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                    // 4. Update waktu login terakhir
                    user.LastLoginAt = DateTime.Now;
                    _context.Users.Update(user);
                    await _context.SaveChangesAsync();

                    // 5. Buat cookie sesi untuk pengguna
                    await HttpContext.SignInAsync("MyCookieAuth", claimsPrincipal);

                    // 6. Arahkan berdasarkan peran (ROLE REDIRECT)
                    if (user.Role == "admin")
                    {
                        return RedirectToAction("Dashboard", "Admin");
                    }
                    else // customer
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }

                // Jika login gagal
                ModelState.AddModelError("", "Email atau password salah.");
            }
            return View(model);
        }

        // GET: /Account/Register (Kode dari tahap sebelumnya)
        [HttpGet]
        public IActionResult Register()
        {
            return View(new RegisterViewModel());
        }

        // POST: /Account/Register (Kode dari tahap sebelumnya)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                
                if (_context.Users.Any(u => u.Username == model.Username) || _context.Users.Any(u => u.Email == model.Email))
                {
                    ModelState.AddModelError("", "Username atau Email sudah ada.");
                    return View(model);
                }

                var user = new User
                {
                    Username = model.Username,
                    Email = model.Email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
                    Role = "customer",
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                
                TempData["SuccessMessage"] = "Registrasi berhasil! Silakan login.";
                return RedirectToAction("Login");
            }
            return View(model);
        }

        // GET: /Account/Logout
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("MyCookieAuth");
            return RedirectToAction("Login");
        }

        // GET: /Account/AccessDenied
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}