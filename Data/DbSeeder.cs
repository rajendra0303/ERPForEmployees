using RecruitmentSystem.Models;
using RecruitmentSystem.Services;

namespace RecruitmentSystem.Data
{
    public static class DbSeeder
    {
        public static void Seed(AppDbContext db)
        {
            if (!db.Users.Any(u => u.Role == "Admin"))
            {
                db.Users.Add(new User
                {
                    Name = "System Admin",
                    Email = "admin@gmail.com",
                    Mobile = "9999999999",
                    PasswordHash = PasswordHelper.HashPassword("Admin@123"),
                    Role = "Admin"
                });
            }

            if (!db.Users.Any(u => u.Role == "HR"))
            {
                db.Users.Add(new User
                {
                    Name = "HR Manager",
                    Email = "hr@gmail.com",
                    Mobile = "8888888888",
                    PasswordHash = PasswordHelper.HashPassword("Hr@123"),
                    Role = "HR"
                });
            }

            db.SaveChanges();
        }
    }
}
