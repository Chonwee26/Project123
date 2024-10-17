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
    public class GenreModel
    {
        [Key]
        public int? GenreId { get; set; }
        public string? GenreName { get; set; }
      
        [NotMapped]
        public IFormFile? GenreImage { get; set; }

        [Column("GenreImage")]
        public string? GenreImagePath { get; set; }

    }
}