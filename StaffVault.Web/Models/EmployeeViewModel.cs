using System.ComponentModel.DataAnnotations;
using StaffVault.Application.Employees;

namespace StaffVault.Web.Models;

public sealed class EmployeeViewModel
{
    // ----- Form fields (Id = 0 means Add, Id > 0 means Edit) -----
    public int Id { get; set; }

    [Required(ErrorMessage = "Name is required.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Date of Birth is required.")]
    [DataType(DataType.Date)]
    [Display(Name = "Date of Birth")]
    public DateTime? DateOfBirth { get; set; }

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
    public string Email { get; set; } = string.Empty;

    [Display(Name = "Upload Picture")]
    public IFormFile? Picture { get; set; }

    public string? ExistingPicturePath { get; set; }

    // ----- Grid -----
    public IReadOnlyList<EmployeeDto> Employees { get; set; } = [];
}
