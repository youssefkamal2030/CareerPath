using CareerPath.Domain.Applications;
using CareerPath.Domain.Identity;
using CareerPath.Domain.Recommendations;
using CareerPath.Domain.ResumeAnalysis;
using System;
using System.Threading.Tasks;
using CareerPath.Application.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;
namespace CareerPath.Application.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IUserProfileRepository UserProfiles { get; }
        ICompanyRepository Companies { get; }
        IJobRepository jobs { get; }

        IJobApplicationRepository JobApplications { get; }
        ICVAnalysisRepository CVAnalysis { get; }

        IBaseRepository<Candidate> Candidates { get; }
        IBaseRepository<PersonalInformation> PersonalInformation { get; }
        IBaseRepository<Skill> Skills { get; }
        IBaseRepository<WorkExperience> WorkExperience { get; }
        IBaseRepository<Education> Education { get; }
        IBaseRepository<Project> Projects { get; }

        Task<int> CompleteAsync();
        Task<int> CompleteAsyncAi();
        Task<IDbContextTransaction> MainDatabaseBeginTransactionAsync();
        Task<IDbContextTransaction> AiDatabaseBeginTransactionAsync();
        Task<IDbContextTransaction> BeginTransactionAsync(IsolationLevel isolationLevel);
        Task SetUserId(string id);
    }
}
