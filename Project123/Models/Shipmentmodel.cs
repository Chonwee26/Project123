using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project123.Models
{
    public class Shipmentmodel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ShipmentId { get; set; }
       
      
        public string OrderNumber { get; set; }
        [Required]
        public string FullName { get; set; }
        public string MobileNumber { get; set; }
        public string Storage { get; set; }
        public int ShipmentStatus { get; set; }
        
        public string ShipDate { get; set; }
        public string ShipDateFR { get; set; }
        public string ShipDateTO { get; set; }
        public string CreateDate { get; set; }

    }
}

