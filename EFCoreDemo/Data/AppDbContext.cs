using EFCoreDemo.Models;
using Microsoft.EntityFrameworkCore;

namespace EFCoreDemo.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {

        }



        public DbSet<Department> Departments { get; set; }

        public DbSet<Employee> Employees { get;}
    }
}
