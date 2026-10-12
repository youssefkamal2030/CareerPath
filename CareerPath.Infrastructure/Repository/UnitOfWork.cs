using CareerPath.Application.Interfaces;
using CareerPath.Domain.Identity;
using CareerPath.Domain.Recommendations;
using CareerPath.Domain.ResumeAnalysis;
using CareerPath.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Threading.Tasks;

namespace CareerPath.Infrastructure.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private readonly AIDataAnalysisDbContext _aiContext;
        private readonly ILogger<JobRepository> _logger;

        public IUserProfileRepository UserProfiles { get; private set; }
        public ICompanyRepository Companies { get; private set; }
        public IJobRepository Jobs { get; }
        public IJobApplicationRepository JobApplications { get; private set; }
        public ICVAnalysisRepository CVAnalysis { get; private set; }

        public IBaseRepository<Candidate> AIDataAnalysis_Candidate { get; private set; }
        public IBaseRepository<PersonalInformation> AIDataAnalysis_PersonalInformation { get; private set; }
        public IBaseRepository<Skill> AIDataAnalysis_Skill { get; private set; }
        public IBaseRepository<WorkExperience> AIDataAnalysis_WorkExperience { get; private set; }
        public IBaseRepository<Education> AIDataAnalysis_Education { get; private set; }
        public IBaseRepository<Project> AIDataAnalysis_Project { get; private set; }
        public UnitOfWork(
     ApplicationDbContext context,
     AIDataAnalysisDbContext aiContext,
     IUserProfileRepository userProfiles,
     ICompanyRepository companies,
     IJobRepository jobs,
     IJobApplicationRepository jobApplications,
     ICVAnalysisRepository cvAnalysis)
        {
            _context = context;
            _aiContext = aiContext;
            UserProfiles = userProfiles;
            Companies = companies;
            Jobs = jobs;
            JobApplications = jobApplications;
            CVAnalysis = cvAnalysis;
        }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task<int> CompleteAsyncAi()
        {
            return await _aiContext.SaveChangesAsync();
        }
        public async Task<IDbContextTransaction> MainDatabaseBeginTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync();
        }
        public async Task<IDbContextTransaction> AiDatabaseBeginTransactionAsync()
        {
            return await _aiContext.Database.BeginTransactionAsync();
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync(IsolationLevel isolationLevel)
        {
            return await _context.Database.BeginTransactionAsync(isolationLevel, default);
        }
        public async Task SetUserId(string id)
        {
            //ApplicationUser user = await _aiContext.ApplicationUser.FirstOrDefaultAsync(u => u.Id == id);
            //_aiContext.ApplicationUser.Add(user);
            throw new NotImplementedException();
        }
        public void Dispose()
        {
            _context.Dispose();
            _aiContext.Dispose();
        }
    }
} 