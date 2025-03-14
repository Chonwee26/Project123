﻿using Microsoft.EntityFrameworkCore;
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
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AlbumModel>()
                .Ignore(a => a.AlbumImage);

            modelBuilder.Entity<ArtistModel>()
              .Ignore(a => a.ArtistImage);

            modelBuilder.Entity<GenreModel>()
             .Ignore(a => a.GenreImage);

            modelBuilder.Entity<AdminModel>()
            .Ignore(a => a.ProfileImage);


            modelBuilder.Entity<SongModel>()
             .Ignore(a => a.SongImage); 
            modelBuilder.Entity<SongModel>()
             .Ignore(a => a.SongFile);

            base.OnModelCreating(modelBuilder);
        }





        public DbSet<dataModel>? Tb_User { get; set; }
        public DbSet<AdminModel>? Tb_Admin { get; set; }
        public DbSet<AlbumModel>? Albums { get; set; }
        public DbSet<ArtistModel>? Artist { get; set; }
        public DbSet<SongModel>? Song { get; set; }
        public DbSet<GenreModel>? Genre { get; set; }

        public DbSet<SpotSidebarModel>? UserArtists { get; set; }
    
      
        public DbSet<ShipmentModel>? Shipment { get; set; }
        public DbSet<ShipmentLocationModel>? ShipmentLocation { get; set; }
        public DbSet<ShipmentLocationModel>? ShipmentStatus { get; set; }
    }
}
