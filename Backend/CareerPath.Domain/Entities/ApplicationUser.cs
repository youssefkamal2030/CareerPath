using CareerPath.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System;

public class ApplicationUser : IdentityUser
{
    public string password { get; set; }
    public DateTime CreatedAt { get; private set; }

    public virtual UserProfile Profile { get; private set; }

    private ApplicationUser() { }

    public ApplicationUser( string email, string password) : base(email)
    {
       
        if (string.IsNullOrWhiteSpace(email) || !email.Contains("@"))
            throw new ArgumentException("Invalid email format.", nameof(email));

        UserName = email;
        Email = email;
        CreatedAt = DateTime.UtcNow;
        this.password = password;
    }


    public void SetProfile(UserProfile profile)
    {
        Profile = profile ?? throw new ArgumentNullException(nameof(profile), "Profile cannot be null.");
    }
}
