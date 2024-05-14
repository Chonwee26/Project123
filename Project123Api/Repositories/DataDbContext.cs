using Microsoft.EntityFrameworkCore;
//using Project123Api.Models;
using Project123.Dto;



namespace Project123Api.Repositories
{
    public class DataDbContext : DbContext
    {
        public DataDbContext(DbContextOptions<DataDbContext> options) : base(options)
        {

        }

        public DbSet<dataModel> Tb_User { get; set; }
        public DbSet<AdminModel> Tb_Admin { get; set; }

        public DbSet<ShipmentModel> Shipment { get; set; }
        public DbSet<ShipmentLocationModel> ShipmentLocation { get; set; }
        public DbSet<ShipmentLocationModel> ShipmentStatus { get; set; }
    }

}

