using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecruitmentSystem.Models
{
    public class JobApplication
    {
        public int JobApplicationId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required, StringLength(300)]
        public string ResumePath { get; set; } = string.Empty;
        [StringLength(200)]
        public string? ResumeFileName { get; set; }

        [Required, StringLength(30)]
        public string Status { get; set; } = "Applied";
        // Applied, Reviewed, Selected, Rejected, ExamSent, EmployeeCreated

        [StringLength(300)]
        public string? HRRemarks { get; set; }

        public DateTime AppliedAt { get; set; } = DateTime.Now;

        // Navigation
        [ForeignKey(nameof(UserId))]
        public User? User { get; set; }

        public ExamInvite? ExamInvite { get; set; }


    }
}
