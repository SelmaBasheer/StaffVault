using StaffVault.Domain.Entities;

namespace StaffVault.Application.Abstractions
{
    public interface IEmployeeRepository
    {
        Task<IReadOnlyList<Employee>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<Employee?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<int> CreateAsync(Employee employee, CancellationToken cancellationToken = default);
        Task<bool> UpdateAsync(Employee employee, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
    }
}
