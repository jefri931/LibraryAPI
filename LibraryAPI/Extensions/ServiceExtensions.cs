using System.Security.Claims;
using System.Text;
using LibraryAPI.Database;
using LibraryAPI.Database.Models;
using LibraryAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace LibraryAPI.Extensions {
    public static class ServiceExtensions
    {
        public static void AddLibraryJwtAuth(this IServiceCollection services, ConfigurationManager configuration)
        {
            services.AddAuthentication(options => {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer("DefaultScheme", options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    RequireExpirationTime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]))
                };
            })
            .AddScheme<JwtBearerOptions, JwtBearerHandler>("PublisherScheme", options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    RequireExpirationTime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"])),
                };
                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        var claimsIdentity = context.Principal.Identity as ClaimsIdentity;
                        var roles = claimsIdentity?.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
                        if (roles != null && roles.Contains("Publisher"))
                        {
                            return Task.CompletedTask;
                        }
                        else
                        {
                            context.Fail("Unauthorized");
                            return Task.CompletedTask;
                        }
                    }
                };
            });
        }

        public static void AddLibraryAPIDbContext(this IServiceCollection services, ConfigurationManager configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(configuration.GetSection("DefaultConnection").Value,b => b.MigrationsAssembly("LibraryAPI.Database"))
            );

            services.AddIdentity<AppUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequiredLength = 8;

                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);

                options.User.RequireUniqueEmail = true;

                options.SignIn.RequireConfirmedEmail = false;
                options.SignIn.RequireConfirmedPhoneNumber = false;
            });
        }

        public static void AddLibraryServices(this IServiceCollection services)
        {
            services.AddTransient<IBookManagementService, BookManagementService>();
            services.AddTransient<IBookRentalService, BookRentalService>();
            services.AddTransient<IOverdueRentalNotificationService, OverdueRentalNotificationService>();
        }
    }
}
