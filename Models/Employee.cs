using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecruitmentSystem.Models
{
    public class Employee
    {
        public int EmployeeId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required, StringLength(20)]
        public string EmployeeCode { get; set; } = string.Empty;

        public DateTime JoiningDate { get; set; } = DateTime.Now;

        [StringLength(80)]
        public string Department { get; set; } = "General";

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation
        [ForeignKey(nameof(UserId))]
        public User? User { get; set; }

        public ICollection<LeaveRequest> LeaveRequests { get; set; } = new List<LeaveRequest>();
    }
}
