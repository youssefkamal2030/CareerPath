using System;
using CareerPath.Domain.Entities.AIDataAnalysis;
using Microsoft.EntityFrameworkCore;

namespace CareerPath.Infrastructure.Data
{
    public class AIDataAnalysisDbContext : DbContext
    {
        public AIDataAnalysisDbContext(DbContextOptions<AIDataAnalysisDbContext> options) : base(options)
        {
        }

        public DbSet<Job> Jobs { get; set; }
        public DbSet<Candidate> Candidates { get; set; }
        public DbSet<UserApplication> Applications { get; set; }
        
     
        public DbSet<PersonalInformation> PersonalInformations { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<WorkExperience> WorkExperiences { get; set; }
        public DbSet<Education> Educations { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<UserCV> userCVs { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            
        }
    }
} 