﻿using Microsoft.AspNetCore.Mvc;
using Project123.Dto;
using System.Net.Http.Headers;
using static Project123.Services.IAuthenticationService;


namespace Project123.Controllers
{
    [Route("Base")]
    public class DataModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class BaseController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ApiHelper _apiHelper;
        protected ResponseModel response;

        public BaseController(/*ApiHelper apiHelper,*/IHttpClientFactory httpClientFactory)
        {
            //_apiHelper = apiHelper;
            _httpClientFactory = httpClientFactory;
            response = new ResponseModel();
        }

        // This method can be called from derived controllers
        protected async Task<IActionResult> GetProtectedData(string endpoint)
        {
            string token = HttpContext.Session.GetString("UserToken");

            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized("Token is missing. Please log in.");
            }

            using (var handler = new HttpClientHandler())
            {
                handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
                var client = _httpClientFactory.CreateClient();
                client.BaseAddress = new Uri("https://localhost:7061/");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                try
                {
                    var response = await client.GetAsync(endpoint);

                    if (response.IsSuccessStatusCode)
                    {
                        var data = await response.Content.ReadAsAsync<DataModel>(); // Replace with your model
                        return Ok(data);
                    }
                    else
                    {
                        return StatusCode((int)response.StatusCode, "Failed to fetch protected data.");
                    }
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"Internal server error: {ex.Message}");
                }
            }
        }
    }
}
