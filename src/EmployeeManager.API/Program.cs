using System.Text;
using EmployeeManager.Services.context;
using EmployeeManager.Services.Helpers.Middleware;
using EmployeeManager.Services.Helpers.Options;
using EmployeeManager.Services.Services.Accounts;
using EmployeeManager.Services.Services.Devices;
using EmployeeManager.Services.Services.Employees;
using EmployeeManager.Services.Services.Positions;
using EmployeeManager.Services.Services.Roles;
using EmployeeManager.Services.services.Tokens;
using EmployeeManager.Services.Services.Validation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var jwtConfigData = builder.Configuration.GetSection("Jwt");

var connectionString = builder.Configuration.GetConnectionString("EmployeeDatabase");
builder.Services.AddDbContext<EmployeeDatabaseContext>(options => options.UseSqlServer(connectionString));

builder.Services.Configure<JwtOptions>(jwtConfigData);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(o =>
{
    o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidIssuer = jwtConfigData["Issuer"],
        ValidAudience = jwtConfigData["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfigData["Key"])),
        ClockSkew = TimeSpan.FromMinutes(10)
    };
});

builder.Services.AddTransient<IDeviceService, DeviceService>();
builder.Services.AddTransient<IEmployeeService, EmployeeService>();
builder.Services.AddTransient<IAccountService, AccountService>();
builder.Services.AddTransient<ITokenService, TokenService>();
builder.Services.AddTransient<IPositionService, PositionService>();
builder.Services.AddTransient<IRoleService, RoleService>();
builder.Services.AddTransient<IValidationService, ValidationService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseMiddleware<ValidationMiddleware>();

app.Run();