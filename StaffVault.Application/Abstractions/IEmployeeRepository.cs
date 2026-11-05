using StaffVault.Domain.Entities;

namespace StaffVault.Application.Abstractions
{
    public interface IEmployeeRepository
    {
        Task<IReadOnlyList<Employee>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<Employee?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<int> AddAsync(Employee employee, CancellationToken cancellationToken = default);
        Task<bool> UpdateAsync(Employee employee, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
