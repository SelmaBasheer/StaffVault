using System.Data;
using Microsoft.Data.SqlClient;
using StaffVault.Application.Abstractions;
using StaffVault.Domain.Entities;
using StaffVault.Domain.Exceptions;
using StaffVault.Infrastructure.Persistence.Helpers;
using StaffVault.Infrastructure.Persistence.Mappers;

namespace StaffVault.Infrastructure.Persistence;

internal sealed class EmployeeRepository(ISqlConnectionFactory connectionFactory)
    : IEmployeeRepository
{
    public async Task<IReadOnlyList<Employee>> GetAllAsync(
        CancellationToken cancellationToken = default
    )
    {
        await using var connection = connectionFactory.CreateConnection();
        await using var command = SqlCommandHelper.CreateStoredProcedureCommand(
            connection,
            StoredProcedures.Employee.GetAll
        );

        await connection.OpenAsync(cancellationToken);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        var employees = new List<Employee>();
        while (await reader.ReadAsync(cancellationToken))
        {
            employees.Add(reader.ToEmployee());
        }

        return employees;
    }

    public async Task<Employee?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        await using var connection = connectionFactory.CreateConnection();
        await using var command = SqlCommandHelper.CreateStoredProcedureCommand(
            connection,
            StoredProcedures.Employee.GetById
        );
        command.Parameters.Add("@Id", SqlDbType.Int).Value = id;

        await connection.OpenAsync(cancellationToken);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        return await reader.ReadAsync(cancellationToken) ? reader.ToEmployee() : null;
    }

    public async Task<int> CreateAsync(
        Employee employee,
        CancellationToken cancellationToken = default
    )
    {
        await using var connection = connectionFactory.CreateConnection();
        await using var command = SqlCommandHelper.CreateStoredProcedureCommand(
            connection,
            StoredProcedures.Employee.Insert
        );
        command.AddEmployeeParameters(employee);

        var idParameter = command.Parameters.Add("@Id", SqlDbType.Int);
        idParameter.Direction = ParameterDirection.Output;

        await connection.OpenAsync(cancellationToken);

        try
        {
            await command.ExecuteNonQueryAsync(cancellationToken);
        }
        catch (SqlException ex) when (SqlErrorHelper.IsDuplicateKey(ex))
        {
            throw new DuplicateEmailException(employee.Email, ex);
        }

        return (int)idParameter.Value;
    }

    public async Task<bool> UpdateAsync(
        Employee employee,
        CancellationToken cancellationToken = default
    )
    {
        await using var connection = connectionFactory.CreateConnection();
        await using var command = SqlCommandHelper.CreateStoredProcedureCommand(
            connection,
            StoredProcedures.Employee.Update
        );
        command.Parameters.Add("@Id", SqlDbType.Int).Value = employee.Id;
        command.AddEmployeeParameters(employee);

        await connection.OpenAsync(cancellationToken);

        try
        {
            var rowsAffected = await command.ExecuteScalarAsync(cancellationToken);
            return Convert.ToInt32(rowsAffected) > 0;
        }
        catch (SqlException ex) when (SqlErrorHelper.IsDuplicateKey(ex))
        {
            throw new DuplicateEmailException(employee.Email, ex);
        }
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        await using var connection = connectionFactory.CreateConnection();
        await using var command = SqlCommandHelper.CreateStoredProcedureCommand(
            connection,
            StoredProcedures.Employee.Delete
        );
        command.Parameters.Add("@Id", SqlDbType.Int).Value = id;

        await connection.OpenAsync(cancellationToken);
        var rowsAffected = await command.ExecuteScalarAsync(cancellationToken);

        return Convert.ToInt32(rowsAffected) > 0;
    }
}
