using CareerPath.Application.Interfaces;
using CareerPath.Infrastructure.Data;
using Microsoft.Extensions.Logging;
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
        public IJobRepository jobs { get; }
        public IJobApplicationRepository JobApplications { get; private set; }
        public ICVAnalysisRepository CVAnalysis { get; private set; }

        // AI DB Repositories - these need to be created
        public IBaseRepository<Domain.Entities.AIDataAnalysis.Candidate> AIDataAnalysis_Candidate { get; private set; }
        public IBaseRepository<Domain.Entities.AIDataAnalysis.PersonalInformation> AIDataAnalysis_PersonalInformation { get; private set; }
        public IBaseRepository<Domain.Entities.AIDataAnalysis.Skill> AIDataAnalysis_Skill { get; private set; }
        public IBaseRepository<Domain.Entities.AIDataAnalysis.WorkExperience> AIDataAnalysis_WorkExperience { get; private set; }
        public IBaseRepository<Domain.Entities.AIDataAnalysis.Education> AIDataAnalysis_Education { get; private set; }
        public IBaseRepository<Domain.Entities.AIDataAnalysis.Project> AIDataAnalysis_Project { get; private set; }
        public UnitOfWork(ApplicationDbContext context, AIDataAnalysisDbContext aiContext, ILogger<JobRepository> logger)
        {
            _context = context;
            _aiContext = aiContext;
            _logger = logger;
            

            UserProfiles = new UserProfileRepository(_context);
            Companies = new CompanyRepository(_context);
            jobs = new JobRepository(_aiContext, _logger);
            JobApplications = new JobApplicationRepository(_context);
            CVAnalysis = new CVAnalysisRepository(_aiContext);

            // AI DB Repositories
            AIDataAnalysis_Candidate = new BaseRepository<Domain.Entities.AIDataAnalysis.Candidate>(_aiContext);
            AIDataAnalysis_PersonalInformation = new BaseRepository<Domain.Entities.AIDataAnalysis.PersonalInformation>(_aiContext);
            AIDataAnalysis_Skill = new BaseRepository<Domain.Entities.AIDataAnalysis.Skill>(_aiContext);
            AIDataAnalysis_WorkExperience = new BaseRepository<Domain.Entities.AIDataAnalysis.WorkExperience>(_aiContext);
            AIDataAnalysis_Education = new BaseRepository<Domain.Entities.AIDataAnalysis.Education>(_aiContext);
            AIDataAnalysis_Project = new BaseRepository<Domain.Entities.AIDataAnalysis.Project>(_aiContext);
        }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task<int> CompleteAsyncAi()
        {
            return await _aiContext.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
            _aiContext.Dispose();
        }
    }
} 