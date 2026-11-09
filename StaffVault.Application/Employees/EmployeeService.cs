using Microsoft.Extensions.Logging;
using StaffVault.Application.Abstractions;
using StaffVault.Application.Common;
using StaffVault.Domain.Entities;
using StaffVault.Domain.Exceptions;

namespace StaffVault.Application.Employees;

internal sealed class EmployeeService(
    IEmployeeRepository repository,
    IFileStorageService fileStorage,
    EmployeeRequestValidator validator,
    ILogger<EmployeeService> logger
) : IEmployeeService
{
    private static readonly Error NotFound = new(string.Empty, "Employee not found.");

    private static readonly Error DuplicateEmail = new(
        nameof(CreateEmployeeRequest.Email),
        "An employee with this email already exists."
    );

    //Read Employees
    public async Task<IReadOnlyList<EmployeeDto>> GetAllAsync(
        CancellationToken cancellationToken = default
    )
    {
        var employees = await repository.GetAllAsync(cancellationToken);
        return employees.Select(ToDto).ToList();
    }

    // Read Employee by Id
    public async Task<EmployeeDto?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default
    )
    {
        var employee = await repository.GetByIdAsync(id, cancellationToken);
        return employee is null ? null : ToDto(employee);
    }

    //Create Employee
    public async Task<Result> CreateAsync(
        CreateEmployeeRequest request,
        CancellationToken cancellationToken = default
    )
    {
        var errors = validator.Validate(request);
        if (errors.Count > 0)
        {
            return Result.Failure(errors);
        }

        var picturePath = await SavePictureAsync(request.Picture, cancellationToken);
        var employee = BuildEmployee(
            id: 0,
            request.Name,
            request.DateOfBirth,
            request.Email,
            picturePath
        );

        try
        {
            var newId = await repository.CreateAsync(employee, cancellationToken);
            logger.LogInformation("Employee {EmployeeId} created", newId);
            return Result.Success();
        }
        catch (DuplicateEmailException)
        {
            fileStorage.DeleteFile(picturePath);
            logger.LogWarning("Create rejected: duplicate email");
            return Result.Failure(DuplicateEmail);
        }
        catch
        {
            fileStorage.DeleteFile(picturePath);
            throw; // unexpected error: clean up, then let it reach the error page/log
        }
    }

    // Update Employee
    public async Task<Result> UpdateAsync(
        UpdateEmployeeRequest request,
        CancellationToken cancellationToken = default
    )
    {
        var errors = validator.Validate(request);
        if (errors.Count > 0)
        {
            return Result.Failure(errors);
        }
        var existing = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (existing is null)
        {
            return Result.Failure(NotFound);
        }
        var newPicturePath = await SavePictureAsync(request.Picture, cancellationToken);
        var picturePath = newPicturePath ?? existing.PicturePath; // no new picture -> keep the old one
        var employee = BuildEmployee(
            request.Id,
            request.Name,
            request.DateOfBirth,
            request.Email,
            picturePath
        );

        try
        {
            if (!await repository.UpdateAsync(employee, cancellationToken))
            {
                fileStorage.DeleteFile(newPicturePath);
                return Result.Failure(NotFound);
            }
        }
        catch (DuplicateEmailException)
        {
            fileStorage.DeleteFile(newPicturePath);
            logger.LogWarning(
                "Update of employee {EmployeeId} rejected: duplicate email",
                request.Id
            );
            return Result.Failure(DuplicateEmail);
        }
        catch
        {
            fileStorage.DeleteFile(newPicturePath);
            throw;
        }

        // Saved successfully: if the picture was replaced, remove the old file.
        if (newPicturePath is not null)
        {
            fileStorage.DeleteFile(existing.PicturePath);
        }

        logger.LogInformation("Employee {EmployeeId} updated", request.Id);
        return Result.Success();
    }

    // Delete Employee
    public async Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var existing = await repository.GetByIdAsync(id, cancellationToken);
        if (existing is null || !await repository.DeleteAsync(id, cancellationToken))
        {
            return Result.Failure(NotFound);
        }

        fileStorage.DeleteFile(existing.PicturePath);

        logger.LogInformation("Employee {EmployeeId} deleted", id);
        return Result.Success();
    }

    private async Task<string?> SavePictureAsync(
        FileUpload? picture,
        CancellationToken cancellationToken
    ) =>
        picture is null
            ? null
            : await fileStorage.SaveAsync(picture.Content, picture.FileName, cancellationToken);

    private static Employee BuildEmployee(
        int id,
        string name,
        DateOnly dateOfBirth,
        string email,
        string? picturePath
    ) =>
        new()
        {
            Id = id,
            Name = name.Trim(),
            DateOfBirth = dateOfBirth,
            Email = email.Trim().ToLowerInvariant(),
            PicturePath = picturePath,
        };

    private static EmployeeDto ToDto(Employee employee) =>
        new(
            employee.Id,
            employee.Name,
            employee.DateOfBirth,
            employee.Email,
            employee.PicturePath,
            employee.CreatedOnUtc,
            employee.UpdatedOnUtc
        );
}
