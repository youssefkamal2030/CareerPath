using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CareerPath.Contracts.Dto;


namespace CareerPath.Application.Interfaces
{
  public interface IAuthService
    {
        Task<(bool success, string? errorMessage)> RegisterUserAsync(RegisterDto user);
        Task<(string? token, string? errorMessage)> LoginUserAsync(LoginDto user);
    }
}
