using System.ComponentModel.DataAnnotations;

namespace SafeVault.Web.Models;

public class User
{
    public int UserId { get; set; }

    [Required]
    [StringLength(100)]
    [RegularExpression(
        @"^[a-zA-Z0-9_ ]+$",
        ErrorMessage = "Username contains invalid characters.")]
    public string Username { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
}