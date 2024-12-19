using Microsoft.EntityFrameworkCore;
using lab7.Models;

namespace lab7.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<usersaccounts> usersaccounts { get; set; }
        public DbSet<book> book { get; set; }
        public DbSet<bookorder> bookorder { get; set; }
        public DbSet<buybook> buybook { get; set; }
    }
}
