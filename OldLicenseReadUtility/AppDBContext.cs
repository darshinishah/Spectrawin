using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;


namespace OldLicenseReadUtility
{
    public class AppDBContext : DbContext
    {

        public AppDBContext(string connName) : base(SqlServerDbContextOptionsExtensions.UseSqlServer(new DbContextOptionsBuilder(), connName).Options)
        {

        }

        public DbSet<LicenseDetails> LicenseDetails { get; set; }

        public int SaveChanges()
        {
            return base.SaveChanges();
        }

    }
}
