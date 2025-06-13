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
            var userProfile = await _unitOfWork.UserProfiles.GetByIdAsync(id);
            if (userProfile == null)
                return null;

            _mapper.Map(dto, userProfile);
            _unitOfWork.UserProfiles.Update(userProfile);
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

        public async Task<bool> DeleteAsync(string id)
        {
            var userProfile = await _unitOfWork.UserProfiles.GetByIdAsync(id);
            if (userProfile == null)
                return false;

            _unitOfWork.UserProfiles.Remove(userProfile);
            await _unitOfWork.CompleteAsync();
            return true;
        }
    }
} 