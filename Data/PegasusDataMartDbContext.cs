using infoX.api.Controllers;
using infoX.api.Models;
using Microsoft.EntityFrameworkCore;

namespace infoX.api.Data
{
    public class PegasusDataMartDbContext : DbContext
    {
        public PegasusDataMartDbContext(DbContextOptions<PegasusDataMartDbContext> options) : base(options) { }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }
    }
}
