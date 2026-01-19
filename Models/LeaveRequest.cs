using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecruitmentSystem.Models
{
    public class LeaveRequest
    {
        public int LeaveRequestId { get; set; }

        [Required]
        public int EmployeeId { get; set; }

        [Required]
        public DateTime FromDate { get; set; }

        [Required]
        public DateTime ToDate { get; set; }

        [Required, StringLength(300)]
        public string Reason { get; set; } = string.Empty;

        [Required, StringLength(20)]
        public string Status { get; set; } = "Pending";
        // Pending, Approved, Rejected

        [StringLength(300)]
        public string? AdminComment { get; set; }

        public DateTime AppliedAt { get; set; } = DateTime.Now;

        // Navigation
        [ForeignKey(nameof(EmployeeId))]
        public Employee? Employee { get; set; }
    }
}
