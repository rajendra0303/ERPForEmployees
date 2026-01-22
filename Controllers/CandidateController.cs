using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecruitmentSystem.Data;
using RecruitmentSystem.Models;
using RecruitmentSystem.ViewModels;

namespace RecruitmentSystem.Controllers
{
    public class CandidateController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _env;

        public CandidateController(AppDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        private int? GetUserId() => HttpContext.Session.GetInt32("UserId");
        private string? GetRole() => HttpContext.Session.GetString("UserRole");

        // GET: /Candidate/Dashboard
        public async Task<IActionResult> Dashboard()
        {
            if (GetUserId() == null || GetRole() != "User")
                return RedirectToAction("Login", "Account");

            int userId = GetUserId()!.Value;

            var application = await _db.JobApplications
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.UserId == userId);

            return View(application);
        }

        // GET: /Candidate/ApplyJob
        [HttpGet]
        public async Task<IActionResult> ApplyJob(string? jobRole)
        {
            if (GetUserId() == null || GetRole() != "User")
                return RedirectToAction("Login", "Account");

            int userId = GetUserId()!.Value;

            // Already applied check
            bool alreadyApplied = await _db.JobApplications.AnyAsync(x => x.UserId == userId);
            if (alreadyApplied)
            {
                TempData["error"] = "You already applied for job. You can see status in dashboard.";
                return RedirectToAction("Dashboard");
            }

            return View(new ApplyJobVM
            {
                JobRole = jobRole ?? string.Empty
            });
        }

        // POST: /Candidate/ApplyJob
        [HttpPost]
        public async Task<IActionResult> ApplyJob(ApplyJobVM model)
        {
            if (GetUserId() == null || GetRole() != "User")
                return RedirectToAction("Login", "Account");

            int userId = GetUserId()!.Value;

            bool alreadyApplied = await _db.JobApplications.AnyAsync(x => x.UserId == userId);
            if (alreadyApplied)
            {
                TempData["error"] = "You already applied for job.";
                return RedirectToAction("Dashboard");
            }

            if (!ModelState.IsValid)
                return View(model);

            if (string.IsNullOrWhiteSpace(model.JobRole))
            {
                ModelState.AddModelError(nameof(model.JobRole), "Please select a job role.");
                return View(model);
            }

            // ✅ File Validation
            var allowedExtensions = new[] { ".pdf", ".doc", ".docx" };
            var ext = Path.GetExtension(model.ResumeFile.FileName).ToLower();

            if (!allowedExtensions.Contains(ext))
            {
                ModelState.AddModelError("", "Only PDF/DOC/DOCX files are allowed.");
                return View(model);
            }

            // ✅ Ensure folder exists
            string resumeFolder = Path.Combine(_env.WebRootPath, "resumes");
            if (!Directory.Exists(resumeFolder))
                Directory.CreateDirectory(resumeFolder);

            // ✅ Create unique filename
            string uniqueName = $"{Guid.NewGuid()}{ext}";
            string filePath = Path.Combine(resumeFolder, uniqueName);

            // ✅ Save file
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await model.ResumeFile.CopyToAsync(stream);
            }

            // ✅ Save record in DB
            var application = new JobApplication
            {
                UserId = userId,
                ResumePath = "/resumes/" + uniqueName, // url path
                ResumeFileName = model.ResumeFile.FileName,
                JobRole = model.JobRole.Trim(),
                Status = "Applied",
                AppliedAt = DateTime.Now
            };

            _db.JobApplications.Add(application);
            await _db.SaveChangesAsync();

            TempData["success"] = "Job Applied Successfully! HR will review your application.";
            return RedirectToAction("Dashboard");
        }
    }
}
