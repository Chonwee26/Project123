using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Formatting;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Project123.Dto;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;

namespace Project123.Controllers
{
    public class AdminController : BaseController
    {
        //private readonly IHttpClientFactory _clientFactory;
        //private readonly IAuthenticationService _authenSvc;


        private readonly IHttpClientFactory _httpClientFactory;
      
        public AdminController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

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

        public IActionResult LoginPage()
        {
            return View();
        }


        public IActionResult TabletLoginPage()
        {
            return View();
        }

        public IActionResult RegisterPage()
        {
            return View();
        }

        public IActionResult UserPage()
        {
            return View();
        }
        [HttpPost("Admin/CreateUser123")]
        public async Task<IActionResult> CreateUser(dataModel UserData)
        {
            using (HttpClientHandler handler = new HttpClientHandler())
            {
                // Temporarily bypass SSL certificate validation (not for production use)
                handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;

                using (HttpClient client = new HttpClient(handler))
                {
                    client.BaseAddress = new Uri("https://localhost:7061/");

                   

                    try
                    {
                        string requestJson = JsonConvert.SerializeObject(UserData);
                        HttpContent httpContent = new StringContent(requestJson, Encoding.UTF8, "application/json");

                        var responseResult = await client.PostAsync("api/Admin/CreateUser123", httpContent);
                        if (responseResult.IsSuccessStatusCode)
                        {
                            this.response = await responseResult.Content.ReadAsAsync<ResponseModel>();
                           ;
                            ////this.response = System.Text.Json.JsonSerializer.Deserialize<ResponseModel>(responseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                         
                        }
                        else
                        {
                            this.response.Status = "E";
                            this.response.Message = $"Error: {responseResult.StatusCode}";
                        }
                    }
                 
                    catch (Exception ex)
                    {

                   
                        this.response.Status = "E";
                        this.response.Message = ex.Message;
                    }
                }
            }

             return Json(new { status = this.response.Status, success = this.response.Success, message = this.response.Message });
        }

        [HttpDelete("Admin/DeleteUser/{id}")]

        public async Task<IActionResult> DeleteUser(int id)
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
                    var requestUrl = $"/api/Admin/DeleteUser/{id}";
                   
                    var response = await client.DeleteAsync(requestUrl);

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

            return Json(new { status = resp.Status, success = resp.Success, message = resp.Message });
        }
       
        [HttpPost("Admin/SearchUser1")]
        public async Task<IActionResult> SearchUser(dataModel UserData)
        {
            ResponseModel resp = new ResponseModel();
            List<dataModel> UserList = new List<dataModel>();

            using (HttpClientHandler handler = new HttpClientHandler())
            {
                // Temporarily bypass SSL certificate validation (not for production use)
                handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;

                var client = _httpClientFactory.CreateClient();
                client.BaseAddress = new Uri("https://localhost:7061/");

                try
                {
                    // Retrieve the token from session storage or wherever it was stored
                    string token = HttpContext.Session.GetString("UserToken")??string.Empty;

                    // Add the token to the request header for authentication
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                    string requestJson = JsonConvert.SerializeObject(UserData);
                    HttpContent httpContent = new StringContent(requestJson, Encoding.UTF8, "application/json");

                    var response = await client.PostAsync("/api/Admin/SearchUser1", httpContent);
                    if (response.IsSuccessStatusCode)
                    {
                        UserList = await response.Content.ReadAsAsync<List<dataModel>>();

                        if (UserList.Count > 0)
                        {
                            resp.Status = "S";
                            resp.Message = "Success";
                        }
                        else
                        {
                            resp.Status = "E";
                            resp.Message = "No users found.";
                        }
                    }
                    else
                    {
                        resp.Status = "E";
                        resp.Message = $"Error: {response.StatusCode}"; 
                    }
                }
                catch (Exception ex)
                {
                    resp.Status = "E";
                    resp.Message = $"An error occurred: {ex.Message}";
                }
            }
            // message forebidden คือ role ผิดนะจ๊ะ
            return Json(new { status = resp.Status, success = resp.Status == "S", message = resp.Message, Data = UserList });
        }


        [HttpPost("Admin/Login")]

        public async Task<IActionResult> Login(AdminModel UserData)
        {
            ResponseModel resp = new ResponseModel();
            List<AdminModel> UserList = new List<AdminModel>();
            using (HttpClientHandler handler = new HttpClientHandler())
            {
                // Temporarily bypass SSL certificate validation (not for production use)
                handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;

                var client = _httpClientFactory.CreateClient();
                client.BaseAddress = new Uri("https://localhost:7061/");


                try
                {
                    string requestJson = JsonConvert.SerializeObject(UserList);
                    HttpContent httpContent = new StringContent(requestJson, Encoding.UTF8, "application/json");
                    // Log the request URL
                    var requestUrl = $"/api/Admin/Login";

                    var response = await client.PostAsync(requestUrl, httpContent);

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

            return Json(new { status = resp.Status, success = resp.Success, message = resp.Message, data = UserList });
        }


        [HttpPost("Admin/Login1")]
        public async Task<IActionResult> Login1(AdminModel UserData)
        {
            using (HttpClientHandler handler = new HttpClientHandler())
            {
                // Temporarily bypass SSL certificate validation (not for production use)
                handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;

                using (HttpClient client = new HttpClient(handler))
                {
                    client.BaseAddress = new Uri("https://localhost:7061/");

                    UserData = new AdminModel
                    {
                        Name = "",
                        Email = UserData.Email,
                        Password = UserData.Password,
                        Role = ""
                    };

                    try
                    {
                        string requestJson = JsonConvert.SerializeObject(UserData);
                        HttpContent httpContent = new StringContent(requestJson, Encoding.UTF8, "application/json");

                        var responseResult = await client.PostAsync("api/Admin/Login1", httpContent);

                        if (responseResult.IsSuccessStatusCode)
                        {
                            var responseContent = await responseResult.Content.ReadAsStringAsync();

                            // Ensure the response content is not empty before deserializing
                            if (!string.IsNullOrWhiteSpace(responseContent))
                            {
                                try
                                {
                                    var responseObject = JsonConvert.DeserializeObject<dynamic>(responseContent);


                                    // Check if responseObject is not null and status is "S"
                                    if (responseObject != null/* && responseObject.status == "S"*/)
                                    {
                                        responseObject.status = "S";
                                        string token = responseObject.access_token;

                                        // Store the token in session, local storage, or cookies as needed
                                        HttpContext.Session.SetString("UserToken", token);
                                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                                        this.response.Status = "S";
                                        this.response.Message = "Login successful.";
                                    }
                                    else
                                    {
                                        this.response.Status = "E";
                                        this.response.Message = responseObject?.message ?? "Login failed.";
                                    }
                                }
                                catch (Exception ex)
                                {
                                    this.response.Status = "E";
                                    this.response.Message = $"Deserialization error: {ex.Message}";
                                }
                            }
                            else
                            {
                                this.response.Status = "E";
                                this.response.Message = "Empty response from server.";
                            }
                        }
                        else
                        {
                            this.response.Status = "E";
                            this.response.Message = $"Error: {responseResult.ReasonPhrase}";
                        }
                    }
                    catch (Exception ex)
                    {
                        this.response.Status = "E";
                        this.response.Message = $"Exception: {ex.Message}";
                    }
                }
            }

            return Json(new { status = this.response.Status, success = this.response.Success, message = this.response.Message });
        }



        //[HttpPost("Admin/Login1")]
        //public async Task<IActionResult> Login1(AdminModel UserData)
        //{
        //    using (HttpClientHandler handler = new HttpClientHandler())
        //    {
        //        // Temporarily bypass SSL certificate validation (not for production use)
        //        handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;

        //        using (HttpClient client = new HttpClient(handler))
        //        { 
        //            client.BaseAddress = new Uri("https://localhost:7061/");

        //            UserData = new AdminModel
        //            {

        //                Name = "",
        //                Email = UserData.Email,
        //                Password = UserData.Password,
        //                Role = ""
        //            };

        //            try
        //            {
        //                string requestJson = JsonConvert.SerializeObject(UserData);
        //                HttpContent httpContent = new StringContent(requestJson, Encoding.UTF8, "application/json");

        //                var responseResult = await client.PostAsync("api/Admin/Login1", httpContent);

        //                if (responseResult.IsSuccessStatusCode)
        //                {
        //                    this.response = await responseResult.Content.ReadAsAsync<ResponseModel>();
        //                    ;
        //                    ////this.response = System.Text.Json.JsonSerializer.Deserialize<ResponseModel>(responseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        //                }
        //                else
        //                {
        //                    this.response.Status = "E";
        //                    this.response.Message = $"Error: {responseResult.StatusCode}";
        //                }
        //            }

        //            catch (Exception ex)
        //            {
        //                this.response.Status = "E";
        //                this.response.Message = ex.Message;
        //            }
        //        }
        //    }

        //    return Json(new { status = this.response.Status, success = this.response.Success, message = this.response.Message });

        //}

        [HttpPost("Admin/Register2")]
        public async Task<IActionResult>Register(AdminModel UserData)
        {
            using (HttpClientHandler handler = new HttpClientHandler())
            {
                // Temporarily bypass SSL certificate validation (not for production use)
                handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;

                using (HttpClient client = new HttpClient(handler))
                {
                    client.BaseAddress = new Uri("https://localhost:7061/");

                    try
                    {
                        string requestJson = JsonConvert.SerializeObject(UserData);
                        HttpContent httpContent = new StringContent(requestJson, Encoding.UTF8, "application/json");

                        var responseResult = await client.PostAsync("api/Admin/Register2", httpContent);

                        if (responseResult.IsSuccessStatusCode)
                        {
                            this.response = await responseResult.Content.ReadAsAsync<ResponseModel>();
                            ;
                            ////this.response = System.Text.Json.JsonSerializer.Deserialize<ResponseModel>(responseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                        }
                        else
                        {
                            this.response.Status = "E";
                            this.response.Message = $"Error: {responseResult.StatusCode}";
                        }
                    }

                    catch (Exception ex)
                    {
                        this.response.Status = "E";
                        this.response.Message = ex.Message;
                    }
                }
            }

            return Json(new { status = this.response.Status, success = this.response.Success, message = this.response.Message });

        }

        //public async Task<IActionResult> SearchUser(dataModel UserData)
        //{
        //    List<dataModel> users = new List<dataModel>();
        //    using (HttpClientHandler handler = new HttpClientHandler())
        //    {
        //        // Temporarily bypass SSL certificate validation (not for production use)
        //        handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;

        //        using (HttpClient client = new HttpClient(handler))
        //        {
        //            client.BaseAddress = new Uri("https://localhost:7061/");



        //            try
        //            {
        //                string requestJson = JsonConvert.SerializeObject(UserData);
        //                HttpContent httpContent = new StringContent(requestJson, Encoding.UTF8, "application/json");

        //                var responseResult = await client.PostAsync("api/Admin/SearchUser", httpContent);
        //                if (responseResult.IsSuccessStatusCode)
        //                {
        //                    users = await responseResult.Content.ReadAsAsync<List<dataModel>>();

        //                    this.response = System.Text.Json.JsonSerializer.Deserialize<ResponseModel>(responseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        //                }
        //                else
        //                {
        //                    this.response.Status = "E";
        //                    this.response.Message = $"Error: {responseResult.StatusCode}";
        //                }
        //            }
        //            catch (HttpRequestException httpEx)
        //            {
        //                // Log the HTTP request exception details
        //                Console.WriteLine($"HttpRequestException: {httpEx.Message}");
        //                this.response.Status = "E";
        //                this.response.Message = httpEx.Message;
        //            }
        //            catch (Exception ex)
        //            {
        //                // Log the general exception details
        //                Console.WriteLine($"Exception: {ex.Message}");
        //                this.response.Status = "E";
        //                this.response.Message = ex.Message;
        //            }
        //        }
        //    }

        //    return Json(new { status = this.response.Status, success = this.response.Success, message = this.response.Message });
        //}





        //public async Task<IActionResult> Hello(dataModel UserData)
        //{
        //    using (HttpClientHandler handler = new HttpClientHandler())
        //    {
        //        // Temporarily bypass SSL certificate validation (not for production use)
        //        handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;

        //        using (HttpClient client = new HttpClient(handler))
        //        {
        //            client.BaseAddress = new Uri("https://localhost:7061/");
        //            // Set a timeout for the request
        //            UserData = new dataModel
        //            {
        //                Name = "peemapi5",
        //                Age = "23" // Assuming age is an integer
        //            };
        //            try
        //            {
        //                 string requestJson = JsonConvert.SerializeObject(UserData);
        //                Console.WriteLine("Request JSON: " + requestJson);
        //                HttpContent httpContent = new StringContent(requestJson, Encoding.UTF8, "application/json");
                        
        //                var responseResult = await client.PostAsync("Admin/Hello", httpContent);
        //                if (responseResult.IsSuccessStatusCode)
        //                {
        //                    var responseString = await responseResult.Content.ReadAsStringAsync();
        //                    this.response = System.Text.Json.JsonSerializer.Deserialize<ResponseModel>(responseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        //                }
        //                else
        //                {
        //                    this.response.Status = "E";
        //                    this.response.Message = $"Error: {responseResult.StatusCode}";
        //                }
        //            }
        //            catch (HttpRequestException httpEx)
        //            {
        //                // Log the HTTP request exception details
        //                Console.WriteLine($"HttpRequestException: {httpEx.Message}");
        //                this.response.Status = "E";
        //                this.response.Message = httpEx.Message;
        //            }
        //            catch (Exception ex)
        //            {
        //                // Log the general exception details
        //                Console.WriteLine($"Exception: {ex.Message}");
        //                this.response.Status = "E";
        //                this.response.Message = ex.Message;
        //            }
        //        }
        //    }

        //    return Json(new { status = this.response.Status, success = this.response.Success, message = this.response.Message });
        //}

        [HttpPost("Logout")]
        public IActionResult Logout()
        {
            ResponseModel resp = new ResponseModel();
            // Clear the user's session
            HttpContext.Session.Clear();

            resp.Status = "S";
            resp.Message = "Log out Success";


            return Json(new { status = resp.Status, success = resp.Success, message = resp.Message });
        }
    }
    }


