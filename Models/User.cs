using Microsoft.AspNetCore.Builder;
using System.ComponentModel.DataAnnotations;

namespace RecruitmentSystem.Models
{
    public class User
    {
        public int UserId { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required, EmailAddress, StringLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required, StringLength(15)]
        public string Mobile { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [Required, StringLength(20)]
        public string Role { get; set; } = "User"; // User, HR, Admin, Employee

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation
        public JobApplication? JobApplication { get; set; }
        public Employee? Employee { get; set; }
    }
}
