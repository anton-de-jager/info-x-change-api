using infoX.api.Controllers;
using infoX.api.Models;
using Microsoft.EntityFrameworkCore;

namespace infoX.api.Data
{
    public class PegasusDataLakeDbContext : DbContext
    {
        public PegasusDataLakeDbContext(DbContextOptions<PegasusDataLakeDbContext> options) : base(options) { }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }
    }
}
