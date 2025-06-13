using CareerPath.Application.Interfaces;
using CareerPath.Domain.Entities;
using CareerPath.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace CareerPath.Infrastructure.Repository
{
    public class UserProfileRepository : BaseRepository<UserProfile>, IUserProfileRepository
    {
        public UserProfileRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<bool> ExistsAsync(string id)
        {
            return await _context.Set<UserProfile>().AnyAsync(e => e.Id == id);
        }
    }
}