using DapperCourse.Models;
using Microsoft.EntityFrameworkCore;

namespace DapperCourse.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<Company> Companies { get; set; } 
    }
}
