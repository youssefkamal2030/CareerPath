using CareerPath.Application.Interfaces;
using CareerPath.Domain.Entities;
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

        public async Task<JobApplication> GetByIdAsync(int id)
        {
            return await _context.JobApplications
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<IEnumerable<JobApplication>> GetByUserIdAsync(string userId)
        {
            return await _context.JobApplications
                .Where(a => a.userId == userId)
                .ToListAsync();
        }

      
        public async Task<IEnumerable<JobApplication>> GetAllJobsAsync()
        {

            return await _context.JobApplications
                .ToListAsync();
        }
    }
} 