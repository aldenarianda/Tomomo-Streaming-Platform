using Tomomo7.Models;

namespace Tomomo7.ViewModels
{
    public class BrowseViewModel
    {
        public Movie? FeaturedMovie { get; set; }

        public List<Movie> Movies { get; set; } = new List<Movie>();
        public List<Genre> AllGenres { get; set; } = new List<Genre>();
        public int? CurrentGenreId { get; set; }
    }
}