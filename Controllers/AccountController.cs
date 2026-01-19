using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecruitmentSystem.Data;
using RecruitmentSystem.Models;
using RecruitmentSystem.Services;
using RecruitmentSystem.ViewModels;

namespace RecruitmentSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _db;

        public AccountController(AppDbContext db)
        {
            _db = db;
        }

        // GET: /Account/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            bool alreadyExists = await _db.Users.AnyAsync(x => x.Email == model.Email);
            if (alreadyExists)
            {
                ModelState.AddModelError("", "Email already registered!");
                return View(model);
            }

            var user = new User
            {
                Name = model.Name,
                Email = model.Email,
                Mobile = model.Mobile,
                PasswordHash = PasswordHelper.HashPassword(model.Password),
                Role = "User"
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            TempData["success"] = "Registration successful. Please login.";
            return RedirectToAction("Login");
        }

        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        public async Task<IActionResult> Login(LoginVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _db.Users.FirstOrDefaultAsync(x => x.Email == model.Email);

            if (user == null || !PasswordHelper.VerifyPassword(model.Password, user.PasswordHash))
            {
                ModelState.AddModelError("", "Invalid Email or Password!");
                return View(model);
            }

            // ✅ Save session
            HttpContext.Session.SetInt32("UserId", user.UserId);
            HttpContext.Session.SetString("UserName", user.Name);
            HttpContext.Session.SetString("UserRole", user.Role);

            // ✅ Role based redirect
            return user.Role switch
            {
                "Admin" => RedirectToAction("Dashboard", "Admin"),
                "HR" => RedirectToAction("Dashboard", "Hr"),
                "Employee" => RedirectToAction("Dashboard", "Employee"),
                _ => RedirectToAction("Dashboard", "Candidate"),
            };
        }

        // GET: /Account/Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
