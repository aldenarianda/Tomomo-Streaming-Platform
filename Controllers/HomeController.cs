using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;
using Tomomo7.Models;
using Tomomo7.ViewModels;

namespace Tomomo7.Controllers
{
    [Authorize(Roles = "customer")]
    public class HomeController : Controller
    {
        private readonly Tomomo7Context _context;

        public HomeController(Tomomo7Context context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int? genreId, string searchString)
        {
            ViewData["CurrentGenre"] = genreId;
            ViewData["CurrentSearch"] = searchString;
            var moviesQuery = _context.Movies.Include(m => m.Genres).AsQueryable();

            if (!string.IsNullOrEmpty(searchString)) { moviesQuery = moviesQuery.Where(m => m.Title.Contains(searchString)); }
            if (genreId.HasValue) { moviesQuery = moviesQuery.Where(m => m.Genres.Any(g => g.GenreId == genreId.Value)); }

            var allMovies = await moviesQuery.OrderByDescending(m => m.ReleaseYear).ToListAsync();

            var viewModel = new BrowseViewModel
            {
                FeaturedMovie = allMovies.FirstOrDefault(),
                Movies = allMovies,
                AllGenres = await _context.Genres.OrderBy(g => g.GenreName).ToListAsync(),
                CurrentGenreId = genreId
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Details(int id)
        {
            var userIdString = User.FindFirstValue("UserId");
            if (!int.TryParse(userIdString, out int userId)) { return Unauthorized(); }

            var today = DateOnly.FromDateTime(DateTime.Today);
            var activeSubscription = await _context.Subscriptions.FirstOrDefaultAsync(s => s.UserId == userId && s.Status == "active" && s.EndDate >= today);
            if (activeSubscription == null) { return RedirectToAction("Index", "Subscription"); }

            var movie = await _context.Movies.Include(m => m.Genres).FirstOrDefaultAsync(m => m.MovieId == id);
            if (movie == null) { return NotFound(); }

            ViewData["IsFavorited"] = await _context.UserFavorites.AnyAsync(f => f.UserId == userId && f.MovieId == id);

            return View(movie);
        }
    }
}
