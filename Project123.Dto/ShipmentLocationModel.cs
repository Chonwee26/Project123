using System.ComponentModel.DataAnnotations;

namespace Project123.Dto
{
    public class ShipmentLocationModel
    {
        [Key]
        public string ShipmentItemID { get; set; }

        public string ShipmentItemText { get; set; }
    }
}
