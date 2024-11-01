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
    public class SpotSidebarModel
    {
        public int? UserId { get; set; }
        public int? AlbumId { get; set; }
        public string? AlbumName { get; set; }
        public string? AlbumImagePath { get; set; }

        public int? ArtistId { get; set; }
        public string? ArtistName { get; set; }
        public string? ArtistImagePath { get; set; }


        public DateTime? FavoriteDate { get; set; } = DateTime.Now;

        public bool? FavoriteAlbum { get; set; }
        public DateTime? FavoriteAlbumDate { get; set; } 
        public bool? FavoriteArtist { get; set; }
        public DateTime? FavoriteArtistDate { get; set; }


    }
}
