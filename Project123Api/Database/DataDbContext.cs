using Microsoft.EntityFrameworkCore;
using Project123Api.Models;




namespace Project123Api.Database
{
    public class DataDbContext : DbContext
    {
        public DataDbContext(DbContextOptions<DataDbContext> options) : base(options)
        {

        }

        public DbSet<dataModel> Tb_User { get; set; }
        public DbSet<AdminModel> Tb_Admin { get; set; }
    }

}

