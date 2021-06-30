// centralise les appels de services pour l'activités vers le starupclass

using System;
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
            /*ajout de DataContext pour recuperer sur sqlite les données chaine de connexion
            services.AddDbContext<DataContext>(opt =>
            {
                //opt.UseSqlite(config.GetConnectionString("DefaultConnection"));
                opt.UseNpgsql(config.GetConnectionString("DefaultConnection"));
            });*/
            services.AddDbContext<DataContext>(options =>
            {
                var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

                string connStr;

                // Depending on if in development or production, use either Heroku-provided
                // connection string, or development connection string from env var.
                if (env == "Development")
                {
                    // Use connection string from file.
                    connStr = config.GetConnectionString("DefaultConnection");
                }
                else
                {
                    // Use connection string provided at runtime by Heroku.
                    var connUrl = Environment.GetEnvironmentVariable("DATABASE_URL");

                    // Parse connection URL to connection string for Npgsql
                    connUrl = connUrl.Replace("postgres://", string.Empty);
                    var pgUserPass = connUrl.Split("@")[0];
                    var pgHostPortDb = connUrl.Split("@")[1];
                    var pgHostPort = pgHostPortDb.Split("/")[0];
                    var pgDb = pgHostPortDb.Split("/")[1];
                    var pgUser = pgUserPass.Split(":")[0];
                    var pgPass = pgUserPass.Split(":")[1];
                    var pgHost = pgHostPort.Split(":")[0];
                    var pgPort = pgHostPort.Split(":")[1];

                    connStr = $"Server={pgHost};Port={pgPort};User Id={pgUser};Password={pgPass};Database={pgDb}; SSL Mode=Require; Trust Server Certificate=true";
                }

                // Whether the connection string came from the local development configuration file
                // or from the environment variable from Heroku, use it to set up your DbContext.
                options.UseNpgsql(connStr);
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