using CrudTestAssignment.DAL.Models;
using CrudTestAssignment.Log;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;
using System.IO;
using System.Reflection;

namespace CrudTestAssignment.Api.Extension
{
    public static class ServiceExtensions
    {
        /// <summary>
        /// Adds a connection to the sql server
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        public static void AddSqlConnection(this IServiceCollection services, IConfiguration configuration) =>
            services.Configure<ConnectionStringOptions>(options =>
                options.ConnectionString = configuration.GetConnectionString("DefaultConnection"));

        /// <summary>
        /// Adds a logging service
        /// </summary>
        /// <param name="services"></param>
        public static void AddLoggerService(this IServiceCollection services) =>
            services.AddScoped<ILoggerManager, LoggerManager>();

        /// <summary>
        /// Adds a swagger documentation
        /// </summary>
        /// <param name="services"></param>
        public static void AddSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc(
                        "v1",
                        new OpenApiInfo
                        {
                            Title = "Crud api",
                            Version = "v1",
                            Description = "CrudTestAssignment api"
                        });
                    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                    c.IncludeXmlComments(xmlPath);
                });
            });
        }
    }
}