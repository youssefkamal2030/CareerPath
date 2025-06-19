using CareerPath.Domain.Entities.AIDataAnalysis;
using System;
using System.Threading.Tasks;
using CareerPath.Application.Interfaces;
using CareerPath.Infrastructure.Repository;
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

        IBaseRepository<Candidate> AIDataAnalysis_Candidate { get; }
        IBaseRepository<PersonalInformation> AIDataAnalysis_PersonalInformation { get; }
        IBaseRepository<Skill> AIDataAnalysis_Skill { get; }
        IBaseRepository<WorkExperience> AIDataAnalysis_WorkExperience { get; }
        IBaseRepository<Education> AIDataAnalysis_Education { get; }
        IBaseRepository<Project> AIDataAnalysis_Project { get; }

        Task<int> CompleteAsync();
        Task<int> CompleteAsyncAi();
        Task<IDbContextTransaction> BeginTransactionAsync();
        Task<IDbContextTransaction> BeginTransactionAsync(IsolationLevel isolationLevel);
    }
}
