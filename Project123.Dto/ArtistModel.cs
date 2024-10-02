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
    public class ArtistModel
    {
        [Key]
        public int? ArtistId { get; set; }    
        public string? ArtistName { get; set; }
        public string? ArtistBio { get; set; }
        [NotMapped]
        public IFormFile? ArtistImage { get; set; }

        [Column("ArtistImage")]
        public string? ArtistImagePath { get; set; }

        public string? ArtistGenres { get; set; }

    }
}