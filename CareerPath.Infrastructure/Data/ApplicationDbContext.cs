using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CareerPath.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CareerPath.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<FavoriteJob> FavoriteJobs { get; set; }
        public DbSet<UserApplication> UserApplications { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<JobApplication> JobApplications { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // ApplicationUser configuration
            builder.Entity<ApplicationUser>(entity =>
            {
                entity.HasOne(u => u.Profile)
                    .WithOne()
                    .HasForeignKey<UserProfile>(up => up.Id);
                
                entity.Property(e => e.password).IsRequired(false);
                entity.Property(e => e.ProfileID).IsRequired(false);
            });

            // Make UserName not unique (for allowing username duplicates)
            builder.Entity<ApplicationUser>()
                .HasIndex(u => u.NormalizedUserName)
                .IsUnique(false);
                
            // Configure UserProfile entity - make all properties nullable
            builder.Entity<UserProfile>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FirstName).IsRequired(false);
                entity.Property(e => e.LastName).IsRequired(false);
                entity.Property(e => e.Username).IsRequired(false);
                entity.Property(e => e.Email).IsRequired(false);
                entity.Property(e => e.Bio).IsRequired(false);
                entity.Property(e => e.Location).IsRequired(false);
                entity.Property(e => e.CoverUrl).IsRequired(false);
                entity.Property(e => e.Experiences).IsRequired(false);
                entity.Property(e => e.AvatarUrl).IsRequired(false);
                entity.Property(e => e.JobTitle).IsRequired(false);
                
                // Configure the Skills property to be stored as JSON
                entity.Property(e => e.Skills)
                    .HasConversion(
                        v => string.Join(',', v),
                        v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList())
                    .IsRequired(false);
            });

            // Configure Company entity - make all properties nullable
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
            });

            // Configure FavoriteJobs entity - make all properties nullable
            builder.Entity<FavoriteJob>(entity =>
            {
                entity.HasKey(e => e.JobId);
            });

            // Configure UserApplication entity - make all properties nullable
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

            // Configure Review entity
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
            });

            // Configure JobApplication entity
            builder.Entity<JobApplication>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.JobId).IsRequired();
                entity.Property(e => e.userId).IsRequired();
                entity.Property(e => e.ApplicationStatus).IsRequired();
                entity.Property(e => e.ApplicationDate).IsRequired();
                entity.Property(e => e.ResumeUrl).IsRequired();

                // Configure relationship with ApplicationUser
                entity.HasOne(a => a.User)
                    .WithMany()
                    .HasForeignKey(a => a.userId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired();
            });
        }
    }
}
