using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project123.Dto
{
    public class ShipmentModel 
    {
        [Key]
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ShipmentId { get; set; }
        public string? OrderNumber { get; set; }
        public string? FullName { get; set; }
        public string? MobileNumber { get; set; }
        public string? Storage { get; set; }
        public int? ShipmentStatus { get; set; }
        public string ShipDate { get; set; }
        public string? ShipDateFR { get; set; }
        public string? ShipDateTO { get; set; }
        public string CreateDate { get; set; }

        // Constructor to initialize properties
  

        public List<ShipmentLocationModel> ShipmentLocation { get; set; }
        public List<ShipmentStatusModel> ShipmentStatusList { get; set; }
       

        public ShipmentModel()
        {
            // Initialize properties to empty strings instead of null
            OrderNumber = "";
            FullName = "";
            MobileNumber = "";
            Storage = "";
            // ShipmentStatus is an int, so it's initialized to 0 by default, which is fine
            ShipDate = "";
            ShipDateFR = "";
            ShipDateTO = "";
            CreateDate = "";
            ShipmentLocation = new List<ShipmentLocationModel>();
            ShipmentStatusList = new List<ShipmentStatusModel>();
      
        }

    }
}

