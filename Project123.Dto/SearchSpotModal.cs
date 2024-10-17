using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project123.Dto
{

    public class SearchSpotModal
    {
        public int? SongId { get; set; }
        public string? SongName { get; set; }
        public string? SongGenres { get; set; }
        public string? SongFilePath { get; set; }
        public string? SongImagePath { get; set; }

        public int? AlbumId { get; set; }
        public string? AlbumName { get; set; }
        public string? AlbumImagePath { get; set; }

        public int? ArtistId { get; set; }
        public string? ArtistName { get; set; }
        public string? ArtistImagePath { get; set; }
        public string? ArtistGenres { get; set; }
    }
    
}
