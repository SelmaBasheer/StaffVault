using StaffVault.Application.Common;

namespace StaffVault.Application.Employees
{
    public interface IEmployeeService
    {
        Task<IReadOnlyList<EmployeeDto>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<EmployeeDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<Result> CreateAsync(
            SaveEmployeeRequest request,
            CancellationToken cancellationToken = default
        );
        Task<Result> UpdateAsync(
            int id,
            SaveEmployeeRequest request,
            CancellationToken cancellationToken = default
        );
        Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default);
    }
}
