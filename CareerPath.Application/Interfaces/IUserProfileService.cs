using CareerPath.Contracts.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CareerPath.Application.Interfaces
{
    public interface IUserProfileService
    {
        Task<UserProfileDto> GetByIdAsync(string id);
        Task<IEnumerable<UserProfileDto>> GetAllAsync();
        Task<UserProfileDto> CreateAsync(string userId, CreateUserProfileDto dto);
        Task<UserProfileDto> UpdateAsync(string id, UpdateUserProfileDto dto);
        Task<bool> DeleteAsync(string id);
    }
} 