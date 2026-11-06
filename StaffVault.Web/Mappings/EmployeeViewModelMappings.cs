using StaffVault.Application.Employees;
using StaffVault.Web.Models;

namespace StaffVault.Web.Mappings;

/// Converts between Application DTOs and Web view models.
internal static class EmployeeViewModelMappings
{
    public static EmployeeListItemViewModel ToListItem(this EmployeeDto dto) =>
        new(dto.Id, dto.Name, dto.DateOfBirth, dto.Email, dto.PicturePath, dto.UpdatedOnUtc);

    public static EmployeeFormViewModel ToForm(this EmployeeDto dto) =>
        new()
        {
            Id = dto.Id,
            Name = dto.Name,
            DateOfBirth = dto.DateOfBirth,
            Email = dto.Email,
            ExistingPicturePath = dto.PicturePath,
        };

    public static SaveEmployeeRequest ToRequest(
        this EmployeeFormViewModel form,
        FileUpload? picture
    ) => new(form.Name, form.DateOfBirth!.Value, form.Email, picture);
}
