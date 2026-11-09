using StaffVault.Application.Common;
using StaffVault.Application.Employees;

namespace StaffVault.Application.Abstractions
{
    public interface IEmployeeService
    {
        Task<IReadOnlyList<EmployeeDto>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<EmployeeDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<Result> CreateAsync(
            CreateEmployeeRequest request,
            CancellationToken cancellationToken = default
        );
        Task<Result> UpdateAsync(
            UpdateEmployeeRequest request,
            CancellationToken cancellationToken = default
        );
        Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default);
    }
}
