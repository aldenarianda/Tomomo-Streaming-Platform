using System;
using System.Collections.Generic;

namespace Tomomo7.Models;

public partial class Movie
{
    public int MovieId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public int? ReleaseYear { get; set; }

    public int? DurationMinutes { get; set; }

    public string? Rating { get; set; }

    public string? Director { get; set; }

    public string? PosterPath { get; set; }

    public string? VideoUrl { get; set; }

    public DateTime AddedAt { get; set; }

    public virtual ICollection<Genre> Genres { get; set; } = new List<Genre>();
    public virtual ICollection<UserFavorite> UserFavorites { get; set; } = new List<UserFavorite>();
}
