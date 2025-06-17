using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Tomomo7.Models;

[Table("Users")]
[Index("Email", Name = "UQ__Users__A9D10534...", IsUnique = true)] 
[Index("Username", Name = "UQ__Users__B15BE12E...", IsUnique = true)] 
public partial class User
{
    [Key]
    [Column("user_id")]
    public int UserId { get; set; }

    [Required(ErrorMessage = "Username wajib diisi.")] 
    [StringLength(50)]
    [Column("username")]
    public string Username { get; set; } = null!;

    [Required(ErrorMessage = "Email wajib diisi.")] 
    [EmailAddress(ErrorMessage = "Format email tidak valid.")] 
    [StringLength(100)]
    [Column("email")]
    public string Email { get; set; } = null!;

    [Required] 
    [StringLength(255)]
    [Column("password_hash")]
    public string PasswordHash { get; set; } = null!;

    [Required]
    [StringLength(20)]
    [Column("role")]
    public string Role { get; set; } = null!;

    [StringLength(20)]
    [Column("phone_number")]
    public string? PhoneNumber { get; set; }

    [StringLength(255)]
    [Column("address")]
    public string? Address { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }

    [Column("last_login_at")]
    public DateTime? LastLoginAt { get; set; }

    public virtual ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
    public virtual ICollection<UserFavorite> UserFavorites { get; set; } = new List<UserFavorite>();
}