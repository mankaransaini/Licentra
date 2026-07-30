using Licentra.API.Data;
using Licentra.API.Interfaces.Departments;
using Licentra.API.Repositories.Departments;
using Licentra.API.Services.Departments;
using Microsoft.EntityFrameworkCore;
using Licentra.API.Middleware;
using Licentra.API.Interfaces.Employees;
using Licentra.API.Repositories.Employees;
using Licentra.API.Interfaces.Employees;
using Licentra.API.Services.Employees;
using Licentra.API.Interfaces.Roles;
using Licentra.API.Repositories.Roles;
using Licentra.API.Services.Roles;
using Licentra.API.Interfaces.Users;
using Licentra.API.Repositories.Users;
using Licentra.API.Services.Users;
using Licentra.API.Interfaces.Vendors;
using Licentra.API.Repositories.Vendors;
using Licentra.API.Services.Vendors;
using Licentra.API.Interfaces.Software;
using Licentra.API.Repositories.Software;
using Licentra.API.Services.Software;
using Licentra.API.Interfaces.Licenses;
using Licentra.API.Repositories.Licenses;
using Licentra.API.Services.Licenses;
using Licentra.API.Interfaces.LicenseAssignments;
using Licentra.API.Repositories.LicenseAssignments;
using Licentra.API.Services.LicenseAssignments;
using Licentra.API.Interfaces.AuditLogs;
using Licentra.API.Repositories.AuditLogs;
using Licentra.API.Interfaces.AuditLogs;
using Licentra.API.Services.AuditLogs;
using Licentra.API.Interfaces.Security;
using Licentra.API.Services.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Licentra.API.Interfaces.Security;
using Licentra.API.Services.Security;
using Licentra.API.Interfaces.Auth;
using Licentra.API.Services.Auth;
using Microsoft.OpenApi.Models;

namespace Licentra.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            


            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };

        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine("JWT ERROR: " + context.Exception.Message);
                return Task.CompletedTask;
            }
        };
    });

            builder.Services.AddAuthorization();

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter JWT Token like: Bearer {your token}"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
            });
            builder.Services.AddDbContext<LicentraDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("LicentraConnection")));
            builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
            builder.Services.AddScoped<IDepartmentService, DepartmentService>();
            builder.Services.AddScoped<IEmployeeService, EmployeeService>();
            builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            builder.Services.AddScoped<IRoleRepository, RoleRepository>();
            builder.Services.AddScoped<IRoleService, RoleService>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IVendorRepository, VendorRepository>();
            builder.Services.AddScoped<IVendorService, VendorService>();
            builder.Services.AddScoped<ISoftwareRepository, SoftwareRepository>();
            builder.Services.AddScoped<ISoftwareService, SoftwareService>();
            builder.Services.AddScoped<ILicenseRepository, LicenseRepository>();
            builder.Services.AddScoped<ILicenseService, LicenseService>();
            builder.Services.AddScoped<ILicenseAssignmentRepository, LicenseAssignmentRepository>();
            builder.Services.AddScoped<ILicenseAssignmentService, LicenseAssignmentService>();
            builder.Services.AddScoped<IAuditLogRepository, AuditLogRepository>();
            builder.Services.AddScoped<IAuditLogService, AuditLogService>();
            builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
            builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
            builder.Services.AddScoped<IAuthService, AuthService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

                app.UseHttpsRedirection();

                app.UseMiddleware<ExceptionMiddleware>();

                app.UseAuthentication();

                app.UseAuthorization();

                app.MapControllers();

                app.Run();
        }
    }
}
