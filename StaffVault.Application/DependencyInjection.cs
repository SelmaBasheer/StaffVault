using Microsoft.Extensions.DependencyInjection;
using StaffVault.Application.Abstractions;
using StaffVault.Application.Employees;

namespace StaffVault.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddSingleton(TimeProvider.System);
            services.AddSingleton<EmployeeRequestValidator>();
            services.AddScoped<IEmployeeService, EmployeeService>();

            return services;
        }
    }
}
