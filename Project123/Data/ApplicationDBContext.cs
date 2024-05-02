using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Project123.Models;

namespace Project123.Data
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
        {
         
        }

        // DbSet for each of your model classes
        public DbSet<Shipmentmodel> Shipment { get; set; }
    }
}
