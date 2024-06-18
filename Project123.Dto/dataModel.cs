using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Project123.Dto
{
    public class dataModel
    {
        [Key]
        public int Id { get; set; }
    
        public string? Name { get; set; }
     
        public string? Age { get; set; }

        public DateTime RecordDate { get; set; } = DateTime.Now;
    }
}
