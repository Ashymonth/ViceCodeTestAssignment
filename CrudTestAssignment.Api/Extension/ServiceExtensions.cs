using CrudTestAssignment.Log;
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