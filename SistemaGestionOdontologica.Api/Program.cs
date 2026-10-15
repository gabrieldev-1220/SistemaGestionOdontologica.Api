
using Microsoft.EntityFrameworkCore;
using SistemaGestionOdontologica.Api.Data;

namespace SistemaGestionOdontologica.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            // Registrar controladores
            builder.Services.AddControllers();

            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            // Entity Framework Core + SQL Server.
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                /// En "DefaultConnection" se coloca el nombre que se le pone a la cadena de conexión configurada en appsettings.Development.json
                /// Si se coloca como nombre CadenaConexion en lugar de DefaultConnection en appsettings.Development.json, entonces ese mismo nombre
                /// se coloca aquí en lugar de "DefaultConnection"
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();

                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint(
                        "/openapi/v1.json",
                        "Sistema de Gestión Odontológico API v1");
                });
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
