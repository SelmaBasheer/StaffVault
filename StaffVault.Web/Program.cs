using Serilog;
using StaffVault.Application;
using StaffVault.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Logging: Serilog, configured in appsettings.json
builder.Services.AddSerilog(config => config.ReadFrom.Configuration(builder.Configuration));

// MVC +  layers
builder.Services.AddControllersWithViews();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration, builder.Environment.WebRootPath);

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/error");
    app.UseHsts();
}

app.UseSerilogRequestLogging();
app.UseHttpsRedirection();
app.UseStaticFiles(); // serves wwwroot, including uploaded pictures

app.MapControllerRoute(name: "default", pattern: "{controller=Employee}/{action=Index}/{id?}");

app.Run();
