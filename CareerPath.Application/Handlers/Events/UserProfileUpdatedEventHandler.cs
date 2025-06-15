using CareerPath.Application.Interfaces;
using CareerPath.Domain.Entities.AIDataAnalysis;
using CareerPath.Domain.Events;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CareerPath.Application.Handlers.Events
{
    public class UserProfileUpdatedEventHandler : INotificationHandler<UserProfileUpdatedEvent>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserProfileUpdatedEventHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(UserProfileUpdatedEvent notification, CancellationToken cancellationToken)
        {
            

            var candidate = await _unitOfWork.AIDataAnalysis_Candidate.GetByIdAsync(notification.UserId);
            if (candidate == null)
            {
                candidate = new Candidate { Id = notification.UserId };
                await _unitOfWork.AIDataAnalysis_Candidate.AddAsync(candidate);
            }

            candidate.FullName = $"{notification.FirstName} {notification.LastName}";

            var personalInfo = await _unitOfWork.AIDataAnalysis_PersonalInformation.GetByIdAsync(notification.UserId);
            if (personalInfo == null)
            {
                personalInfo = new PersonalInformation { Id = notification.UserId };
                await _unitOfWork.AIDataAnalysis_PersonalInformation.AddAsync(personalInfo);
            }
            personalInfo.FirstName = notification.FirstName;
            personalInfo.LastName = notification.LastName;


            // This is a simplified skill handling. You might need more complex logic.
            var skills = notification.Skills.Select(s => new Skill { SkillName = s, UserId = notification.UserId }).ToList();
            
            // Clear existing skills and add new ones
            var existingSkills = await _unitOfWork.AIDataAnalysis_Skill.FindAsync(s => s.UserId == notification.UserId);
            _unitOfWork.AIDataAnalysis_Skill.RemoveRange(existingSkills);
            await _unitOfWork.AIDataAnalysis_Skill.AddRangeAsync(skills);

            await _unitOfWork.CompleteAsyncAi();
        }
    }
} 