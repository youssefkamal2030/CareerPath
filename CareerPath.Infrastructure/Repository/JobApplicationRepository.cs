using CareerPath.Application.Interfaces;
using CareerPath.Domain.Applications;
using CareerPath.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CareerPath.Infrastructure.Repository
{
    public class JobApplicationRepository : IJobApplicationRepository
    {
        private readonly ApplicationDbContext _context;

        public JobApplicationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<JobApplication> GetByIdAsync(string id)
        {
            return await _context.JobApplications
                .FirstOrDefaultAsync(a => a.ApplicationId == id);
        }

        public async Task<IEnumerable<JobApplication>> GetByUserIdAsync(string userId)
        {
            return await _context.JobApplications
                .Where(a => a.UserId == userId)
                .ToListAsync();
        }

      
        public async Task<IEnumerable<JobApplication>> GetAllJobsAsync()
        {

            return await _context.JobApplications
                .ToListAsync();
        }

     
    }
} 