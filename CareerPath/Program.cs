using System;
using System.Collections.Generic;
using CareerPath.Application.Profiles;
using Microsoft.OpenApi.Models;
using System.Reflection;
using MediatR;
using Microsoft.AspNetCore.HttpOverrides;
using CareerPath.Infrastructure;
using CareerPath.Infrastructure.DbInitializer;
using CareerPath.Infrastructure.Configuration;
namespace CareerPath
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add Infrastructure services
            builder.Services.AddInfraStructure(builder.Configuration);

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

            // Register MediatR
            builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Application.Services.UserProfileService).Assembly));

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

            builder.Services.AddControllers().AddJsonOptions(options => 
            {
                options.JsonSerializerOptions.ReferenceHandler = null;
                options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.Never;
                options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
            });

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
            
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var logger = services.GetRequiredService<ILogger<Program>>();

                try
                {
                    // Validate application configuration
                    logger.LogInformation("Validating application configuration...");
                    var settingsProvider = services.GetRequiredService<IAppSettingsProvider>();
                    var configErrors = settingsProvider.ValidateConfiguration();
                    
                    if (configErrors.Any())
                    {
                        logger.LogWarning("Configuration validation warnings:");
                        foreach (var (section, message) in configErrors)
                        {
                            logger.LogWarning($"  [{section}] {message}");
                        }
                    }
                    else
                    {
                        logger.LogInformation("Configuration validated successfully.");
                    }

                    var dbInitializer = services.GetRequiredService<Dbinitializer>();
                    await dbInitializer.InitializeAsync();
                    logger.LogInformation("Database initialization completed successfully.");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to initialize databases during application startup.");
                }
            }

            app.UseCors("AllowAll");
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
           
            app.Run();
        }
    }
}
