using System.ComponentModel.DataAnnotations;

namespace Project123.Dto
{
    public class ShipmentStatusModel
    {
        [Key]
        public string ShipmentItemID { get; set; }

        public string ShipmentItemText { get; set; }

        public ShipmentStatusModel()
        {
            // Initialize properties to empty strings instead of null
            ShipmentItemID = "";
            ShipmentItemText = "";

        }
    }
}
   
