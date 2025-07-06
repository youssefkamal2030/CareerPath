using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CareerPath.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CareerPath.Infrastructure.DbInitializer
{
    public class Dbinitializer
    {
        private readonly ILogger<Dbinitializer> _logger;
        private readonly ApplicationDbContext _db;
        private readonly AIDataAnalysisDbContext _aiDb;

        public Dbinitializer(ApplicationDbContext db, ILogger<Dbinitializer> logger, AIDataAnalysisDbContext aiDb)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _aiDb = aiDb ?? throw new ArgumentNullException(nameof(aiDb)); // Added null check
        }

        public async Task InitializeAsync()
        {
            try
            {
                await InitializeMainDatabaseAsync();
                await InitializeAiDatabaseAsync();
                _logger.LogInformation("All databases initialized successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during database initialization.");
                throw; // Re-throw to let the caller decide what to do
            }
        }

        private async Task InitializeMainDatabaseAsync()
        {
            bool mainDatabaseExists = await _db.Database.CanConnectAsync();

            if (!mainDatabaseExists)
            {
                _logger.LogInformation("Main database does not exist or cannot connect. Creating and applying migrations...");
                await _db.Database.MigrateAsync();
                _logger.LogInformation("Main database migrations applied successfully.");
            }
            else
            {
                var mainPendingMigrations = await _db.Database.GetPendingMigrationsAsync();
                if (mainPendingMigrations.Any())
                {
                    _logger.LogInformation($"Found {mainPendingMigrations.Count()} pending migrations for main database. Applying...");
                    await _db.Database.MigrateAsync();
                    _logger.LogInformation("Main database migrations applied successfully.");
                }
                else
                {
                    _logger.LogInformation("No pending migrations for main database. Skipping migration.");
                }
            }
        }

        private async Task InitializeAiDatabaseAsync()
        {
            bool aiDatabaseExists = await _aiDb.Database.CanConnectAsync();

            if (!aiDatabaseExists)
            {
                _logger.LogInformation("AI database does not exist or cannot connect. Creating and applying migrations...");
                await _aiDb.Database.MigrateAsync();
                _logger.LogInformation("AI database migrations applied successfully.");
            }
            else
            {
                var aiPendingMigrations = await _aiDb.Database.GetPendingMigrationsAsync();
                if (aiPendingMigrations.Any())
                {
                    _logger.LogInformation($"Found {aiPendingMigrations.Count()} pending migrations for AI database. Applying...");
                    await _aiDb.Database.MigrateAsync();
                    _logger.LogInformation("AI database migrations applied successfully.");
                }
                else
                {
                    _logger.LogInformation("No pending migrations for AI database. Skipping migration.");
                }
            }
        }
    }
}