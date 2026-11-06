namespace StaffVault.Application.Employees
{
    public sealed record EmployeeDto(
        int Id,
        string Name,
        DateTime DateOfBirth,
        string Email,
        string? PicturePath,
        DateTime UpdatedOnUtc
    );

    public sealed record FileUpload(Stream Content, string FileName, long Length);

    public sealed record SaveEmployeeRequest(
        string Name,
        DateTime DateOfBirth,
        string Email,
        FileUpload? Picture
    );
}
