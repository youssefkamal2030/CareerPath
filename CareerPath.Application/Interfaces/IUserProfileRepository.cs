using CareerPath.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CareerPath.Application.Interfaces
{
    public interface IUserProfileRepository
    {
        Task<UserProfile> GetByIdAsync(string id);
        Task<IEnumerable<UserProfile>> GetAllAsync();
        Task<UserProfile> CreateAsync(UserProfile userProfile);
        Task<UserProfile> UpdateAsync(UserProfile userProfile);
        Task<bool> DeleteAsync(string id);
        Task<bool> ExistsAsync(string id);
    }
} 