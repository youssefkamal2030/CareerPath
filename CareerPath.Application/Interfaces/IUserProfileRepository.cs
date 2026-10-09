using CareerPath.Domain.Identity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CareerPath.Application.Interfaces
{
    public interface IUserProfileRepository : IBaseRepository<UserProfile>
    {
        Task<bool> ExistsAsync(string id);
    }
} 