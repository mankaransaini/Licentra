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
namespace Licentra.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddDbContext<LicentraDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("LicentraConnection")));
            builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
            builder.Services.AddScoped<IDepartmentService, DepartmentService>();
            builder.Services.AddScoped<IEmployeeService, EmployeeService>();
            builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            builder.Services.AddScoped<IRoleRepository, RoleRepository>();
            builder.Services.AddScoped<IRoleService, RoleService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseMiddleware<ExceptionMiddleware>();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
