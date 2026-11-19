namespace StaffVault.Infrastructure.Persistence
{
    internal static class StoredProcedures
    {
        internal static class Employee
        {
            public const string GetAll = "dbo.usp_Employee_GetAll";
            public const string GetById = "dbo.usp_Employee_GetById";
            public const string Insert = "dbo.usp_Employee_Insert";
            public const string Update = "dbo.usp_Employee_Update";
            public const string Delete = "dbo.usp_Employee_Delete";
        }
    }
}
