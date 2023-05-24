using System.ComponentModel.DataAnnotations;
using MyPrescription.Common.Models;

namespace MyPrescription.Common.Attributes;

public class RequiredWhenPatient : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var newUser = (RegisterUser)validationContext.ObjectInstance;
        if (newUser.Role != "patient")
            return ValidationResult.Success;

        var doctorId = value as string;
        if (string.IsNullOrWhiteSpace(doctorId))
        {
            return new ValidationResult("Select a doctor.", new List<string>() { validationContext.MemberName });
        }
        return ValidationResult.Success;
    }
}
