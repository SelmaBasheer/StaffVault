using System.Text.RegularExpressions;
using StaffVault.Application.Common;
using StaffVault.Domain.Rules;

namespace StaffVault.Application.Employees
{
    internal sealed partial class EmployeeRequestValidator(TimeProvider timeProvider)
    {
        public IReadOnlyList<Error> Validate(SaveEmployeeRequest request)
        {
            var errors = new List<Error>();
            ValidateName(request.Name, errors);
            ValidateDateOfBirth(request.DateOfBirth, errors);
            ValidateEmail(request.Email, errors);
            ValidatePicture(request.Picture, errors);
            return errors;
        }

        private static void ValidateName(string? name, List<Error> errors)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                errors.Add(new Error(nameof(SaveEmployeeRequest.Name), "Name is required."));
            }
            else if (name.Trim().Length > EmployeeRules.NameMaxLength)
            {
                errors.Add(
                    new Error(
                        nameof(SaveEmployeeRequest.Name),
                        $"Name cannot exceed {EmployeeRules.NameMaxLength} characters."
                    )
                );
            }
        }

        private void ValidateDateOfBirth(DateTime dateOfBirth, List<Error> errors)
        {
            var today = DateOnly.FromDateTime(timeProvider.GetLocalNow().DateTime);

            if (DateOnly.FromDateTime(dateOfBirth) >= today)
            {
                errors.Add(
                    new Error(
                        nameof(SaveEmployeeRequest.DateOfBirth),
                        "Date of Birth must be earlier than today."
                    )
                );
            }
        }

        private static void ValidateEmail(string? email, List<Error> errors)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                errors.Add(new Error(nameof(SaveEmployeeRequest.Email), "Email is required."));
            }
            else if (email.Trim().Length > EmployeeRules.EmailMaxLength)
            {
                errors.Add(
                    new Error(
                        nameof(SaveEmployeeRequest.Email),
                        $"Email cannot exceed {EmployeeRules.EmailMaxLength} characters."
                    )
                );
            }
            else if (!EmailPattern().IsMatch(email.Trim()))
            {
                errors.Add(
                    new Error(
                        nameof(SaveEmployeeRequest.Email),
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
                        nameof(SaveEmployeeRequest.Picture),
                        $"Only {EmployeeRules.AllowedPictureExtensionsDisplay} files are allowed."
                    )
                );
            }
            else if (picture.Length == 0)
            {
                errors.Add(
                    new Error(nameof(SaveEmployeeRequest.Picture), "The selected picture is empty.")
                );
            }
            else if (picture.Length > EmployeeRules.MaxPictureSizeBytes)
            {
                errors.Add(
                    new Error(
                        nameof(SaveEmployeeRequest.Picture),
                        "Picture must be 2 MB or smaller."
                    )
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
}
