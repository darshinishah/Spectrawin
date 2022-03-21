using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;


namespace Spectrawin.DataAccess
{
    public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options)
        { 
        
        }

        public DbSet<Applications> Applications { get; set; }

        public DbSet<SpectrawinModels> SpectrawinModels { get; set; }

        public DbSet<Labels> Labels { get; set; }

        public DbSet<Options> Options { get; set; }

        public DbSet<Users> Users { get; set; }

        public DbSet<ModelApplicationMapping> ModelApplicationMapping { get; set; }

        public DbSet<LabelApplicationMapping> LabelApplicationMapping { get; set; }

        public DbSet<OptionApplicationMapping> OptionApplicationMapping { get; set; }

        public DbSet<LicenseDetails> LicenseDetails { get; set; }

        public async Task<int> SaveChanges()
        {
            return await base.SaveChangesAsync();
        }
    }
}
