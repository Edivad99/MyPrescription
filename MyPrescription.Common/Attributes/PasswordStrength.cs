using System.ComponentModel.DataAnnotations;

namespace MyPrescription.Common.Attributes;

public class PasswordStrength : ValidationAttribute
{
    private readonly int level;
    public PasswordStrength(int level) => this.level = level;
    private string GetErrorMessage(int passwordLevel) =>
        $"The password is too weak. The current password strength is {passwordLevel}, while it should be at least {level}";

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var input = (string)value;

        var result = Zxcvbn.Core.EvaluatePassword(input);
        int score = result.Score;
        if (score >= level)
        {
            return ValidationResult.Success;
        }
        return new ValidationResult(GetErrorMessage(score), new List<string>() { validationContext.MemberName });
    }
}


