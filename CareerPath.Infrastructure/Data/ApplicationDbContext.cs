using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CareerPath.Domain.Entities;
using CareerPath.Domain.Identity;
using CareerPath.Domain.Applications;
using CareerPath.Domain.Recommendations;
using CareerPath.Domain.ResumeAnalysis;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CareerPath.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        
        // Identity Module
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<Review> Reviews { get; set; }
        
        // Applications Module
        public DbSet<FavoriteJob> FavoriteJobs { get; set; }
        public DbSet<CareerPath.Domain.Applications.JobApplication> JobApplications { get; set; }
        
        // Recommendations Module
        public DbSet<Job> Jobs { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Candidate> Candidates { get; set; }
        public DbSet<Skill> Skills { get; set; }
        
        // Legacy entities (will be moved in future phases)
        public DbSet<UserApplication> UserApplications { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure Job entity with all nullable properties for external ingestion
            ConfigureJobEntity(builder);
            
            // Configure other entities
            ConfigureApplicationUser(builder);
            ConfigureUserProfile(builder);
            ConfigureCompany(builder);
            ConfigureFavoriteJob(builder);
            ConfigureUserApplication(builder);
            ConfigureReview(builder);
            ConfigureJobApplication(builder);
            ConfigureCandidate(builder);
            ConfigureSkill(builder);
        }

        private void ConfigureJobEntity(ModelBuilder builder)
        {
            builder.Entity<Job>(entity =>
            {
                // Primary key
                entity.HasKey(e => e.JobId);
                
                // Make ALL properties nullable since external providers may not have all data
                entity.Property(e => e.JobId).IsRequired(); // Only JobId is required (our internal ID)
                entity.Property(e => e.ExternalId).IsRequired(false).HasMaxLength(255);
                entity.Property(e => e.JobTitle).IsRequired(false).HasMaxLength(500);
                entity.Property(e => e.JobIndustry).IsRequired(false).HasMaxLength(200);
                entity.Property(e => e.CompanyName).IsRequired(false).HasMaxLength(300);
                entity.Property(e => e.JobDescription).IsRequired(false); // No max length for descriptions
                entity.Property(e => e.RequiredSkills).IsRequired(false); // No max length for skills
                entity.Property(e => e.ExperienceLevel).IsRequired(false).HasMaxLength(100);
                entity.Property(e => e.EducationLevel).IsRequired(false).HasMaxLength(100);
                entity.Property(e => e.CertificationsRequired).IsRequired(false);
                entity.Property(e => e.RequiredLanguage).IsRequired(false).HasMaxLength(100);
                entity.Property(e => e.Location).IsRequired(false).HasMaxLength(300);
                entity.Property(e => e.SalaryRange).IsRequired(false).HasMaxLength(100);
                entity.Property(e => e.EmploymentType).IsRequired(false).HasMaxLength(100);
                entity.Property(e => e.PostingDate).IsRequired(false);
                entity.Property(e => e.ApplicationDeadline).IsRequired(false);
                entity.Property(e => e.SourceProvider).IsRequired(false).HasMaxLength(100);
                entity.Property(e => e.ExternalUrl).IsRequired(false).HasMaxLength(1000);
                
                // Domain properties
                entity.Property(e => e.Status)
                    .HasConversion<int>() // Store enum as int
                    .IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired();
                entity.Property(e => e.LastUpdated).IsRequired();
                entity.Property(e => e.ExpiredAt).IsRequired(false);
                entity.Property(e => e.UserId).IsRequired(false).HasMaxLength(450); // For manual jobs
                
                // Indexes for performance
                entity.HasIndex(e => new { e.ExternalId, e.SourceProvider })
                    .IsUnique()
                    .HasDatabaseName("IX_Jobs_ExternalId_SourceProvider");
                    
                entity.HasIndex(e => e.Status)
                    .HasDatabaseName("IX_Jobs_Status");
                    
                entity.HasIndex(e => e.PostingDate)
                    .HasDatabaseName("IX_Jobs_PostingDate");
                    
                entity.HasIndex(e => e.CompanyName)
                    .HasDatabaseName("IX_Jobs_CompanyName");
                    
                entity.HasIndex(e => e.Location)
                    .HasDatabaseName("IX_Jobs_Location");
                    
                entity.HasIndex(e => e.JobTitle)
                    .HasDatabaseName("IX_Jobs_JobTitle");

                // Configure table name with schema (if needed for modular monolith)
                entity.ToTable("Jobs", "recommendations");
            });
        }

        private void ConfigureApplicationUser(ModelBuilder builder)
        {
            builder.Entity<ApplicationUser>(entity =>
            {
                entity.HasOne(u => u.Profile)
                    .WithOne()
                    .HasForeignKey<UserProfile>(up => up.Id)
                    .OnDelete(DeleteBehavior.Cascade);
                
                entity.Property(e => e.password).IsRequired(false);
                entity.Property(e => e.ProfileID).IsRequired(false);
            });

            // Make UserName not unique (for allowing username duplicates)
            builder.Entity<ApplicationUser>()
                .HasIndex(u => u.NormalizedUserName)
                .IsUnique(false);
        }

        private void ConfigureUserProfile(ModelBuilder builder)
        {
            builder.Entity<UserProfile>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FirstName).IsRequired(false);
                entity.Property(e => e.LastName).IsRequired(false);
                entity.Property(e => e.Username).IsRequired(false);
                entity.Property(e => e.Email).IsRequired(false);
                entity.Property(e => e.Bio).IsRequired(false);
                entity.Property(e => e.AvatarUrl).IsRequired(false);
                entity.Property(e => e.CoverUrl).IsRequired(false);
                entity.Property(e => e.Experiences).IsRequired(false);
                entity.Property(e => e.JobTitle).IsRequired(false);
                
                // Configure the Skills property to be stored as JSON
                entity.Property(e => e.Skills)
                    .HasConversion(
                        v => string.Join(',', v),
                        v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList())
                    .IsRequired(false);
                    
                entity.ToTable("UserProfiles", "identity");
            });
        }

        private void ConfigureCompany(ModelBuilder builder)
        {
            builder.Entity<Company>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired(false);
                entity.Property(e => e.CompanyProfile).IsRequired(false);
                entity.Property(e => e.Location).IsRequired(false);
                entity.Property(e => e.Website).IsRequired(false);
                entity.Property(e => e.Industry).IsRequired(false);
                entity.Property(e => e.LogoUrl).IsRequired(false);
                entity.Property(e => e.Contacts).IsRequired(false);
                entity.Property(e => e.officeLocation).IsRequired(false);
                
                entity.ToTable("Companies", "recommendations");
            });
        }

        private void ConfigureFavoriteJob(ModelBuilder builder)
        {
            builder.Entity<FavoriteJob>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.UserId).IsRequired(false);
                entity.Property(e => e.JobId).IsRequired(false);
                
                entity.ToTable("FavoriteJobs", "applications");
            });
        }

        private void ConfigureUserApplication(ModelBuilder builder)
        {
            builder.Entity<UserApplication>(entity =>
            {
                entity.HasKey(e => e.ApplicationId);
                entity.Property(e => e.CandidateId).IsRequired(false);
                entity.Property(e => e.JobId).IsRequired(false);
                entity.Property(e => e.JobName).IsRequired(false);
                entity.Property(e => e.ApplicationStatus).IsRequired(false);

                // Define relationships
                entity.HasOne(a => a.Candidate)
                    .WithMany()
                    .HasForeignKey(a => a.CandidateId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired(false);

                entity.HasOne(a => a.Job)
                    .WithMany()
                    .HasForeignKey(a => a.JobId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired(false);
            });
        }

        private void ConfigureReview(ModelBuilder builder)
        {
            builder.Entity<Review>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Comment).IsRequired();

                // Configure relationship with UserProfile
                entity.HasOne(r => r.UserProfile)
                    .WithMany()
                    .HasForeignKey(r => r.UserProfileId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired();
                    
                entity.ToTable("Reviews", "identity");
            });
        }

        private void ConfigureJobApplication(ModelBuilder builder)
        {
            builder.Entity<CareerPath.Domain.Applications.JobApplication>(entity =>
            {
                entity.HasKey(e => e.ApplicationId);
                entity.Property(e => e.CandidateId).IsRequired(false);
                entity.Property(e => e.JobId).IsRequired(false);
                entity.Property(e => e.ApplicationStatus).IsRequired(false);
                entity.Property(e => e.UserId).IsRequired(false);

                // Configure relationships - these will be handled in future phases
                entity.ToTable("JobApplications", "applications");
            });
        }

        private void ConfigureCandidate(ModelBuilder builder)
        {
            builder.Entity<Candidate>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Location).IsRequired(false);
                entity.Property(e => e.Skills).IsRequired(false);
                entity.Property(e => e.ExperienceLevel).IsRequired(false);
                entity.Property(e => e.EducationLevel).IsRequired(false);
                entity.Property(e => e.Certifications).IsRequired(false);
                entity.Property(e => e.Languages).IsRequired(false);
                entity.Property(e => e.ExpectedSalary).IsRequired(false);
                entity.Property(e => e.Gender).IsRequired(false);
                entity.Property(e => e.Nationality).IsRequired(false);
                entity.Property(e => e.FullName).IsRequired(false);
                entity.Property(e => e.UserId).IsRequired(false);
                
                entity.ToTable("Candidates", "recommendations");
            });
        }

        private void ConfigureSkill(ModelBuilder builder)
        {
            builder.Entity<Skill>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.SkillName).IsRequired(false);
                entity.Property(e => e.ProficiencyLevel).IsRequired(false);
                entity.Property(e => e.UserId).IsRequired(false);
                entity.Property(e => e.PersonalInformationId).IsRequired(false);
                
                entity.ToTable("Skills", "recommendations");
            });
        }
    }
}
