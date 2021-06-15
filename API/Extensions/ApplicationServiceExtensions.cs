// centralise les appels de services pour le startup.cs

using Application.Activities;
using Application.Core;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Persistence;

namespace API.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services,
            IConfiguration config)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });
            });
            // ajout de DataContext pour recuperer sur sqlite les données chaine de connexion
            services.AddDbContext<DataContext>(opt =>
            {
                opt.UseSqlite(config.GetConnectionString("DefaultConnection"));
            });
            //CORS BACK et Client-app indispensable pour browser
            services.AddCors(opt =>
            {
                opt.AddPolicy("CorsPolicy", policy =>
                {
                    policy.AllowAnyMethod().AllowAnyHeader().WithOrigins("http://localhost:3000");
                });
            });
            //utilisation de mediator pour recuperer la liste des activiter par activites controllor api
            services.AddMediatR(typeof(List.Handler).Assembly);
            //utilisation de automapper pour correspondance des champs assembly title title api
            services.AddAutoMapper(typeof(MappingProfiles).Assembly);

            return services;
        }
    }
}