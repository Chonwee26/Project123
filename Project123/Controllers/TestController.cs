using Microsoft.AspNetCore.Mvc;
//using Project123.Data;
//using Project123.Models;
using Project123.Dto;
using Project123.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using System.Text;
using System.Data;
using Microsoft.Extensions.Configuration;
using System.ComponentModel;
using Microsoft.Extensions.Primitives;
using Project123.Migrations;
using Project123Api.Repositories;
using Newtonsoft.Json;
using System.Net.Http;

namespace Project123.Controllers
{
    public class BackupController : BaseController
    {

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ResponseModel response = new ResponseModel();
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

        [HttpPost("Test/SearchShipmentAsync")]
        public async Task<IActionResult> SearchShipmentAsync(ShipmentModel ShipmentData)
        {
          ResponseModel resp = new ResponseModel();
            List<ShipmentModel> shipmentList = new List<ShipmentModel>();
            using (HttpClientHandler handler = new HttpClientHandler())
            {
                // Temporarily bypass SSL certificate validation (not for production use)
                handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;

                var client = _httpClientFactory.CreateClient();
                client.BaseAddress = new Uri("https://localhost:7061/");


                try
                {
                    string requestJson = JsonConvert.SerializeObject(ShipmentData);
                    HttpContent httpContent = new StringContent(requestJson, Encoding.UTF8, "application/json");

                    var response = await client.PostAsync("/api/Test/SearchShipmentAsync", httpContent);
                    if (response.IsSuccessStatusCode)
                    {
                        shipmentList = await response.Content.ReadAsAsync<List<ShipmentModel>>();

                        if (shipmentList.Count >0)
                        {
                            resp.Status = "S";
                            resp.Message = "Success";
                        }

                        else
                        {
                            resp.Status = "E";
                            resp.Message = $"Error:";
                        }


                        ////this.response = System.Text.Json.JsonSerializer.Deserialize<ResponseModel>(responseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    }
                    else
                    {
                        resp.Status = "E";
                        resp.Message = $"Error:";
                    }
                }

                catch (Exception ex)
                {
                    this.response.Status = "E";
                    this.response.Message = ex.Message;
                }
            }
            
            return Json(new { status = resp.Status, success = resp.Success, message = resp.Message, Data = shipmentList });
        }

        [HttpDelete("Test/DeleteShipmentAsync/{id}")]

        public async Task<IActionResult> DeleteShipmentAsync(int id )
        {
            ResponseModel resp = new ResponseModel();
         
            using (HttpClientHandler handler = new HttpClientHandler())
            {
                // Temporarily bypass SSL certificate validation (not for production use)
                handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;

                var client = _httpClientFactory.CreateClient();
                client.BaseAddress = new Uri("https://localhost:7061/");


                try
                        {
                    //string requestJson = JsonConvert.SerializeObject(id);
                    //HttpContent httpContent = new StringContent(requestJson, Encoding.UTF8, "application/json");
                    // Log the request URL
                    var requestUrl = $"/api/Test/DeleteShipmentAsync/{id}";
                    Console.WriteLine($"Sending DELETE request to: {requestUrl}");
                    var response = await client.DeleteAsync(requestUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        resp = await response.Content.ReadAsAsync<ResponseModel>();

                        ////this.response = System.Text.Json.JsonSerializer.Deserialize<ResponseModel>(responseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                        resp.Status = "S";
                        resp.Message = "Delete Success";
                    }
                    else
                    {
                        resp.Status = "E";
                        resp.Message = $"Error:";
                    }
                }

                catch (Exception ex)
                {
                    this.response.Status = "E";
                    this.response.Message = ex.Message;
                }
            }

            return Json(new { status = resp.Status, success = resp.Success, message = resp.Message });
        }

        [HttpPost("Test/CreateShipmentAsync")]

        public async Task<IActionResult> CreateShipmentAsync(ShipmentModel ShipmentData)
        {
            ResponseModel resp = new ResponseModel();
            List<ShipmentModel> shipmentList = new List<ShipmentModel>();
            using (HttpClientHandler handler = new HttpClientHandler())
            {
                // Temporarily bypass SSL certificate validation (not for production use)
                handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;

                var client = _httpClientFactory.CreateClient();
                client.BaseAddress = new Uri("https://localhost:7061/");


                try
                {
                    string requestJson = JsonConvert.SerializeObject(ShipmentData);
                    HttpContent httpContent = new StringContent(requestJson, Encoding.UTF8, "application/json");
                    // Log the request URL
                    var requestUrl = $"/api/Test/CreateShipmentAsync";
                 
                    var response = await client.PostAsync(requestUrl,httpContent);

                    if (response.IsSuccessStatusCode)
                    {
                        resp = await response.Content.ReadAsAsync<ResponseModel>();

                        ////this.response = System.Text.Json.JsonSerializer.Deserialize<ResponseModel>(responseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    }
                    else
                    {
                        resp.Status = "E";
                        resp.Message = $"Error:";
                    }
                }

                catch (Exception ex)
                {
                    this.response.Status = "E";
                    this.response.Message = ex.Message;
                }
            }

            return Json(new { status = resp.Status, success = resp.Success, message = resp.Message, data = shipmentList });
        }

        [HttpPost("Test/UpdateShipmentAsync")]
        public async Task<IActionResult> UpdateShipmentAsync(ShipmentModel ShipmentData)
        {
            ResponseModel resp = new ResponseModel();
            List<ShipmentModel> shipmentList = new List<ShipmentModel>();
            using (HttpClientHandler handler = new HttpClientHandler())
            {
                // Temporarily bypass SSL certificate validation (not for production use)
                handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;

                var client = _httpClientFactory.CreateClient();
                client.BaseAddress = new Uri("https://localhost:7061/");


                try
                {
                    string requestJson = JsonConvert.SerializeObject(ShipmentData);
                    HttpContent httpContent = new StringContent(requestJson, Encoding.UTF8, "application/json");
                    // Log the request URL
                    var requestUrl = $"/api/Test/UpdateShipmentAsync";
                 
                    var response = await client.PostAsync(requestUrl,httpContent);

                    if (response.IsSuccessStatusCode)
                    {
                        shipmentList = await response.Content.ReadAsAsync<List<ShipmentModel>>();

                        ////this.response = System.Text.Json.JsonSerializer.Deserialize<ResponseModel>(responseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                        resp.Status = "S";
                        resp.Message = "Success";
                    }
                    else
                    {
                        resp.Status = "E";
                        resp.Message = $"Error:";
                    }
                }

                catch (Exception ex)
                {
                    this.response.Status = "E";
                    this.response.Message = ex.Message;
                }
            }

            return Json(new { status = resp.Status, success = resp.Success, message = resp.Message, data = shipmentList });
        }
    }
}

        
    

 