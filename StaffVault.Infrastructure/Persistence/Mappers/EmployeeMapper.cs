using System.Data;
using Microsoft.Data.SqlClient;
using StaffVault.Domain.Entities;

namespace StaffVault.Infrastructure.Persistence.Mappers
{
    ///Converts between SQL (rows / parameters) and the Employee entity.
    internal static class EmployeeMapper
    {
        ///One SQL row -> Employee. Column names must match the SELECT in the procedures
        public static Employee ToEmployee(this SqlDataReader reader) =>
            new()
            {
                Id = (int)reader["Id"],
                Name = (string)reader["Name"],
                DateOfBirth = DateOnly.FromDateTime((DateTime)reader["DateOfBirth"]),
                Email = (string)reader["Email"],
                PicturePath = reader["PicturePath"] as string,
                CreatedOnUtc = DateTime.SpecifyKind(
                    (DateTime)reader["CreatedOnUtc"],
                    DateTimeKind.Utc
                ),
                UpdatedOnUtc = DateTime.SpecifyKind(
                    (DateTime)reader["UpdatedOnUtc"],
                    DateTimeKind.Utc
                ),
            };

        /// Employee -> parameters shared by Insert and Update. Names must match the procedures.
        public static void AddEmployeeParameters(this SqlCommand command, Employee employee)
        {
            command.Parameters.Add("@Name", SqlDbType.NVarChar, 100).Value = employee.Name;
            command.Parameters.Add("@DateOfBirth", SqlDbType.Date).Value = employee.DateOfBirth;
            command.Parameters.Add("@Email", SqlDbType.NVarChar, 254).Value = employee.Email;
            command.Parameters.Add("@PicturePath", SqlDbType.NVarChar, 300).Value =
                (object?)employee.PicturePath ?? DBNull.Value;
        }
    }
}
