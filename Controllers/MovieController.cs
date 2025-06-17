using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Tomomo7.Models;
using Tomomo7.ViewModels;

namespace Tomomo7.Controllers
{
    [Authorize(Roles = "admin")]
    public class MovieController : Controller
    {
        private readonly Tomomo7Context _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public MovieController(Tomomo7Context context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Movie 
        public async Task<IActionResult> Index(string searchString)
        {
            ViewData["CurrentFilter"] = searchString; 

            var moviesQuery = _context.Movies.Include(m => m.Genres).AsQueryable();

            if (!String.IsNullOrEmpty(searchString))
            {
                moviesQuery = moviesQuery.Where(m => m.Title.Contains(searchString)
                                                  || m.Director.Contains(searchString));
            }

            var movies = await moviesQuery.OrderByDescending(m => m.AddedAt).ToListAsync();
            return View(movies);
        }

        // GET: Movie/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movies
                .Include(m => m.Genres) 
                .FirstOrDefaultAsync(m => m.MovieId == id);

            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }


        // GET: Movie/Create
        public async Task<IActionResult> Create()
        {
            var genres = await _context.Genres.OrderBy(g => g.GenreName).ToListAsync();
            var viewModel = new MovieViewModel
            {
                Genres = new MultiSelectList(genres, "GenreId", "GenreName")
            };
            return View(viewModel);
        }

        // POST: Movie/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MovieViewModel viewModel)
        {
            ModelState.Remove("PosterImage");

            if (ModelState.IsValid)
            {
                string? uniqueFileName = await UploadPosterImage(viewModel.PosterImage);

                var movie = new Movie
                {
                    Title = viewModel.Title,
                    Description = viewModel.Description,
                    ReleaseYear = viewModel.ReleaseYear,
                    DurationMinutes = viewModel.DurationMinutes,
                    Rating = viewModel.Rating,
                    Director = viewModel.Director,
                    PosterPath = uniqueFileName,
                    VideoUrl = viewModel.VideoUrl,
                    AddedAt = DateTime.Now
                };

                if (viewModel.SelectedGenreIds != null && viewModel.SelectedGenreIds.Any())
                {
                    var selectedGenres = await _context.Genres.Where(g => viewModel.SelectedGenreIds.Contains(g.GenreId)).ToListAsync();
                    movie.Genres = selectedGenres;
                }

                _context.Movies.Add(movie);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            var genres = await _context.Genres.OrderBy(g => g.GenreName).ToListAsync();
            viewModel.Genres = new MultiSelectList(genres, "GenreId", "GenreName", viewModel.SelectedGenreIds);
            return View(viewModel);
        }

        // GET: Movie/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var movie = await _context.Movies.Include(m => m.Genres).FirstOrDefaultAsync(m => m.MovieId == id);
            if (movie == null) return NotFound();

            var genres = await _context.Genres.OrderBy(g => g.GenreName).ToListAsync();
            var viewModel = new MovieViewModel
            {
                MovieId = movie.MovieId,
                Title = movie.Title,
                Description = movie.Description,
                ReleaseYear = movie.ReleaseYear,
                DurationMinutes = movie.DurationMinutes,
                Rating = movie.Rating,
                Director = movie.Director,
                ExistingPosterPath = movie.PosterPath,
                VideoUrl = movie.VideoUrl,
                SelectedGenreIds = movie.Genres.Select(g => g.GenreId).ToList(),
                Genres = new MultiSelectList(genres, "GenreId", "GenreName", movie.Genres.Select(g => g.GenreId))
            };

            return View(viewModel);
        }

        // POST: Movie/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, MovieViewModel viewModel)
        {
            if (id != viewModel.MovieId) return NotFound();

            ModelState.Remove("PosterImage");

            if (ModelState.IsValid)
            {
                var movieToUpdate = await _context.Movies.Include(m => m.Genres).FirstOrDefaultAsync(m => m.MovieId == id);
                if (movieToUpdate == null) return NotFound();

                movieToUpdate.Title = viewModel.Title;
                movieToUpdate.Description = viewModel.Description;
                movieToUpdate.ReleaseYear = viewModel.ReleaseYear;
                movieToUpdate.DurationMinutes = viewModel.DurationMinutes;
                movieToUpdate.Rating = viewModel.Rating;
                movieToUpdate.Director = viewModel.Director;
                movieToUpdate.VideoUrl = viewModel.VideoUrl;

                if (viewModel.PosterImage != null)
                {
                    if (!string.IsNullOrEmpty(movieToUpdate.PosterPath))
                    {
                        var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, movieToUpdate.PosterPath.TrimStart('/'));
                        if (System.IO.File.Exists(oldImagePath)) { System.IO.File.Delete(oldImagePath); }
                    }
                    movieToUpdate.PosterPath = await UploadPosterImage(viewModel.PosterImage);
                }

                var selectedGenres = await _context.Genres.Where(g => viewModel.SelectedGenreIds.Contains(g.GenreId)).ToListAsync();
                movieToUpdate.Genres = selectedGenres;

                try
                {
                    _context.Update(movieToUpdate);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException) { throw; }
                return RedirectToAction(nameof(Index));
            }

            var genres = await _context.Genres.OrderBy(g => g.GenreName).ToListAsync();
            viewModel.Genres = new MultiSelectList(genres, "GenreId", "GenreName", viewModel.SelectedGenreIds);
            return View(viewModel);
        }

        // GET: Movie/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var movie = await _context.Movies.Include(m => m.Genres).FirstOrDefaultAsync(m => m.MovieId == id);
            if (movie == null) return NotFound();
            return View(movie);
        }

        // POST: Movie/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie != null)
            {
                if (!string.IsNullOrEmpty(movie.PosterPath))
                {
                    var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, movie.PosterPath.TrimStart('/'));
                    if (System.IO.File.Exists(imagePath)) { System.IO.File.Delete(imagePath); }
                }
                _context.Movies.Remove(movie);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private async Task<string?> UploadPosterImage(IFormFile? posterFile)
        {
            if (posterFile == null || posterFile.Length == 0) return null;
            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "posters");
            if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);
            string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(posterFile.FileName);
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);
            using (var fileStream = new FileStream(filePath, FileMode.Create)) { await posterFile.CopyToAsync(fileStream); }
            return "/images/posters/" + uniqueFileName;
        }
    }
}
