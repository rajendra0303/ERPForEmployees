using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace RecruitmentSystem.ViewModels
{
    public class ApplyJobVM
    {
        [Required]
        public IFormFile ResumeFile { get; set; } = default!;
    }
}
