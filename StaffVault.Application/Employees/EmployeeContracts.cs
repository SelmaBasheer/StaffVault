namespace StaffVault.Application.Employees
{
    public sealed record EmployeeDto(
        int Id,
        string Name,
        DateOnly DateOfBirth,
        string Email,
        string? PicturePath,
        DateTime CreatedOnUtc,
        DateTime UpdatedOnUtc
    );

    public sealed record CreateEmployeeRequest(
        string Name,
        DateOnly DateOfBirth,
        string Email,
        FileUpload? Picture
    );

    public sealed record UpdateEmployeeRequest(
        int Id,
        string Name,
        DateOnly DateOfBirth,
        string Email,
        FileUpload? Picture
    );

    public sealed record FileUpload(Stream Content, string FileName, long Length);
}
