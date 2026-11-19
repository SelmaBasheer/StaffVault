using Microsoft.Data.SqlClient;

namespace StaffVault.Infrastructure.Persistence.Helpers
{
    // Recognises SQL Server error numbers.
    internal static class SqlErrorHelper
    {
        private const int PrimaryKeyViolation = 2627;
        private const int UniqueIndexViolation = 2601;

        // True when an insert/update breaks a primary key or unique index (e.g. duplicate Email).
        public static bool IsDuplicateKey(SqlException exception) =>
            exception.Number is PrimaryKeyViolation or UniqueIndexViolation;
    }
}
