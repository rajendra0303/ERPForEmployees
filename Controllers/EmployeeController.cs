using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecruitmentSystem.Data;
using RecruitmentSystem.Models;
using RecruitmentSystem.ViewModels;

namespace RecruitmentSystem.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly AppDbContext _db;

        public EmployeeController(AppDbContext db)
        {
            _db = db;
        }

        private int? GetUserId() => HttpContext.Session.GetInt32("UserId");
        private string? GetRole() => HttpContext.Session.GetString("UserRole");

        // GET: /Employee/Dashboard
        public async Task<IActionResult> Dashboard()
        {
            if (GetUserId() == null || GetRole() != "Employee")
                return RedirectToAction("Login", "Account");

            int userId = GetUserId()!.Value;

            var employee = await _db.Employees
                .Include(e => e.User)
                .FirstOrDefaultAsync(e => e.UserId == userId);

            if (employee == null)
            {
                TempData["error"] = "Employee record not found!";
                return RedirectToAction("Login", "Account");
            }

            // Last 5 leaves show
            var leaves = await _db.LeaveRequests
                .Where(l => l.EmployeeId == employee.EmployeeId)
                .OrderByDescending(l => l.AppliedAt)
                .Take(5)
                .ToListAsync();

            ViewBag.Employee = employee;
            return View(leaves);
        }

        // ✅ GET: /Employee/ApplyLeave
        [HttpGet]
        public IActionResult ApplyLeave()
        {
            if (GetUserId() == null || GetRole() != "Employee")
                return RedirectToAction("Login", "Account");

            return View(new LeaveApplyVM
            {
                FromDate = DateTime.Today,
                ToDate = DateTime.Today
            });
        }

        // ✅ POST: /Employee/ApplyLeave
        [HttpPost]
        public async Task<IActionResult> ApplyLeave(LeaveApplyVM model)
        {
            if (GetUserId() == null || GetRole() != "Employee")
                return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
                return View(model);

            if (model.ToDate < model.FromDate)
            {
                ModelState.AddModelError("", "To Date cannot be less than From Date.");
                return View(model);
            }

            int userId = GetUserId()!.Value;

            var employee = await _db.Employees.FirstOrDefaultAsync(e => e.UserId == userId);
            if (employee == null)
            {
                TempData["error"] = "Employee record not found!";
                return RedirectToAction("Dashboard");
            }

            var leave = new LeaveRequest
            {
                EmployeeId = employee.EmployeeId,
                FromDate = model.FromDate,
                ToDate = model.ToDate,
                Reason = model.Reason,
                Status = "Pending",
                AppliedAt = DateTime.Now
            };

            _db.LeaveRequests.Add(leave);
            await _db.SaveChangesAsync();

            TempData["success"] = "Leave request submitted successfully!";
            return RedirectToAction("MyLeaves");
        }

        // ✅ GET: /Employee/MyLeaves
        public async Task<IActionResult> MyLeaves()
        {
            if (GetUserId() == null || GetRole() != "Employee")
                return RedirectToAction("Login", "Account");

            int userId = GetUserId()!.Value;

            var employee = await _db.Employees.FirstOrDefaultAsync(e => e.UserId == userId);
            if (employee == null)
            {
                TempData["error"] = "Employee record not found!";
                return RedirectToAction("Dashboard");
            }

            var leaves = await _db.LeaveRequests
                .Where(l => l.EmployeeId == employee.EmployeeId)
                .OrderByDescending(l => l.AppliedAt)
                .ToListAsync();

            return View(leaves);
        }
    }
}
