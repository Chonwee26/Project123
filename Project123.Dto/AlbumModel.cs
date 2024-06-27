using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Project123.Dto
{
    public class AlbumModel
    {
        public int? AlbumId { get; set; }
        public string? AlbumName { get; set; }
        public string? ArtistName { get; set; }
        public IFormFile? AlbumImage { get; set; }
        public string? AlbumImagePath { get; set; }

    }
}