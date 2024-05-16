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

    public class BackupController : BaseController
    {



        public async Task<IActionResult> GetShipmentLocation()
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


    }
        
    

}