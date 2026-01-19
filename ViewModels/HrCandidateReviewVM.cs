using System.ComponentModel.DataAnnotations;

namespace RecruitmentSystem.ViewModels
{
    public class HrCandidateReviewVM
    {
        public int JobApplicationId { get; set; }

        [StringLength(300)]
        public string? HRRemarks { get; set; }

        [Required]
        public string Status { get; set; } = "Reviewed";
        // Reviewed / Selected / Rejected
    }
}
