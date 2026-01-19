using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecruitmentSystem.Models
{
    public class ExamInvite
    {
        public int ExamInviteId { get; set; }

        [Required]
        public int JobApplicationId { get; set; }

        [Required, StringLength(500)]
        public string GoogleFormLink { get; set; } = string.Empty;

        public bool IsSubmitted { get; set; } = false;

        public int CorrectAnswers { get; set; } = 0;

        public int TotalQuestions { get; set; } = 10;

        [StringLength(20)]
        public string ResultStatus { get; set; } = "Pending";
        // Pending, Pass, Fail

        public DateTime SentAt { get; set; } = DateTime.Now;

        // Navigation
        [ForeignKey(nameof(JobApplicationId))]
        public JobApplication? JobApplication { get; set; }
    }
}
