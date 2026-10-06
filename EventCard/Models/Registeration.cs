using System.ComponentModel.DataAnnotations;

public class RegistrationModel
{
    [Required(ErrorMessage = "Name is required.")]
    [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
    public string Email { get; set; } = string.Empty;

    [Phone(ErrorMessage = "Please enter a valid phone number.")]
    public string? PhoneNumber { get; set; }

    [Required(ErrorMessage = "Event selection is required.")]
    public int EventId { get; set; }
}