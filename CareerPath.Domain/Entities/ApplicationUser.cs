using CareerPath.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations.Schema;

public class ApplicationUser : IdentityUser
{
    public string password { get; set; }
    public string? ProfileID { get; set; }
    public DateTime CreatedAt { get; private set; }
    [ForeignKey("ProfileID")]
    public virtual UserProfile? Profile { get; private set; }

    private ApplicationUser() { }

    public ApplicationUser(string email, string password, string username = null) : base(email)
    {
        if (string.IsNullOrWhiteSpace(email) || !email.Contains("@"))
            throw new ArgumentException("Invalid email format.", nameof(email));

        UserName = !string.IsNullOrWhiteSpace(username) ? username : email;
        Email = email;
        CreatedAt = DateTime.UtcNow;
        this.password = password;
        ProfileID = string.Empty;
    }

    public void SetProfile(UserProfile profile)
    {
        Profile = profile ?? throw new ArgumentNullException(nameof(profile), "Profile cannot be null.");
        ProfileID = profile.Id;
    }
}
