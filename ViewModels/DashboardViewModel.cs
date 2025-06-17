using Tomomo7.Models;
using System.Collections.Generic;

namespace Tomomo7.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalMovies { get; set; }
        public int TotalUsers { get; set; }
        public int TotalActiveSubscriptions { get; set; }
        public decimal TotalRevenue { get; set; }

        public List<Movie> RecentlyAddedMovies { get; set; } = new List<Movie>();
        public List<User> RecentUsers { get; set; } = new List<User>();

        public Dictionary<string, int> MoviesPerGenre { get; set; } = new Dictionary<string, int>();
        public List<Movie> MostFavoritedMovies { get; set; } = new List<Movie>();
    }
}
