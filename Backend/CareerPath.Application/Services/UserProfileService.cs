using CareerPath.Application.Interfaces;
using CareerPath.Contracts.Dto;
using CareerPath.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Microsoft.AspNetCore.Identity;

namespace CareerPath.Application.Services
{
    public class UserProfileService : IUserProfileService
    {
        private readonly IUserProfileRepository _userProfileRepository;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserProfileService(IUserProfileRepository userProfileRepository, IMapper mapper, UserManager<ApplicationUser> userManager)
        {
            _userProfileRepository = userProfileRepository;
            _mapper = mapper;
            _userManager = userManager;
        }

        public async Task<UserProfileDto> GetByIdAsync(string id)
        {
            try
            {
                var userProfile = await _userProfileRepository.GetByIdAsync(id);
                return userProfile != null ? _mapper.Map<UserProfileDto>(userProfile) : null;
            }
            catch (SqlException)
            {
                // Log error and return null
                Console.WriteLine("Database connection error in GetByIdAsync");
                return null;
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in GetByIdAsync: {ex.Message}");
                return null;
            }
        }

        public async Task<IEnumerable<UserProfileDto>> GetAllAsync()
        {
            try
            {
                var userProfiles = await _userProfileRepository.GetAllAsync();
                return _mapper.Map<IEnumerable<UserProfileDto>>(userProfiles);
            }
            catch (SqlException)
            {
                // Log error and return empty list
                Console.WriteLine("Database connection error in GetAllAsync");
                return new List<UserProfileDto>();
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in GetAllAsync: {ex.Message}");
                return new List<UserProfileDto>();
            }
        }

        public async Task<UserProfileDto> CreateAsync(string userId, CreateUserProfileDto dto)
        {
            try
            {
                // Get user's email and username
                var user = await _userManager.FindByIdAsync(userId);
                string email = user?.Email ?? dto.Email;
                string username = user?.UserName ?? "User";

                var userProfile = _mapper.Map<UserProfile>(dto, opts => 
                {
                    opts.Items["UserId"] = userId;
                    opts.Items["Email"] = email;
                    opts.Items["Username"] = username;
                });
                
                var createdProfile = await _userProfileRepository.CreateAsync(userProfile);
                return _mapper.Map<UserProfileDto>(createdProfile);
            }
            catch (SqlException)
            {
                // Log error
                Console.WriteLine("Database connection error in CreateAsync");
                return null;
            }
            catch (DbUpdateException)
            {
                Console.WriteLine("Database update error in CreateAsync");
                return null;
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in CreateAsync: {ex.Message}");
                return null;
            }
        }

        public async Task<UserProfileDto> UpdateAsync(string id, UpdateUserProfileDto dto)
        {
            try
            {
                var userProfile = await _userProfileRepository.GetByIdAsync(id);
                if (userProfile == null)
                    return null;

                _mapper.Map(dto, userProfile);
                var updatedProfile = await _userProfileRepository.UpdateAsync(userProfile);
                return _mapper.Map<UserProfileDto>(updatedProfile);
            }
            catch (SqlException)
            {
                // Log error
                Console.WriteLine("Database connection error in UpdateAsync");
                return null;
            }
            catch (DbUpdateException)
            {
                Console.WriteLine("Database update error in UpdateAsync");
                return null;
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in UpdateAsync: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> DeleteAsync(string id)
        {
            try
            {
                return await _userProfileRepository.DeleteAsync(id);
            }
            catch (SqlException)
            {
                // Log error
                Console.WriteLine("Database connection error in DeleteAsync");
                return false;
            }
            catch (DbUpdateException)
            {
                Console.WriteLine("Database update error in DeleteAsync");
                return false;
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in DeleteAsync: {ex.Message}");
                return false;
            }
        }
    }
} 