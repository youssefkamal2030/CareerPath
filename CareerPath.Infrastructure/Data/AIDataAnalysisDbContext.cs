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

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

         
            builder.Entity<UserApplication>()
                .HasOne(a => a.Candidate)
                .WithMany()
                .HasForeignKey(a => a.CandidateId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<UserApplication>()
                .HasOne(a => a.Job)
                .WithMany()
                .HasForeignKey(a => a.JobId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<PersonalInformation>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.UserId);
                entity.Property(e => e.Name).HasMaxLength(100);
                entity.Property(e => e.Email).HasMaxLength(100);
                entity.Property(e => e.Phone).HasMaxLength(20);
                entity.Property(e => e.Address).HasMaxLength(200);
            });

            builder.Entity<Skill>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.UserId);
                entity.Property(e => e.SkillName).HasMaxLength(100);
                entity.Property(e => e.ProficiencyLevel).HasMaxLength(50);
            });

            builder.Entity<WorkExperience>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.UserId);
                entity.Property(e => e.JobTitle).HasMaxLength(100);
                entity.Property(e => e.JobLevel).HasMaxLength(50);
                entity.Property(e => e.Company).HasMaxLength(100);
                entity.Property(e => e.JobDescription).HasMaxLength(2000);
            });

            builder.Entity<Education>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.UserId);
                entity.Property(e => e.Institution).HasMaxLength(100);
                entity.Property(e => e.Degree).HasMaxLength(100);
                entity.Property(e => e.FieldOfStudy).HasMaxLength(100);
                entity.Property(e => e.EducationLevel).HasMaxLength(50);
            });

            builder.Entity<Project>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.UserId);
                entity.Property(e => e.ProjectName).HasMaxLength(100);
                entity.Property(e => e.Url).HasMaxLength(200);
                entity.Property(e => e.Description).HasMaxLength(2000);
            });
        }
    }
} 