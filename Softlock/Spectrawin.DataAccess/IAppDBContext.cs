using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Spectrawin.DataAccess
{
    public interface IAppDBContext
    {
        DbSet<Applications> Applications { get; set; }

        Task<int> SaveChanges();
    }
}
