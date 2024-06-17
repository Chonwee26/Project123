using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Project123.Dto;

namespace Project123Api.Repositories
{
    public class DataDbContext : DbContext
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public DataDbContext(DbContextOptions<DataDbContext> options, IConfiguration configuration, HttpClient httpClient) : base(options)
        {
            _configuration = configuration;
            _httpClient = httpClient;
        }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_configuration.GetConnectionString("DefaultConnection"));
            optionsBuilder.UseSqlServer(_configuration.GetConnectionString("DefaultConnection"), b => b.MigrationsAssembly("Project123Api"));
        }
     



        public DbSet<dataModel> Tb_User { get; set; }
        public DbSet<AdminModel> Tb_Admin { get; set; }

        public DbSet<ShipmentModel> Shipment { get; set; }
        public DbSet<ShipmentLocationModel> ShipmentLocation { get; set; }
        public DbSet<ShipmentLocationModel> ShipmentStatus { get; set; }
    }
}
