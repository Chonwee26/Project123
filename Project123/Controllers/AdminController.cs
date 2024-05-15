using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Project123.Dto;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
namespace Project123.Controllers
{
    public class AdminController : BaseController
    {
        //private readonly IHttpClientFactory _clientFactory;
        //private readonly IAuthenticationService _authenSvc;
        private ResponseModel response;

        //public AdminController(IHttpClientFactory clientFactory, IAuthenticationService authenticationService)
        //{
        //    _clientFactory = clientFactory;
        //    //_authenSvc = authenticationService;
        //    response = new ResponseModel(); // Initialize response model
        //}

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        public async Task<IActionResult> CreateUser(dataModel UserData)
        {
            using (HttpClientHandler handler = new HttpClientHandler())
            {
                // Temporarily bypass SSL certificate validation (not for production use)
                handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;

                using (HttpClient client = new HttpClient(handler))
                {
                    client.BaseAddress = new Uri("https://localhost:7061/");
                   // Set a timeout for the request
                    UserData = new dataModel
                    {
                        Name = "peemapi5",
                        Age = "23" // Assuming age is an integer
                    };

                    try
                    {
                        string requestJson = JsonConvert.SerializeObject(UserData);
                        HttpContent httpContent = new StringContent(requestJson, Encoding.UTF8, "application/json");

                        var responseResult = await client.PostAsync("Admin/CreateUser", httpContent);
                        if (responseResult.IsSuccessStatusCode)
                        {
                            var responseString = await responseResult.Content.ReadAsStringAsync();
                            this.response = System.Text.Json.JsonSerializer.Deserialize<ResponseModel>(responseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                        }
                        else
                        {
                            this.response.Status = "E";
                            this.response.Message = $"Error: {responseResult.StatusCode}";
                        }
                    }
                    catch (HttpRequestException httpEx)
                    {
                            // Log the HTTP request exception details
                        Console.WriteLine($"HttpRequestException: {httpEx.Message}");
                        this.response.Status = "E";
                        this.response.Message = httpEx.Message;
                    }
                    catch (Exception ex)
                    {
                        // Log the general exception details
                        Console.WriteLine($"Exception: {ex.Message}");
                        this.response.Status = "E";
                        this.response.Message = ex.Message;
                    }
                }
            }

            return Json(new { status = this.response.Status, success = this.response.Success, message = this.response.Message });
        }







        public async Task<IActionResult> Hello(dataModel UserData)
        {
            using (HttpClientHandler handler = new HttpClientHandler())
            {
                // Temporarily bypass SSL certificate validation (not for production use)
                handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;

                using (HttpClient client = new HttpClient(handler))
                {
                    client.BaseAddress = new Uri("https://localhost:7061/");
                    // Set a timeout for the request
                    UserData = new dataModel
                    {
                        Name = "peemapi5",
                        Age = "23" // Assuming age is an integer
                    };
                    try
                    {
                         string requestJson = JsonConvert.SerializeObject(UserData);
                        Console.WriteLine("Request JSON: " + requestJson);
                        HttpContent httpContent = new StringContent(requestJson, Encoding.UTF8, "application/json");
                        
                        var responseResult = await client.PostAsync("Admin/Hello", httpContent);
                        if (responseResult.IsSuccessStatusCode)
                        {
                            var responseString = await responseResult.Content.ReadAsStringAsync();
                            this.response = System.Text.Json.JsonSerializer.Deserialize<ResponseModel>(responseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                        }
                        else
                        {
                            this.response.Status = "E";
                            this.response.Message = $"Error: {responseResult.StatusCode}";
                        }
                    }
                    catch (HttpRequestException httpEx)
                    {
                        // Log the HTTP request exception details
                        Console.WriteLine($"HttpRequestException: {httpEx.Message}");
                        this.response.Status = "E";
                        this.response.Message = httpEx.Message;
                    }
                    catch (Exception ex)
                    {
                        // Log the general exception details
                        Console.WriteLine($"Exception: {ex.Message}");
                        this.response.Status = "E";
                        this.response.Message = ex.Message;
                    }
                }
            }

            return Json(new { status = this.response.Status, success = this.response.Success, message = this.response.Message });
        }
    }
    }


