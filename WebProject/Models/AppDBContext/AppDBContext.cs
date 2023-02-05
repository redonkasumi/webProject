using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace WebProject.Models.AppDBContext
{
    public class AppDBContext : IdentityDbContext
    {
        private readonly DbContextOptions _options;

        public AppDBContext(DbContextOptions options) : base(options)
        {
            _options = options;
        }
        public DbSet<Faculty> Faculties { get; set; }

        public DbSet<Department> Departments { get; set; }

        public DbSet<Student> Students { get; set; }
        public DbSet<Professor> Professors { get; set; }

        public DbSet<ThesisSubject> ThesisSubjects { get; set; }

        public DbSet<ThesisRequest> ThesisRequests { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<IdentityRole>().HasData(
                new IdentityRole { Name = "Admin", NormalizedName = "ADMIN" },
                new IdentityRole { Name = "Professor", NormalizedName = "PROFESSOR" },
                new IdentityRole { Name = "Student", NormalizedName = "STUDENT" }
            );
        }
    }
}
