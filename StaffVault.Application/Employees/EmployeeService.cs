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
    private static readonly Error NotFoundError = new(string.Empty, "Employee not found.");

    private static readonly Error DuplicateEmailError = new(
        nameof(SaveEmployeeRequest.Email),
        "An employee with this email already exists."
    );

    public async Task<IReadOnlyList<EmployeeDto>> GetAllAsync(
        CancellationToken cancellationToken = default
    )
    {
        var employees = await repository.GetAllAsync(cancellationToken);
        return employees.Select(ToDto).ToList();
    }

    public async Task<EmployeeDto?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default
    )
    {
        var employee = await repository.GetByIdAsync(id, cancellationToken);
        return employee is null ? null : ToDto(employee);
    }

    public async Task<Result> CreateAsync(
        SaveEmployeeRequest request,
        CancellationToken cancellationToken = default
    )
    {
        var errors = validator.Validate(request);
        if (errors.Count > 0)
        {
            return Result.Failure(errors);
        }

        var picturePath = await SavePictureAsync(request.Picture, cancellationToken);
        var employee = BuildEmployee(id: 0, request, picturePath);

        try
        {
            var newId = await repository.AddAsync(employee, cancellationToken);
            logger.LogInformation("Employee {EmployeeId} created", newId);
            return Result.Success();
        }
        catch (DuplicateEmailException)
        {
            fileStorage.DeleteFile(picturePath);
            logger.LogWarning("Create rejected: duplicate email");
            return Result.Failure(DuplicateEmailError);
        }
        catch
        {
            fileStorage.DeleteFile(picturePath);
            throw;
        }
    }

    public async Task<Result> UpdateAsync(
        int id,
        SaveEmployeeRequest request,
        CancellationToken cancellationToken = default
    )
    {
        var existing = await repository.GetByIdAsync(id, cancellationToken);
        if (existing is null)
        {
            return Result.Failure(NotFoundError);
        }

        var errors = validator.Validate(request);
        if (errors.Count > 0)
        {
            return Result.Failure(errors);
        }

        var newPicturePath = await SavePictureAsync(request.Picture, cancellationToken);
        var updated = BuildEmployee(id, request, newPicturePath ?? existing.PicturePath);

        try
        {
            if (!await repository.UpdateAsync(updated, cancellationToken))
            {
                fileStorage.DeleteFile(newPicturePath);
                return Result.Failure(NotFoundError);
            }
        }
        catch (DuplicateEmailException)
        {
            fileStorage.DeleteFile(newPicturePath);
            logger.LogWarning("Update of employee {EmployeeId} rejected: duplicate email", id);
            return Result.Failure(DuplicateEmailError);
        }
        catch
        {
            fileStorage.DeleteFile(newPicturePath);
            throw;
        }

        if (newPicturePath is not null)
        {
            fileStorage.DeleteFile(existing.PicturePath);
        }

        logger.LogInformation("Employee {EmployeeId} updated", id);
        return Result.Success();
    }

    public async Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var existing = await repository.GetByIdAsync(id, cancellationToken);
        if (existing is null || !await repository.DeleteAsync(id, cancellationToken))
        {
            return Result.Failure(NotFoundError);
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
        SaveEmployeeRequest request,
        string? picturePath
    ) =>
        new()
        {
            Id = id,
            Name = request.Name.Trim(),
            DateOfBirth = request.DateOfBirth.Date,
            Email = request.Email.Trim().ToLowerInvariant(),
            PicturePath = picturePath,
        };

    private static EmployeeDto ToDto(Employee employee) =>
        new(
            employee.Id,
            employee.Name,
            employee.DateOfBirth,
            employee.Email,
            employee.PicturePath,
            employee.UpdatedOnUtc
        );
}
