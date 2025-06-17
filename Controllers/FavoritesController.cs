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
    public class FavoritesController : Controller
    {
        private readonly Tomomo7Context _context;

        public FavoritesController(Tomomo7Context context)
        {
            _context = context;
        }

        // GET: /Favorites/Index (Halaman "My List")
        public async Task<IActionResult> Index()
        {
            var userId = int.Parse(User.FindFirstValue("UserId"));

            
            var favoriteMovies = await _context.UserFavorites
                .Where(f => f.UserId == userId)
                .Include(f => f.Movie) 
                    .ThenInclude(m => m.Genres) 
                .OrderByDescending(f => f.AddedAt)
                .Select(f => f.Movie) 
                .ToListAsync();

            return View(favoriteMovies);
        }

        // POST: /Favorites/ToggleFavorite/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleFavorite(int movieId)
        {
            var userId = int.Parse(User.FindFirstValue("UserId"));

            
            var existingFavorite = await _context.UserFavorites
                .FirstOrDefaultAsync(f => f.UserId == userId && f.MovieId == movieId);

            if (existingFavorite == null)
            {
                
                var newFavorite = new UserFavorite
                {
                    UserId = userId,
                    MovieId = movieId,
                    AddedAt = DateTime.Now
                };
                _context.UserFavorites.Add(newFavorite);
            }
            else
            {
                _context.UserFavorites.Remove(existingFavorite);
            }

            await _context.SaveChangesAsync();

            return Redirect(Request.Headers["Referer"].ToString());
        }
    }
}
