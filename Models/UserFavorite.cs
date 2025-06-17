using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tomomo7.Models
{
    [Table("UserFavorites")]
    [PrimaryKey(nameof(UserId), nameof(MovieId))]
    public partial class UserFavorite
    {
        [Column("user_id")]
        public int UserId { get; set; }

        [Column("movie_id")]
        public int MovieId { get; set; }

        [Column("added_at")]
        public DateTime AddedAt { get; set; }

        [ForeignKey(nameof(MovieId))]
        public virtual Movie Movie { get; set; } = null!;

        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; } = null!;
    }
}