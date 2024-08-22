using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project123.Dto
{
    public class SongModel
    {
        [Key]
        public int? SongId { get; set; }
        public int? AlbumId { get; set; }
        public string? ArtistName { get; set; }
        public string? SongName { get; set; }
        public string? SongGenres { get; set; }

    
    
        public IFormFile? SongFile { get; set; }     
        public IFormFile? SongImage { get; set; } 
        public int? SongLength { get; set; }
        [Column("SongFile")]
        public string? SongFilePath { get; set; }
        [Column("SongImage")]
        public string? SongImagePath { get; set; }
    }
}
