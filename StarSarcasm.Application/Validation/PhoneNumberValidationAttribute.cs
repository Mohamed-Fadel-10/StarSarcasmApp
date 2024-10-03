using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

public class PhoneNumberValidationAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var phoneNumber = value as string;

        if (string.IsNullOrEmpty(phoneNumber))
        {
            return new ValidationResult("Phone number is required.");
        }

        string saudiPattern = @"^(\+966|05)[0-9]{8}$";
        string egyptPattern = @"^\+201[0-9]{9}$";

        bool isValid = false;
        string errorMessage = "Phone number is invalid."; 

        if (Regex.IsMatch(phoneNumber, saudiPattern))
        {
            isValid = true;
        }
        else if (Regex.IsMatch(phoneNumber, egyptPattern))
        {
            isValid = true;
        }

        if (!isValid)
        {
            return new ValidationResult("Please enter a valid phone number with a valid Saudi or Egyptian code.");
        }

        return ValidationResult.Success;
    }
}
