using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CareerPath.Application.Interfaces
{
    public interface ITokenService
    {
        string GenerateToken(ApplicationUser user);
        ClaimsPrincipal ValidateToken(string token);
    }
}
