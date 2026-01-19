using Microsoft.EntityFrameworkCore;
using RecruitmentSystem.Models;

namespace RecruitmentSystem.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<JobApplication> JobApplications => Set<JobApplication>();
        public DbSet<ExamInvite> ExamInvites => Set<ExamInvite>();
        public DbSet<Employee> Employees => Set<Employee>();
        public DbSet<LeaveRequest> LeaveRequests => Set<LeaveRequest>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Unique Email
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // 1 User -> 0/1 JobApplication
            modelBuilder.Entity<User>()
                .HasOne(u => u.JobApplication)
                .WithOne(a => a.User)
                .HasForeignKey<JobApplication>(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // 1 JobApplication -> 0/1 ExamInvite
            modelBuilder.Entity<JobApplication>()
                .HasOne(a => a.ExamInvite)
                .WithOne(e => e.JobApplication)
                .HasForeignKey<ExamInvite>(e => e.JobApplicationId)
                .OnDelete(DeleteBehavior.Cascade);

            // 1 User -> 0/1 Employee
            modelBuilder.Entity<User>()
                .HasOne(u => u.Employee)
                .WithOne(e => e.User)
                .HasForeignKey<Employee>(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
