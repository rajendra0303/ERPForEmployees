using System.ComponentModel.DataAnnotations;

namespace RecruitmentSystem.ViewModels
{
    public class LeaveApplyVM
    {
        [Required]
        public DateTime FromDate { get; set; }

        [Required]
        public DateTime ToDate { get; set; }

        [Required, StringLength(300)]
        public string Reason { get; set; } = string.Empty;
    }
}
