using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StaffVault.Application.Abstractions;
using StaffVault.Infrastructure.Persistence;
using StaffVault.Infrastructure.Storage;

namespace StaffVault.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        string webRootPath
    )
    {
        var connectionString =
            configuration.GetConnectionString("StaffVault")
            ?? throw new InvalidOperationException(
                "Connection string 'StaffVault' is missing in appsettings.json."
            );

        var uploadsFolderPath = Path.Combine(webRootPath, "uploads");

        services.AddSingleton<ISqlConnectionFactory>(new SqlConnectionFactory(connectionString));
        services.AddScoped<IEmployeeRepository, EmployeeRepository>();
        services.AddSingleton<IFileStorageService>(new LocalFileStorageService(uploadsFolderPath));

        return services;
    }
}
