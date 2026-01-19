using Microsoft.AspNetCore.Mvc;

namespace RecruitmentSystem.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Dashboard()
        {
            return View();
        }
    }
}
