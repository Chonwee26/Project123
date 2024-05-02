using Microsoft.AspNetCore.Mvc;
using Project123.Data;
using Project123.Models;
using Project123.Controllers;

namespace Project123.Controllers
{

    public class ShipmentController : Controller
    {
        private readonly ApplicationDBContext _db;
        public ShipmentController(ApplicationDBContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            Shipmentmodel Ship1 = new Shipmentmodel();

            Ship1.ShipmentId = 1;
            Ship1.FullName = "Chonwee";
            Ship1.MobileNumber = "0838829874";

            Shipmentmodel Ship2 = new Shipmentmodel();
            Ship2.ShipmentId = 2;
            Ship2.FullName = "Peem";
            Ship2.MobileNumber = "0812910876";

            List<Shipmentmodel> allShipment = new List<Shipmentmodel>();

            allShipment.Add(Ship1);
            allShipment.Add(Ship2);


            return View(allShipment);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SearchShipment(Shipmentmodel ShipmentData)
        {
            _db.Shipment.Add(ShipmentData);

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateShipmentAjax(Shipmentmodel ShipmentData)
        {

            _db.Shipment.Add(ShipmentData);
            _db.SaveChanges();
            return RedirectToAction("Index");


        }
        public IActionResult CreateShipmentAjax1(Shipmentmodel ShipmentData)
        {
            try
            {
                ShipmentData.OrderNumber = OrderGenerator.GenerateOrderNumber();
                _db.Shipment.Add(ShipmentData);

                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {

                // Log the exception or handle it appropriately
                // For example, you can return an error message to the user
                Console.WriteLine(ex.Message);
                ModelState.AddModelError("", "An error occurred while saving the shipment data.");
                return View(ShipmentData); // or return a partial view with an error message


            }



        }


        public IActionResult CreateShipment()
        {
            return View();
        }

    }

    public class OrderGenerator
    {
        private static Random random = new Random();

        public static string GenerateOrderNumber()
        {
            // Get the current timestamp
            long timestamp = DateTimeOffset.Now.ToUnixTimeSeconds();

            // Generate a random number
            int randomNumber = random.Next(1000, 9999); // You can adjust the range as needed

            // Combine timestamp and random number to create the order number
            string orderNumber = $"{timestamp}{randomNumber}";

            return orderNumber;
        }
    }
}