using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Tomomo7.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Tomomo7.ViewModels;

namespace Tomomo7.Controllers
{
    [Authorize(Roles = "admin")]
    public class AdminController : Controller
    {
        private readonly Tomomo7Context _context;

        public AdminController(Tomomo7Context context)
        {
            _context = context;
        }

        public async Task<IActionResult> Dashboard()
        {
            var today = DateOnly.FromDateTime(DateTime.Today);

            // Ambil ID film yang paling banyak difavoritkan
            var mostFavoritedMovieIds = await _context.UserFavorites
                .GroupBy(f => f.MovieId)
                .OrderByDescending(g => g.Count())
                .Take(5)
                .Select(g => g.Key)
                .ToListAsync();

            // Ambil detail film berdasarkan ID yang sudah diurutkan
            var mostFavoritedMovies = await _context.Movies
                .Where(m => mostFavoritedMovieIds.Contains(m.MovieId))
                .ToListAsync();

            var viewModel = new DashboardViewModel
            {
                // Statistik Utama
                TotalMovies = await _context.Movies.CountAsync(),
                TotalUsers = await _context.Users.CountAsync(),
                TotalActiveSubscriptions = await _context.Subscriptions.CountAsync(s => s.Status == "active" && s.EndDate >= today),
                TotalRevenue = await _context.Subscriptions.Include(s => s.Plan).SumAsync(s => s.Plan.Price),

                // Daftar Aktivitas
                RecentlyAddedMovies = await _context.Movies
                                                    .OrderByDescending(m => m.AddedAt)
                                                    .Take(5)
                                                    .ToListAsync(),
                RecentUsers = await _context.Users
                                            .OrderByDescending(u => u.CreatedAt)
                                            .Take(5)
                                            .ToListAsync(),

                // Data Chart & Peringkat
                MoviesPerGenre = await _context.Genres
                                               .Include(g => g.Movies)
                                               .Where(g => g.Movies.Any()) 
                                               .ToDictionaryAsync(g => g.GenreName, g => g.Movies.Count),

                
                MostFavoritedMovies = mostFavoritedMovieIds
                                        .Select(id => mostFavoritedMovies.FirstOrDefault(m => m.MovieId == id))
                                        .Where(m => m != null)
                                        .ToList()
            };

            return View(viewModel);
        }
    }
}
