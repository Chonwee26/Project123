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
    public class AlbumModel
    {
        [Key]
        public int? AlbumId { get; set; }
        public string? AlbumName { get; set; }
        public string? ArtistName { get; set; }
        [NotMapped]
        public IFormFile? AlbumImage { get; set; }

        [Column("AlbumImage")]
        public string? AlbumImagePath { get; set; }
        public int? AlbumGenre { get; set; }

    }
}