using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Tomomo7.ViewModels
{
    public class MovieViewModel
    {
        public int MovieId { get; set; }

        [Required]
        [StringLength(255)]
        public string Title { get; set; }

        public string? Description { get; set; }

        [Display(Name = "Release Year")]
        public int? ReleaseYear { get; set; }

        [Display(Name = "Duration (Minutes)")]
        public int? DurationMinutes { get; set; }

        [StringLength(10)]
        public string? Rating { get; set; }

        [StringLength(100)]
        public string? Director { get; set; }

        [Display(Name = "Poster Image")]
        public IFormFile? PosterImage { get; set; }

        public string? ExistingPosterPath { get; set; }

        [Required]
        [Display(Name = "YouTube Video ID")]
        [StringLength(255)]
        public string VideoUrl { get; set; }

        [Required(ErrorMessage = "Pilih minimal satu genre.")]
        [Display(Name = "Genres")]
        public List<int> SelectedGenreIds { get; set; } = new List<int>();

        public MultiSelectList? Genres { get; set; }
    }
}