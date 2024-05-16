using Microsoft.AspNetCore.Mvc;
//using Project123.Data;
//using Project123.Models;
using Project123.Dto;
using Project123.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using System.Data;
using Microsoft.Extensions.Configuration;
using System.ComponentModel;
using System.Text;
using Microsoft.Extensions.Primitives;
using Project123.Migrations;
using Project123Api.Repositories;

namespace Project123.Controllers
{

    public class ShipmentController : BaseController
    {



        private readonly DataDbContext _db;
        private readonly string connectionString;

        public ShipmentController(DataDbContext db, IConfiguration configuration)
        {
            _db = db;
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }


        public IActionResult Index()
        {
            ShipmentModel Ship1 = new ShipmentModel();

            Ship1.ShipmentId = 1;
            Ship1.FullName = "Chonwee";
            Ship1.MobileNumber = "0838829874";

            ShipmentModel Ship2 = new ShipmentModel();
            Ship2.ShipmentId = 2;
            Ship2.FullName = "Peem";
            Ship2.MobileNumber = "0812910876";

            List<ShipmentModel> allShipment = new List<ShipmentModel>();

            allShipment.Add(Ship1);
            allShipment.Add(Ship2);


            return View(allShipment);

        }
        [HttpPost]

        public IActionResult SearchShipment(ShipmentModel ShipmentData)
        {
            try
            {
                ShipmentModel shipping = new ShipmentModel();
                shipping.OrderNumber = ShipmentData.OrderNumber;
                // Execute the SQL query




                var queryResult = _db.Shipment
                    .FirstOrDefault(s => s.OrderNumber == shipping.OrderNumber);
                   

                if (queryResult == null )
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
                    Message = "E" ,// "E" indicating an error
                      Status = ex.Message
                };
                return Json(response);
            }

        }


        public IActionResult SearchShipmentAll(ShipmentModel ShipmentData)
        {
            try
            {
                // Check if all fields in ShipmentData are null or empty
                if (string.IsNullOrEmpty(ShipmentData.OrderNumber) &&
                    string.IsNullOrEmpty(ShipmentData.FullName) &&
                    string.IsNullOrEmpty(ShipmentData.MobileNumber) &&
                    string.IsNullOrEmpty(ShipmentData.Storage) &&
                    (ShipmentData.ShipmentStatus == null))


                {

                    var response = new
                    {
                        Message = "E", // "E" indicating an error
                        Error = "At least one search parameter must be provided."
                    };
                    return Json(response);
                }

                var query = _db.Shipment.AsQueryable();



                if (!string.IsNullOrEmpty(ShipmentData.OrderNumber))
                {
                    query = query.Where(s => s.OrderNumber == ShipmentData.OrderNumber);
                }

                if (!string.IsNullOrEmpty(ShipmentData.FullName))
                {
                    query = query.Where(s => s.FullName == ShipmentData.FullName);
                }

                if (!string.IsNullOrEmpty(ShipmentData.MobileNumber))
                {
                    query = query.Where(s => s.MobileNumber == ShipmentData.MobileNumber);
                }
                if (!string.IsNullOrEmpty(ShipmentData.Storage))
                {
                    query = query.Where(s => s.Storage == ShipmentData.Storage);
                }

                if ((ShipmentData.ShipmentStatus != null))
                {
                    query = query.Where(s => s.ShipmentStatus == ShipmentData.ShipmentStatus);
                }


                var queryResult = query.ToList();

                if (queryResult == null || queryResult.Count == 0)
                {
                    var response = new
                    {
                        Message = "E", // "E" indicating no data found
                        Error = "No data found matching the search criteria."
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
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                // Return message indicating an error occurred
                var response = new
                {
                    Message = "E", // "E" indicating an error
                    Error = ex.Message
                };
                return Json(response);
            }
        }
        //เดี๋ยวมาทำให้มัน join ได้
        //public IActionResult SearchShipmentAll1(ShipmentModel ShipmentData,ShipmentLocationModel )
        //{
        //    try
        //    {
        //        // Check if all fields in ShipmentData are null or empty
        //        if (string.IsNullOrEmpty(ShipmentData.OrderNumber) &&
        //            string.IsNullOrEmpty(ShipmentData.FullName) &&                 
        //            string.IsNullOrEmpty(ShipmentData.MobileNumber)&&
        //            string.IsNullOrEmpty(ShipmentData.Storage)&&
        //            (ShipmentData.ShipmentStatus == null))
                
                    
        //        {
                   
        //            var response = new
        //            {
        //                Message = "E", // "E" indicating an error
        //                Error = "At least one search parameter must be provided."
        //            };
        //            return Json(response);
        //        }

        //        var query = from Shipment in _db.Shipment
        //                    join ShipmentLocation in _db.ShipmentLocation
        //                    on Shipment.Storage equals ShipmentLocation.ShipmentStorageID
        //                    select new
        //                    {
        //                        Shipment.ShipmentId,
        //                        Shipment.OrderNumber, Shipment.FullName,
        //                        Shipment.MobileNumber,Shipment.Storage,
        //                        Shipment.ShipmentStatus,Shipment.ShipDate,
        //                        Shipment.ShipDateFR,Shipment.ShipDateTO,
                                
                                
        //                        ShipmentLocation.ShipmentStorageID, ShipmentLocation.ShipmentStorageName
        //                    };



        //        if (!string.IsNullOrEmpty(ShipmentData.OrderNumber))
        //        {
        //            query = query.Where(s => s.OrderNumber == ShipmentData.OrderNumber);
        //        }

        //        if (!string.IsNullOrEmpty(ShipmentData.FullName))
        //        {
        //            query = query.Where(s => s.FullName == ShipmentData.FullName);
        //        }

        //        if (!string.IsNullOrEmpty(ShipmentData.MobileNumber))
        //        {
        //            query = query.Where(s => s.MobileNumber == ShipmentData.MobileNumber);
        //        } 
        //        if (!string.IsNullOrEmpty(ShipmentData.Storage))
        //        {
        //            query = query.Where(s => s.Storage == ShipmentData.Storage);
        //        }

        //        if ((ShipmentData.ShipmentStatus != null))
        //        {
        //            query = query.Where(s => s.ShipmentStatus == ShipmentData.ShipmentStatus);
        //        }
              

        //        var queryResult = query.ToList();

        //        if (queryResult == null || queryResult.Count == 0)
        //        {
        //            var response = new
        //            {
        //                Message = "E", // "E" indicating no data found
        //                Error = "No data found matching the search criteria."
        //            };
        //            return Json(response);
        //        }
        //        else
        //        {
        //            var response = new
        //            {
        //                Message = "S",
        //                Data = queryResult
        //            };
        //            return Json(response);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log the exception or handle it as needed
        //        // Return message indicating an error occurred
        //        var response = new
        //        {
        //            Message = "E", // "E" indicating an error
        //            Error = ex.Message
        //        };
        //        return Json(response);
        //    }
        //}



        public IActionResult SearchShipmentArray(ShipmentModel ShipmentData)
        {
            try
            {
                ShipmentModel shipping = new ShipmentModel();
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
                    Message = "E", // "E" indicating an error
                    Status = ex.Message
                };
                return Json(response);
            }

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateShipmentAjax(ShipmentModel ShipmentData)
        {

            _db.Shipment.Add(ShipmentData);
            _db.SaveChanges();
            return Json(ShipmentData);


        }
        public IActionResult CreateShipmentAjax1(ShipmentModel ShipmentData)
        {
            try
            {
                //ShipmentData.OrderNumber = OrderGenerator.GenerateOrderNumber(); // int
                ShipmentData.OrderNumber = StringGenerator.GenerateRandomString(); //string
                _db.Shipment.Add(ShipmentData);

                _db.SaveChanges();

                var response =new { Message = "Create shipment success", Status = "S", Data = ShipmentData };
              
                return Json(response);
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


        public async Task<IActionResult> GetShipmentLocationAjax()
        {
            List<ShipmentLocationModel> storageList = new List<ShipmentLocationModel>();
            using (HttpClientHandler handler = new HttpClientHandler())
            {
                // Temporarily bypass SSL certificate validation (not for production use)
                handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;

                using (HttpClient client = new HttpClient(handler))
                {
                    client.BaseAddress = new Uri("https://localhost:7061/");

                    try
                    {
                        var response = await client.GetAsync("/api/Test/GetShipmentLocation");
                        if (response.IsSuccessStatusCode)
                        {
                            storageList = await response.Content.ReadAsAsync<List<ShipmentLocationModel>>();
                        }
                    }
                    catch (HttpRequestException ex)
                    {
                        this.response.Status = "E";
                        this.response.Message = ex.Message;
                    }
                }

                return Json(new { success = this.response.Success, message = this.response.Message, Data = storageList });
            }
        }

        public IActionResult GetShipmentLocation()
        {
            List<ShipmentLocationModel> storageList = new List<ShipmentLocationModel>();
            object response = null;

            string sqlSelect = @"SELECT CONVERT(VARCHAR(10), ShipmentStorageID) AS ShipmentItemID, ShipmentStorageName AS ShipmentItemText
                         FROM ShipmentLocation
                         ORDER BY ShipmentStorageID";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (SqlDataAdapter adapter = new SqlDataAdapter(sqlSelect, connection))
                    {
                        DataTable dtResult = new DataTable();
                        adapter.Fill(dtResult);

                        foreach (DataRow row in dtResult.Rows)
                        {
                            ShipmentLocationModel model = new ShipmentLocationModel();
                            model.ShipmentItemID = row["ShipmentItemID"].ToString();
                            model.ShipmentItemText = row["ShipmentItemText"].ToString();
                            storageList.Add(model);
                        }
                    }

                    response = new
                    {
                        Status = "S",
                        Data = storageList
                    };

                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                response = new
                {
                    Status = "E",
                    ErrorMessage = ex.Message
                };
            }

            return Json(response);
        }



        public IActionResult GetShipmentStatus()
        {
            List<ShipmentLocationModel> statusList = new List<ShipmentLocationModel>();
            object response = null;

            string sqlSelect = @"SELECT CONVERT(VARCHAR(10),ShipmentStatusID) AS ShipmentItemID, ShipmentStatusName AS ShipmentItemText
                         FROM ShipmentStatus
                         ORDER BY ShipmentStatusID";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (SqlDataAdapter adapter = new SqlDataAdapter(sqlSelect, connection))
                    {
                        DataTable dtResult = new DataTable();
                        adapter.Fill(dtResult);

                        foreach (DataRow row in dtResult.Rows)
                        {
                            ShipmentLocationModel model = new ShipmentLocationModel();
                        
                            model.ShipmentItemID = row["ShipmentItemID"].ToString();
                            model.ShipmentItemText = row["ShipmentItemText"].ToString();
                            statusList.Add(model);
                            
                        }
                    }

                    response = new
                    {
                        Status = "S",
                        Data = statusList
                    };

                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                response = new
                {
                    Status = "E",
                    ErrorMessage = ex.Message
                };
            }

            return Json(response);
        }




        public IActionResult DeleteShipment(ShipmentModel ShipmentData, string Username)
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


        public IActionResult SearchShipment()
        {
            return View();
        }

    }

    //public class OrderGenerator
    //{
    //    private static Random random = new Random();

    //    public static string GenerateOrderNumber()
    //    {
    //        // Get the current timestamp
    //        long timestamp = DateTimeOffset.Now.ToUnixTimeSeconds();

    //        // Generate a random number
    //        int randomNumber = random.Next(1000, 9999); // You can adjust the range as needed

    //        // Combine timestamp and random number to create the order number
    //        string orderNumber = $"{timestamp}{randomNumber}";

    //        return orderNumber;
    //    }
    //}


    public class StringGenerator
    {
        public static string GenerateRandomString()
        {
            // Define the characters from which to generate the random string
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            int length = 10;
            // Create a StringBuilder to store the random string
            StringBuilder sb = new StringBuilder();

            // Create a Random object
            Random random = new Random();

            // Generate the random string
            for (int i = 0; i < length-1; i++)
            {
                // Append a random character from the 'chars' string
                sb.Append(chars[random.Next(length)]);
            }
            sb.Append(random.Next(5));
                
           string orderNumber = sb.ToString();
            // Return the generated random string
            return orderNumber;
        }
    }

  
}