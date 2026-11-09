using System.Text.RegularExpressions;
using StaffVault.Application.Common;
using StaffVault.Domain.Rules;

namespace StaffVault.Application.Employees;

// Server-side validation. JavaScript can be bypassed, so the server checks every rule again.

internal sealed partial class EmployeeRequestValidator
{
    public IReadOnlyList<Error> Validate(CreateEmployeeRequest request) =>
        Validate(request.Name, request.DateOfBirth, request.Email, request.Picture);

    public IReadOnlyList<Error> Validate(UpdateEmployeeRequest request) =>
        Validate(request.Name, request.DateOfBirth, request.Email, request.Picture);

    private static List<Error> Validate(
        string name,
        DateOnly dateOfBirth,
        string email,
        FileUpload? picture
    )
    {
        var errors = new List<Error>();
        ValidateName(name, errors);
        ValidateDateOfBirth(dateOfBirth, errors);
        ValidateEmail(email, errors);
        ValidatePicture(picture, errors);
        return errors;
    }

    private static void ValidateName(string? name, List<Error> errors)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            errors.Add(new Error(nameof(CreateEmployeeRequest.Name), "Name is required."));
        }
        else if (name.Trim().Length > EmployeeRules.NameMaxLength)
        {
            errors.Add(
                new Error(
                    nameof(CreateEmployeeRequest.Name),
                    $"Name cannot exceed {EmployeeRules.NameMaxLength} characters."
                )
            );
        }
    }

    private static void ValidateDateOfBirth(DateOnly dateOfBirth, List<Error> errors)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);

        if (dateOfBirth >= today)
        {
            errors.Add(
                new Error(
                    nameof(CreateEmployeeRequest.DateOfBirth),
                    "Date of Birth must be earlier than today."
                )
            );
        }
    }

    private static void ValidateEmail(string? email, List<Error> errors)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            errors.Add(new Error(nameof(CreateEmployeeRequest.Email), "Email is required."));
        }
        else if (email.Trim().Length > EmployeeRules.EmailMaxLength)
        {
            errors.Add(
                new Error(
                    nameof(CreateEmployeeRequest.Email),
                    $"Email cannot exceed {EmployeeRules.EmailMaxLength} characters."
                )
            );
        }
        else if (!EmailPattern().IsMatch(email.Trim()))
        {
            errors.Add(
                new Error(
                    nameof(CreateEmployeeRequest.Email),
                    "Please enter a valid email address."
                )
            );
        }
    }

    private static void ValidatePicture(FileUpload? picture, List<Error> errors)
    {
        if (picture is null)
        {
            return; // Picture is optional.
        }

        var extension = Path.GetExtension(picture.FileName);

        if (!EmployeeRules.AllowedPictureExtensions.Contains(extension))
        {
            errors.Add(
                new Error(
                    nameof(CreateEmployeeRequest.Picture),
                    $"Only {EmployeeRules.AllowedPictureExtensionsDisplay} files are allowed."
                )
            );
        }
        else if (picture.Length > EmployeeRules.MaxPictureSizeBytes)
        {
            errors.Add(
                new Error(nameof(CreateEmployeeRequest.Picture), "Picture must be 2 MB or smaller.")
            );
        }
    }

    [GeneratedRegex(
        @"^[^\s@]+@[^\s@]+\.[^\s@]{2,}$",
        RegexOptions.CultureInvariant,
        matchTimeoutMilliseconds: 250
    )]
    private static partial Regex EmailPattern();
}
