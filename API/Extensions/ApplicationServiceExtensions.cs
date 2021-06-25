// centralise les appels de services pour l'activités vers le starupclass

using Application.Activities;
using Application.Core;
using Application.Interfaces;
using AutoMapper;
using Infrastructure.Photos;
using Infrastructure.Security;
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
                    policy
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        //credentials for signalr between back and front
                        .AllowCredentials()
                        .WithOrigins("http://localhost:3000");
                });
            });
            //utilisation de mediator pour recuperer la liste des activiter par activites controllor api
            services.AddMediatR(typeof(List.Handler).Assembly);
            //utilisation de automapper pour correspondance des champs assembly title title api
            services.AddAutoMapper(typeof(MappingProfiles).Assembly);
            //permet d'utliser le lien many many user activité en sachant qui est le user logger
            services.AddScoped<IUserAccessor, UserAccessor>();
            //permet upload delete photo de cloudinary
            services.AddScoped<IPhotoAccessor, PhotoAccessor>();
            //permet d'utliser le service cloudinary config dans appsettingg prod
            services.Configure<CloudinarySettings>(config.GetSection("Cloudinary"));
            //services SignalR stream du tchat
            services.AddSignalR();

            return services;
        }
    }
}