using System.ComponentModel.DataAnnotations;
using MyPrescription.Common.Attributes;

namespace MyPrescription.Common.Models;

public class RegisterUser
{
    [Required]
    public string FirstName { get; set; }
    [Required]
    public string LastName { get; set; }
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    [Required]
    [PasswordStrength(3)]
    public string Password { get; set; }
    [Required]
    [RegularExpression("Male|Female|Other")]
    public string Gender { get; set; }
    [Required]
    public DateOnly Birthdate { get; set; }
    [Required]
    public string Role { get; set; }
}

