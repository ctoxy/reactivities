// centralise les appels de services pour lidentiication vers le starupclass
using System.Text;
using System.Threading.Tasks;
using API.Services;
using Domain;
using Infrastructure.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Persistence;

namespace API.Extensions
{
    public static class IdentityServiceExtensions
    {
        public static IServiceCollection AddIdentityServices(this IServiceCollection services,
            IConfiguration config)
        {
            //complexité du mot de passe 
            services.AddIdentityCore<AppUser>(opt =>
            {
                opt.Password.RequireNonAlphanumeric = false;
            })
            //localisation de l endroit ou on stocke les user et leur roles quel db
            .AddEntityFrameworkStores<DataContext>()
            //appui sur IDENTITY pour l'authentification
            .AddSignInManager<SignInManager<AppUser>>();

            //création de la secret key de validation du token consigner en config dans le appsetting dev/prod methode alternative ci desous
            //https://docs.microsoft.com/fr-fr/aspnet/core/security/app-secrets?view=aspnetcore-5.0&tabs=windows
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"]));

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(opt =>
                {
                    //condition de validation de l api que le token est bon
                    opt.TokenValidationParameters = new TokenValidationParameters
                    {
                        //validation de la secret key est valide
                        ValidateIssuerSigningKey = true,
                        //quel secret key utiliser
                        IssuerSigningKey = key,
                        //paramétre defaut
                        ValidateIssuer = false,
                        //paramétre defaut
                        ValidateAudience = false
                    };
                    //passage des options SignalR du chat
                    opt.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var accessToken = context.Request.Query["access_token"];
                            var path = context.HttpContext.Request.Path;
                            if (!string.IsNullOrEmpty(accessToken) && (path.StartsWithSegments("/chat")))
                            {
                                context.Token = accessToken;
                            }
                            return Task.CompletedTask;
                        }
                    };
                });
            // policy for cancel and event
            services.AddAuthorization(opt =>
            {
                opt.AddPolicy("IsActivityHost", policy =>
                {
                    policy.Requirements.Add(new IsHostRequirement());
                });
            });
            services.AddTransient<IAuthorizationHandler, IsHostRequirementHandler>();
            services.AddScoped<TokenService>();

            return services;
        }
    }
}