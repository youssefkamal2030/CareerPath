using CareerPath.Application.Interfaces;
using CareerPath.Application.Services;
using CareerPath.Domain.Entities;
using CareerPath.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using CareerPath.Application.Profiles;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using System.Reflection;

namespace CareerPath
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("MonsterAPI"), 
                sqlServerOptionsAction: sqlOptions => 
                {
                    sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorNumbersToAdd: null);
                }));
            // Add Identity Services
            builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 8;
                options.Password.RequiredUniqueChars = 0;
                options.User.RequireUniqueEmail = true;
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();
            // Configure JWT Authentication
            var jwtSettings = builder.Configuration.GetSection("Jwt");
            var secretKey = jwtSettings["SecretKey"];
            var key = GetSecretKeyBytes(secretKey);
            
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false; // Set to true in production
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidateAudience = true,
                    ValidAudience = jwtSettings["Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });
            //CORS 
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    policy =>
                    {
                        policy.AllowAnyOrigin()
                              .AllowAnyMethod()
                              .AllowAnyHeader();
                    });
            });
            // AutoMapper
            builder.Services.AddAutoMapper(typeof(UserProfileMapping).Assembly);

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "CareerPath API",
                    Version = "v1",
                    Description = "REST API for the CareerPath application"
                });
                
                // JWT Bearer authentication scheme
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below. Example: 'Bearer 12345abcdef'",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
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
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header
                        },
                        new List<string>()
                    }
                });
            });

            builder.Services.AddControllers();  
            builder.Services.AddScoped<ITokenService, TokenService>();
            builder.Services.AddScoped<IAuthService, AuthService>();

            //  UserProfile services
            builder.Services.AddScoped<IUserProfileRepository, UserProfileRepository>();
            builder.Services.AddScoped<IUserProfileService, UserProfileService>();
            builder.Services.AddScoped<IEmailService, SmtpEmailService>();

            var app = builder.Build();
            if(app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "CareerPath API V1");
                    options.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
                    options.OAuthUsePkce();
                });
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            //   HTTP request pipeline.
            app.UseHttpsRedirection();

            app.UseCors("AllowAll");
            
            //  authentication middleware before authorization
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }

        private static byte[] GetSecretKeyBytes(string secretKey)
        {
           
            try
            {
                return Convert.FromBase64String(secretKey);
            }
            catch (FormatException)
            {
              
                string base64Secret = Convert.ToBase64String(Encoding.UTF8.GetBytes(secretKey));
                return Convert.FromBase64String(base64Secret);
            }
        }
    }
}
