using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecruitmentSystem.Data;
using RecruitmentSystem.Models;
using RecruitmentSystem.Services;
using RecruitmentSystem.ViewModels;

namespace RecruitmentSystem.Controllers
{
    public class HrController : Controller
    {
        private readonly AppDbContext _db;
        private readonly EmailService _emailService;

        public HrController(AppDbContext db, EmailService emailService)
        {
            _db = db;
            _emailService = emailService;
        }

        private string? GetRole() => HttpContext.Session.GetString("UserRole");

        // GET: /Hr/Dashboard
        public IActionResult Dashboard()
        {
            if (GetRole() != "HR")
                return RedirectToAction("Login", "Account");

            return View();
        }

        // ✅ GET: /Hr/Candidates
        public async Task<IActionResult> Candidates()
        {
            if (GetRole() != "HR")
                return RedirectToAction("Login", "Account");

            var applications = await _db.JobApplications
                .Include(x => x.User)
                .OrderByDescending(x => x.AppliedAt)
                .ToListAsync();

            return View(applications);
        }

        // ✅ GET: /Hr/Review/5
        [HttpGet]
        public async Task<IActionResult> Review(int id)
        {
            if (GetRole() != "HR")
                return RedirectToAction("Login", "Account");

            var application = await _db.JobApplications
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.JobApplicationId == id);

            if (application == null)
                return NotFound();

            var vm = new HrCandidateReviewVM
            {
                JobApplicationId = application.JobApplicationId,
                HRRemarks = application.HRRemarks,
                Status = application.Status
            };

            ViewBag.CandidateName = application.User?.Name;
            ViewBag.ResumePath = application.ResumePath;

            return View(vm);
        }

        // ✅ POST: /Hr/Review
        [HttpPost]
        public async Task<IActionResult> Review(HrCandidateReviewVM model)
        {
            if (GetRole() != "HR")
                return RedirectToAction("Login", "Account");

            var application = await _db.JobApplications.FindAsync(model.JobApplicationId);
            if (application == null)
                return NotFound();

            application.HRRemarks = model.HRRemarks;
            application.Status = model.Status;

            await _db.SaveChangesAsync();

            TempData["success"] = "Candidate review updated successfully!";
            return RedirectToAction("Candidates");
        }

        // ✅ GET: /Hr/ConductExam/5
        [HttpGet]
        public async Task<IActionResult> ConductExam(int id)
        {
            if (GetRole() != "HR")
                return RedirectToAction("Login", "Account");

            var application = await _db.JobApplications
                .Include(x => x.User)
                .Include(x => x.ExamInvite)
                .FirstOrDefaultAsync(x => x.JobApplicationId == id);

            if (application == null)
                return NotFound();

            if (application.Status != "Selected")
            {
                TempData["error"] = "Only Selected candidates can be sent exam link!";
                return RedirectToAction("Candidates");
            }

            if (application.ExamInvite != null)
            {
                TempData["error"] = "Exam already sent for this candidate!";
                return RedirectToAction("Candidates");
            }

            return View(application);
        }

        // ✅ POST: /Hr/ConductExam
        [HttpPost]
        public async Task<IActionResult> ConductExam(int jobApplicationId, string googleFormLink)
        {
            if (GetRole() != "HR")
                return RedirectToAction("Login", "Account");

            if (string.IsNullOrWhiteSpace(googleFormLink))
            {
                TempData["error"] = "Google Form link is required!";
                return RedirectToAction("Candidates");
            }

            var application = await _db.JobApplications
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.JobApplicationId == jobApplicationId);

            if (application == null)
                return NotFound();

            // ✅ Create ExamInvite
            var invite = new ExamInvite
            {
                JobApplicationId = jobApplicationId,
                GoogleFormLink = googleFormLink,
                ResultStatus = "Pending",
                TotalQuestions = 10,
                CorrectAnswers = 0,
                IsSubmitted = false,
                SentAt = DateTime.Now
            };

            _db.ExamInvites.Add(invite);

            application.Status = "ExamSent";
            await _db.SaveChangesAsync();

            // ✅ Send email
            string subject = "Online Exam Link - Recruitment";
            string body = $@"
                <h3>Hello {application.User?.Name},</h3>
                <p>You have been shortlisted for exam.</p>
                <p><b>Exam Link:</b> <a href='{googleFormLink}' target='_blank'>Click Here to Start Exam</a></p>
                <p>Please complete the exam ASAP.</p>
                <br/>
                <p>Thanks,<br/>HR Team</p>";

            await _emailService.SendEmailAsync(application.User!.Email, subject, body);

            TempData["success"] = "Exam link sent successfully to candidate!";
            return RedirectToAction("Candidates");
        }
        // ✅ GET: /Hr/CheckExamResult/5
        public async Task<IActionResult> CheckExamResult(int id,
    [FromServices] ExamResultService examService)
        {
            if (HttpContext.Session.GetString("UserRole") != "HR")
                return RedirectToAction("Login", "Account");

            var application = await _db.JobApplications
                .Include(x => x.User)
                .Include(x => x.ExamInvite)
                .FirstOrDefaultAsync(x => x.JobApplicationId == id);

            if (application == null)
                return NotFound();

            if (application.ExamInvite == null)
            {
                TempData["error"] = "Exam is not sent yet for this candidate!";
                return RedirectToAction("Candidates");
            }

            // ✅ Call API
            var (result, raw) = await examService.GetExamResultByEmailAsync(application.User!.Email);

            

            if (result == null)
            {
                TempData["error"] = "API did not return JSON. RAW=" + raw;
                return RedirectToAction("Candidates");
            }

            if (!string.IsNullOrEmpty(result.error))
            {
                TempData["error"] = "API ERROR=" + result.error + " RAW=" + raw;
                return RedirectToAction("Candidates");
            }

            // ✅ Update ExamInvite
            application.ExamInvite.IsSubmitted = true;
            application.ExamInvite.CorrectAnswers = result.score;

            if (result.score >= 8)
            {
                application.ExamInvite.ResultStatus = "Pass";

                // Duplicate check
                bool alreadyEmp = await _db.Employees.AnyAsync(e => e.UserId == application.User.UserId);
                if (!alreadyEmp)
                {
                    application.Status = "EmployeeCreated";
                    application.User.Role = "Employee";

                    var emp = new Employee
                    {
                        UserId = application.User.UserId,
                        EmployeeCode = "EMP" + new Random().Next(1000, 9999),
                        JoiningDate = DateTime.Now,
                        Department = "IT"
                    };

                    _db.Employees.Add(emp);
                }

                await _db.SaveChangesAsync();

                TempData["success"] = $"PASS! Score={result.score}/10. Employee Created!";
            }
            else
            {
                application.ExamInvite.ResultStatus = "Fail";
                application.Status = "Rejected";
                await _db.SaveChangesAsync();

                TempData["error"] = $"FAIL! Score={result.score}/10. Candidate rejected.";
            }

            return RedirectToAction("Candidates");
        }

    }
}
