using System.Data;
using Microsoft.Data.SqlClient;
using StaffVault.Application.Abstractions;
using StaffVault.Domain.Entities;
using StaffVault.Domain.Exceptions;

namespace StaffVault.Infrastructure.Persistence;

// Calls the Employee stored procedures using ADO.NET
internal sealed class EmployeeRepository(ISqlConnectionFactory connectionFactory)
    : IEmployeeRepository
{
    public async Task<IReadOnlyList<Employee>> GetAllAsync(
        CancellationToken cancellationToken = default
    )
    {
        await using var connection = connectionFactory.CreateConnection();
        await using var command = CreateCommand(connection, "dbo.usp_Employee_GetAll");

        await connection.OpenAsync(cancellationToken);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        var employees = new List<Employee>();
        while (await reader.ReadAsync(cancellationToken))
        {
            employees.Add(Map(reader));
        }

        return employees;
    }

    public async Task<Employee?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        await using var connection = connectionFactory.CreateConnection();
        await using var command = CreateCommand(connection, "dbo.usp_Employee_GetById");
        command.Parameters.Add("@Id", SqlDbType.Int).Value = id;

        await connection.OpenAsync(cancellationToken);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        if (await reader.ReadAsync(cancellationToken))
        {
            return Map(reader);
        }

        return null;
    }

    public async Task<int> AddAsync(
        Employee employee,
        CancellationToken cancellationToken = default
    )
    {
        await using var connection = connectionFactory.CreateConnection();
        await using var command = CreateCommand(connection, "dbo.usp_Employee_Insert");
        AddEmployeeParameters(command, employee);

        var idParameter = command.Parameters.Add("@Id", SqlDbType.Int);
        idParameter.Direction = ParameterDirection.Output;

        await connection.OpenAsync(cancellationToken);

        try
        {
            await command.ExecuteNonQueryAsync(cancellationToken);
        }
        catch (SqlException ex) when (IsDuplicateKey(ex))
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
        await using var command = CreateCommand(connection, "dbo.usp_Employee_Update");
        command.Parameters.Add("@Id", SqlDbType.Int).Value = employee.Id;
        AddEmployeeParameters(command, employee);

        await connection.OpenAsync(cancellationToken);

        try
        {
            var rowsAffected = await command.ExecuteScalarAsync(cancellationToken);
            return Convert.ToInt32(rowsAffected) > 0;
        }
        catch (SqlException ex) when (IsDuplicateKey(ex))
        {
            throw new DuplicateEmailException(employee.Email, ex);
        }
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        await using var connection = connectionFactory.CreateConnection();
        await using var command = CreateCommand(connection, "dbo.usp_Employee_Delete");
        command.Parameters.Add("@Id", SqlDbType.Int).Value = id;

        await connection.OpenAsync(cancellationToken);
        var rowsAffected = await command.ExecuteScalarAsync(cancellationToken);

        return Convert.ToInt32(rowsAffected) > 0;
    }

    private static SqlCommand CreateCommand(SqlConnection connection, string procedureName)
    {
        return new SqlCommand(procedureName, connection)
        {
            CommandType = CommandType.StoredProcedure,
        };
    }

    private static void AddEmployeeParameters(SqlCommand command, Employee employee)
    {
        command.Parameters.Add("@Name", SqlDbType.NVarChar, 100).Value = employee.Name;
        command.Parameters.Add("@DateOfBirth", SqlDbType.Date).Value = employee.DateOfBirth.Date;
        command.Parameters.Add("@Email", SqlDbType.NVarChar, 254).Value = employee.Email;
        command.Parameters.Add("@Picture", SqlDbType.NVarChar, 260).Value =
            (object?)employee.PicturePath ?? DBNull.Value;
    }

    // 2627 = primary key violation, 2601 = unique index violation -> duplicate Email
    private static bool IsDuplicateKey(SqlException ex) => ex.Number == 2627 || ex.Number == 2601;

    private static Employee Map(SqlDataReader reader)
    {
        return new Employee
        {
            Id = (int)reader["ID"],
            Name = (string)reader["Name"],
            DateOfBirth = (DateTime)reader["DateOfBirth"],
            Email = (string)reader["Email"],
            PicturePath = reader["Picture"] as string,
            UpdatedOnUtc = DateTime.SpecifyKind((DateTime)reader["UpdatedOn"], DateTimeKind.Utc),
        };
    }
}
