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

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                {
                    policy.WithOrigins("http://localhost:5173", "http://localhost:3000", "http://localhost:5174", "http://localhost:5000")
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                });
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
                    builder.Configuration.GetConnectionString("LicentraConnection"),
                    sqlOptions => sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(10),
                        errorNumbersToAdd: null)));
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
            builder.Services.AddHttpContextAccessor();


            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<LicentraDbContext>();
                try
                {
                    db.Database.ExecuteSqlRaw(@"
                        -- 1. Drop all check constraints on Licenses table
                        DECLARE @chkName nvarchar(200);
                        DECLARE chk_cursor CURSOR FOR SELECT name FROM sys.check_constraints WHERE parent_object_id = OBJECT_ID('Licenses');
                        OPEN chk_cursor;
                        FETCH NEXT FROM chk_cursor INTO @chkName;
                        WHILE @@FETCH_STATUS = 0
                        BEGIN
                            EXEC('ALTER TABLE [Licenses] DROP CONSTRAINT [' + @chkName + '];');
                            FETCH NEXT FROM chk_cursor INTO @chkName;
                        END
                        CLOSE chk_cursor;
                        DEALLOCATE chk_cursor;

                        -- 2. Drop all default constraints on Licenses table
                        DECLARE @defName nvarchar(200);
                        DECLARE def_cursor CURSOR FOR SELECT name FROM sys.default_constraints WHERE parent_object_id = OBJECT_ID('Licenses');
                        OPEN def_cursor;
                        FETCH NEXT FROM def_cursor INTO @defName;
                        WHILE @@FETCH_STATUS = 0
                        BEGIN
                            EXEC('ALTER TABLE [Licenses] DROP CONSTRAINT [' + @defName + '];');
                            FETCH NEXT FROM def_cursor INTO @defName;
                        END
                        CLOSE def_cursor;
                        DEALLOCATE def_cursor;

                        -- 3. Drop all triggers on Licenses table
                        DECLARE @trgName nvarchar(200);
                        DECLARE trg_cursor CURSOR FOR SELECT name FROM sys.triggers WHERE parent_id = OBJECT_ID('Licenses');
                        OPEN trg_cursor;
                        FETCH NEXT FROM trg_cursor INTO @trgName;
                        WHILE @@FETCH_STATUS = 0
                        BEGIN
                            EXEC('DROP TRIGGER [' + @trgName + '];');
                            FETCH NEXT FROM trg_cursor INTO @trgName;
                        END
                        CLOSE trg_cursor;
                        DEALLOCATE trg_cursor;

                        -- 4. Re-add clean tinyint default for LicenseStatus
                        ALTER TABLE [Licenses] ADD CONSTRAINT [DF_Licenses_LicenseStatus_Clean] DEFAULT ((1)) FOR [LicenseStatus];
                    ");
                    Console.WriteLine("[DB CLEANUP] Successfully dropped all invalid constraints and triggers on Licenses table.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[DB FIX NOTICE] {ex.Message}");
                }
            }

            // Configure the HTTP request pipeline.
            
                app.UseSwagger();
                app.UseSwaggerUI();
            

                app.UseHttpsRedirection();

                app.UseMiddleware<ExceptionMiddleware>();

                app.UseCors("AllowFrontend");

                app.UseAuthentication();

                app.UseAuthorization();

                app.MapGet("/", () => Results.Redirect("/swagger"));

                app.MapControllers();

                app.Run();
        }
    }
}
