using Microsoft.EntityFrameworkCore;
using PlatformService.Models;

namespace PlatformService.Data
{
    public class PlatformDbContext : DbContext
    {
        public DbSet<Platform> Platforms { get; set; }

        public PlatformDbContext(DbContextOptions<PlatformDbContext> options) : base(options)
        {
            
        }
    }
}
