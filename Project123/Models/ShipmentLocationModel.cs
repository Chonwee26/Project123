using System.ComponentModel.DataAnnotations;

namespace Project123.Models
{
    public class ShipmentLocationModel
    {
        [Key]
        public string ShipmentItemID {  get; set; }

        public string ShipmentItemText { get; set; }
    }
}
