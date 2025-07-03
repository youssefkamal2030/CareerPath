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
using MediatR;
using CareerPath.Domain.Events;

namespace CareerPath.Application.Services
{
    public class UserProfileService : IUserProfileService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMediator _mediator;

        public UserProfileService(IUnitOfWork unitOfWork, IMapper mapper, UserManager<ApplicationUser> userManager, IMediator mediator)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userManager = userManager;
            _mediator = mediator;
        }

        public async Task<UserProfileDto> GetByIdAsync(string id)
        {
            var userProfile = await _unitOfWork.UserProfiles.GetByIdAsync(id);
            return userProfile != null ? _mapper.Map<UserProfileDto>(userProfile) : null;
        }

        public async Task<IEnumerable<UserProfileDto>> GetAllAsync()
        {
            var userProfiles = await _unitOfWork.UserProfiles.GetAllAsync();
            return _mapper.Map<IEnumerable<UserProfileDto>>(userProfiles);
        }

        public async Task<UserProfileDto> CreateAsync(string userId, CreateUserProfileDto dto)
        {
            var user = await _userManager.FindByIdAsync(userId);
            string email = user?.Email ?? dto.Email;
            string username = user?.UserName ?? "User";

            var userProfile = _mapper.Map<UserProfile>(dto, opts =>
            {
                opts.Items["UserId"] = userId;
                opts.Items["Email"] = email;
                opts.Items["Username"] = username;
            });

            await _unitOfWork.UserProfiles.AddAsync(userProfile);
            await _unitOfWork.CompleteAsync();

            var userProfileDto = _mapper.Map<UserProfileDto>(userProfile);

            // Dispatch event
            var userProfileUpdatedEvent = new UserProfileUpdatedEvent
            {
                UserId = userProfile.Id,
                FirstName = userProfile.FirstName,
                LastName = userProfile.LastName,
                Skills = userProfile.Skills,
                UpdatedAt = DateTime.UtcNow
            };
            await _mediator.Publish(userProfileUpdatedEvent);

            return userProfileDto;
        }

        public async Task<UserProfileDto> UpdateAsync(string id, UpdateUserProfileDto dto)
        {
            var user = await _userManager.Users
                .Include(u => u.Profile)
                .FirstOrDefaultAsync(u => u.ProfileID == id);
            if (user == null || user.Profile == null)
                return null;

            user.Profile.UpdateProfile(
                dto.FirstName,
                dto.LastName,
                dto.Bio,
                dto.Location,
                dto.AvatarUrl,
                dto.CoverUrl,
                dto.JobTitle,
                dto.Skills ?? new List<string>()
            );
            user.Profile.UpdatedAt = DateTime.UtcNow;

            await _userManager.UpdateAsync(user); 
            await _unitOfWork.CompleteAsync();   

            var userProfileDto = _mapper.Map<UserProfileDto>(user.Profile);

            var userProfileUpdatedEvent = new UserProfileUpdatedEvent
            {
                UserId = user.Profile.Id,
                FirstName = user.Profile.FirstName,
                LastName = user.Profile.LastName,
                Skills = user.Profile.Skills,
                UpdatedAt = user.Profile.UpdatedAt
            };
            await _mediator.Publish(userProfileUpdatedEvent);

            return userProfileDto;
        }

        public async Task<bool> DeleteAsync(string id)
        {
          
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.ProfileID == id);
            if (user == null)
                return false;

            var result = await _userManager.DeleteAsync(user);
            return result.Succeeded;
        }
    }
} 