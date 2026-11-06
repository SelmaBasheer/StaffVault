using Microsoft.Data.SqlClient;

namespace StaffVault.Infrastructure.Persistence
{
    internal interface ISqlConnectionFactory
    {
        SqlConnection CreateConnection();
    }

    internal class SqlConnectionFactory(string connectionString) : ISqlConnectionFactory
    {
        public SqlConnection CreateConnection()
        {
            return new SqlConnection(connectionString);
        }
    }
}
