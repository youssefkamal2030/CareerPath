using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareerPath.Domain.Entities
{
    public class UserProfile
    {
        public string  Id { get; private set; }

        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string AvatarUrl { get; private set; }
        public string JobTitle { get; private set; }

        public UserProfile(string id, string firstName, string lastName, string avatarUrl)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            AvatarUrl = avatarUrl;
        }

        public string FullName => $"{FirstName} {LastName}";

        public void UpdateProfile(string firstName, string lastName, string avatarUrl)
        {
            FirstName = firstName;
            LastName = lastName;
            AvatarUrl = avatarUrl;
        }
    }

}
