using _1640WebApp.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace _1640WebApp.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Idea> Ideas { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Catogory> Catogorys { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<CView> CViews { get; set; }
        public DbSet<React> Reacts { get; set; }
        public DbSet<Submission> Submissions { get; set; }
        public DbSet<tNotification> tNotifications { get; set; }
    }
}