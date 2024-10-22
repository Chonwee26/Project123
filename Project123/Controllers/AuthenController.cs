using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Project123.Dto;
using static Project123.Services.IAuthenticationService;

namespace Project123.Controllers
{
    [Route("Authen")]
    public class AuthenController : BaseController
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ApiHelper _apiHelper;
        public AuthenController(ApiHelper apiHelper, IHttpClientFactory httpClientFactory) : base(httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
            _apiHelper = apiHelper;
        }

        [HttpPost("Login1")]
        public async Task<IActionResult> Login(AdminModel UserData)
        {
            ResponseModel response = new ResponseModel();

         
                try
                {
                    string requestJson = JsonConvert.SerializeObject(UserData);
                    var client = _apiHelper.CreateClient();
                    client.BaseAddress = new Uri("https://localhost:7061/"); // Ensure the base address is set
              
                    HttpContent httpContent = new StringContent(requestJson, Encoding.UTF8, "application/json");
                   
                    var responseResult = await client.PostAsync("api/Authen/Login", httpContent);

                    if (responseResult.IsSuccessStatusCode)
                    {
                        response = await responseResult.Content.ReadAsAsync<ResponseModel>();
                        string tokenInfo = response.user_roles;
                        string token = response.access_token;
                        HttpContext.Session.SetString("UserTokenInfo", tokenInfo);
                        HttpContext.Session.SetString("UserToken", token);
                        // STEP 1: Pass the JWT token in authorization header for subsequent requests
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);


                        response.Status = "S";
                        response.Message = "Login successful.";
                    }
                    else
                    {
                        response.Status = "E";
                        response.Message = "Login failed.";
                    }
                }
                catch (Exception ex)
                {
                    response.Status = "E";
                    response.Message = ex.Message;
                }
            

            return Json(new { status = response.Status, success = response.Success, message = response.Message });
        }

        [HttpPost("Register2")]
        public async Task<IActionResult> Register(AdminModel UserData)
        {
            ResponseModel response = new ResponseModel();

            using (HttpClientHandler handler = new HttpClientHandler())
            {
                handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
                var client = _httpClientFactory.CreateClient();
                client.BaseAddress = new Uri("https://localhost:7061/");

                try
                {
                    string requestJson = JsonConvert.SerializeObject(UserData);
                    HttpContent httpContent = new StringContent(requestJson, Encoding.UTF8, "application/json");

                    var responseResult = await client.PostAsync("api/Authen/Register", httpContent);

                    if (responseResult.IsSuccessStatusCode)
                    {
                        response = await responseResult.Content.ReadAsAsync<ResponseModel>();
                    }
                    else
                    {
                        response.Status = "E";
                        response.Message = $"Error: {responseResult.StatusCode}";
                    }
                }
                catch (Exception ex)
                {
                    response.Status = "E";
                    response.Message = ex.Message;
                }
            }

            return Json(new { status = response.Status, success = response.Success, message = response.Message });
        }

        [HttpPost("Logout1")]
        public IActionResult Logout()
        {
            ResponseModel response = new ResponseModel();

            // Clear the user's session
            HttpContext.Session.Clear();
            response.Status = "S";
            response.Message = "Log out Success";

            return Json(new { status = response.Status, success = response.Success, message = response.Message });
        }
    }
}
