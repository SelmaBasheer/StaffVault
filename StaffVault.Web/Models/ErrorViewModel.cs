using System.ComponentModel.DataAnnotations;

namespace StaffVault.Web.Models;

/// The whole page: form on top, grid below.
public sealed class EmployeeIndexViewModel
{
    public EmployeeFormViewModel Form { get; init; } = new();

    public IReadOnlyList<EmployeeListItemViewModel> Employees { get; init; } = [];
}

/// The Add/Edit form. Id = 0 means Add, Id > 0 means Edit.
public sealed class EmployeeFormViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Name is required.")]
    [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Date of Birth is required.")]
    [DataType(DataType.Date)]
    [Display(Name = "Date of Birth")]
    public DateTime? DateOfBirth { get; set; }

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
    [StringLength(254, ErrorMessage = "Email cannot exceed 254 characters.")]
    public string Email { get; set; } = string.Empty;

    [Display(Name = "Upload Picture")]
    public IFormFile? Picture { get; set; }

    /// Only for showing the current picture while editing.
    public string? ExistingPicturePath { get; set; }

    public bool IsEditMode => Id > 0;
}

/// One row in the grid.
public sealed record EmployeeListItemViewModel(
    int Id,
    string Name,
    DateTime DateOfBirth,
    string Email,
    string? PictureUrl,
    DateTime UpdatedOnUtc
);
