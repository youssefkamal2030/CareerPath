﻿using AutoMapper;
using CareerPath.Contracts.Dto;
using CareerPath.Domain.Entities;
using System.Collections.Generic;

namespace CareerPath.Application.Profiles
{
    public class UserProfileMapping : Profile
    {
        public UserProfileMapping()
        {
            
     CreateMap<UserProfile, UserProfileDto>()
    .ForMember(dest => dest.Bio, opt => opt.MapFrom(src => src.Bio ?? string.Empty))
    .ForMember(dest => dest.Country, opt => opt.MapFrom(src => src.Country ?? string.Empty))
    .ForMember(dest => dest.CoverUrl, opt => opt.MapFrom(src => src.CoverUrl ?? string.Empty))
    .ForMember(dest => dest.JobTitle, opt => opt.MapFrom(src => src.JobTitle ?? string.Empty))
    .ForMember(dest => dest.AvatarUrl, opt => opt.MapFrom(src => src.AvatarUrl ?? string.Empty))
    .ForMember(dest => dest.Skills, opt => opt.MapFrom(src => src.Skills ?? new List<string>()))
    .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.Phone ?? string.Empty))
    .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.City ?? string.Empty))
    .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.DateOfBirth))
    .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address ?? string.Empty));


            CreateMap<CreateUserProfileDto, UserProfile>()
                .ConstructUsing((src, ctx) => new UserProfile(
                    ctx.Items["UserId"].ToString(),
                    src.FirstName,
                    src.LastName,
                    src.AvatarUrl,
                    ctx.Items.TryGetValue("Username", out var username) ? username?.ToString() : null,
                    ctx.Items.TryGetValue("Email", out var email) ? email?.ToString() : null
                ))
                .AfterMap((src, dest) => {
                    if (src.Skills != null)
                    {
                        dest.UpdateProfile(
                            dest.FirstName,
                            dest.LastName,
                            src.Bio,
                            src.Country,
                            dest.AvatarUrl,
                            src.CoverUrl,
                            src.JobTitle,
                            new List<string>(src.Skills)
                        );
                    }
                });

            CreateMap<UpdateUserProfileDto, UserProfile>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Username, opt => opt.Ignore())
                .ForMember(dest => dest.Email, opt => opt.Ignore())
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(dest => dest.Bio, opt => opt.MapFrom(src => src.Bio))
                .ForMember(dest => dest.Country, opt => opt.MapFrom(src => src.Country))
                .ForMember(dest => dest.AvatarUrl, opt => opt.MapFrom(src => src.AvatarUrl))
                .ForMember(dest => dest.CoverUrl, opt => opt.MapFrom(src => src.CoverUrl))
                .ForMember(dest => dest.JobTitle, opt => opt.MapFrom(src => src.JobTitle))
                .ForMember(dest => dest.Skills, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .AfterMap((src, dest) => {
                    dest.UpdateProfile(
                        src.FirstName ?? dest.FirstName,
                        src.LastName ?? dest.LastName,
                        src.Bio ?? dest.Bio,
                        src.Country ?? dest.Country,
                        src.AvatarUrl ?? dest.AvatarUrl,
                        src.CoverUrl ?? dest.CoverUrl,
                        src.JobTitle ?? dest.JobTitle,
                        src.Skills != null ? new List<string>(src.Skills) : dest.Skills
                    );
                });
        }
    }
}
