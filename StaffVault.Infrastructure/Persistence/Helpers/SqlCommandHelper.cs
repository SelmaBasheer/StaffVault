using System.Data;
using Microsoft.Data.SqlClient;

namespace StaffVault.Infrastructure.Persistence
{
    internal static class SqlCommandHelper
    {
        public static SqlCommand CreateStoredProcedureCommand(
            SqlConnection connection,
            string procedureName
        )
        {
            ArgumentNullException.ThrowIfNull(connection);
            ArgumentException.ThrowIfNullOrWhiteSpace(procedureName);

            return new SqlCommand(procedureName, connection)
            {
                CommandType = CommandType.StoredProcedure,
            };
        }
    }
}
