using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project123.Dto
{
    public class SongModel
    {
        public int SongId { get; set; }
        public int AlbumId { get; set; }
        public string ArtistName { get; set; }
        public string SongName { get; set; }
        public string SongGenres { get; set; }
        public IFormFile SongFile { get; set; } 
        public IFormFile SongImage { get; set; }
        public string SongFilePath { get; set; }
        public string SongImagePath { get; set; }

    }
}
