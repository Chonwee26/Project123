using Microsoft.AspNetCore.Mvc;
using Project123.Data;
using Project123.Models;
using Project123.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using System.Data;
using Microsoft.Extensions.Configuration;

namespace Project123.Controllers
{

    public class ShipmentController : Controller
    {



        private readonly ApplicationDBContext _db;
        private readonly string connectionString;

        public ShipmentController(ApplicationDBContext db, IConfiguration configuration)
        {
            _db = db;
            connectionString = configuration.GetConnectionString("DefaultConnection");
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
       
        public IActionResult SearchShipment(Shipmentmodel ShipmentData)
        {
            try
            {
                Shipmentmodel shipping = new Shipmentmodel();
                shipping.OrderNumber = ShipmentData.OrderNumber;
                // Execute the SQL query
                var queryResult = _db.Shipment
                    .Where(s => s.OrderNumber == shipping.OrderNumber)
                    .ToList();
                if (queryResult == null || queryResult.Count == 0)
                {

                    var response = new
                    {
                        Message = "E"
                    };
                    return Json(response);
                }
                else
                {
                    var response = new
                    {
                        Message = "S",
                        Data = queryResult
                    };
                    return Json(response);

                }
                // Send data to view
               
            }
           
            catch (Exception ex)
            {

                // Log the exception or handle it as needed
                // Return message indicating an error occurred
                var response = new
                {
                    Message = "E" // "E" indicating an error
                };
                return Json(response);
            }
        
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateShipmentAjax(Shipmentmodel ShipmentData)
        {

            _db.Shipment.Add(ShipmentData);
            _db.SaveChanges();
            return Json(ShipmentData);


        }
        public IActionResult CreateShipmentAjax1(Shipmentmodel ShipmentData)
        {
            try
            {
                ShipmentData.OrderNumber = OrderGenerator.GenerateOrderNumber();
                _db.Shipment.Add(ShipmentData);

                _db.SaveChanges();
                return Json(ShipmentData) ;
            }
            catch (Exception ex)
            {

                // Log the exception or handle it appropriately
                // For example, you can return an error message to the user
                Console.WriteLine(ex.Message);
                ModelState.AddModelError("", "An error occurred while saving the shipment data.");
                return Json(ShipmentData); // or return a partial view with an error message


            }



        }


        public IActionResult DeleteShipment(Shipmentmodel ShipmentData, string Username)
        {
            object? response = null; // Declare response variable outside try block

            string sqlDeleteDoc = @" DELETE FROM Shipment WHERE ShipmentId = @ShipmentId";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(sqlDeleteDoc, connection))
                    {
                        command.CommandType = CommandType.Text;
                        command.Parameters.Clear();
                        command.Parameters.AddWithValue("@ShipmentId", ShipmentData.ShipmentId); 
                        command.ExecuteNonQuery();
                        response = new
                        {
                            Status = "S",
                            Message = "Delete shipment success! : " + ShipmentData.ShipmentId,
                            Data = ShipmentData
                        };
                    }
                }
                catch (Exception ex)
                {
                    response = new
                    {
                        Status = "E",
                        Message = ex.Message // Include exception message in response
                    };
                }
                finally
                {
                    connection.Close();
                }
            }

            return Json(response);
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