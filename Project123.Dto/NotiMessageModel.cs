using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project123.Dto
{
    public class NotiMessageModel
    {
        [Key]

        public int NotificationId { get; set; }
        public int UserId { get; set; }
        public string? NotiMessage { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreateAt { get; set; }
        public int AlbumId { get; set; }
        public int ArtistId { get; set; }
        public string? ArtistName { get; set; }
        public string? ArtistImage { get; set; }
        public string? AlbumName { get; set; }
        public string? AlbumImage { get; set; }
        public string? SongName { get; set; }
        public string? SongImage { get; set; }


    }
}


