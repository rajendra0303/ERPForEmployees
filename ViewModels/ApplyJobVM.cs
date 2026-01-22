using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace RecruitmentSystem.ViewModels
{
    public class ApplyJobVM
    {
        [Required, StringLength(50)]
        public string JobRole { get; set; } = string.Empty;

        [Required]
        public IFormFile ResumeFile { get; set; } = default!;
    }
}
