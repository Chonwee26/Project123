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



        private readonly IHttpClientFactory _httpClientFactory;

        public BackupController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }


        [HttpGet("Test/GetShipmentLocationAsync")]
        public async Task<IActionResult> GetShipmentLocationAsync()
        {
            List<ShipmentLocationModel> storageList = new List<ShipmentLocationModel>();
            using (HttpClientHandler handler = new HttpClientHandler())
            {
                // Temporarily bypass SSL certificate validation (not for production use)
                handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;

                var client = _httpClientFactory.CreateClient();
                client.BaseAddress = new Uri("https://localhost:7061/");

                 //using (var client = _httpClientFactory.CreateClient("BaseClient"))
                try
                    {

                        var response = await client.GetAsync("/api/Test/GetShipmentLocationAsync");
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

        [HttpGet("Test/GetShipmentStatusAsync")]
        public async Task<IActionResult> GetShipmentStatusAsync()
        {
            List<ShipmentLocationModel> statusList = new List<ShipmentLocationModel>();
            using (HttpClientHandler handler = new HttpClientHandler())
            {
                // Temporarily bypass SSL certificate validation (not for production use)
                handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;

                var client = _httpClientFactory.CreateClient();
                client.BaseAddress = new Uri("https://localhost:7061/");

                //using (var client = _httpClientFactory.CreateClient("BaseClient"))
                try
                {

                    var response = await client.GetAsync("/api/Test/GetShipmentStatusAsync");
                    if (response.IsSuccessStatusCode)
                    {
                        statusList = await response.Content.ReadAsAsync<List<ShipmentLocationModel>>();
                    }
                }
                catch (HttpRequestException ex)
                {
                    this.response.Status = "E";
                    this.response.Message = ex.Message;
                }
            }

            return Json(new { success = this.response.Success, message = this.response.Message, Data = statusList });
        }

    }





}
        
    

