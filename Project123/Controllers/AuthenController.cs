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
                        string tokenUserId = response.user_id;
                        string token = response.access_token;
                    if (UserData.RememberMe)
                    {
                        if (response.Success)
                        {
                            HttpContext.Response.Cookies.Append("UserId", tokenUserId, new CookieOptions
                            {
                                Expires = DateTimeOffset.Now.AddDays(1),
                                HttpOnly = true,
                                IsEssential = true,
                                SameSite = SameSiteMode.None, // Allows cross-site usage if needed
                                Secure = true // Required if SameSite is None; ensures it's only sent over HTTPS
                            });



                            // Issue a persistent cookie with a longer expiration
                            HttpContext.Response.Cookies.Append("AuthToken", token, new CookieOptions
                            {
                                HttpOnly = true,
                                Secure = true,
                                SameSite = SameSiteMode.Lax,
                                Expires = DateTime.UtcNow.AddDays(1) // Adjust expiration as needed
                            });
                        }
                       
                    }
                    else
                    {
                        //// Session-only cookie (no explicit expiration)
                        //HttpContext.Response.Cookies.Append("AuthToken", token.AccessToken, new CookieOptions
                        //{
                        //    HttpOnly = true,
                        //    Secure = true
                        //});
                        if (response.Success)
                        {

                            HttpContext.Session.SetString("UserToken", token);

                            HttpContext.Session.SetString("UserId", tokenUserId);
                        }

                    }
                    if (response.Success)
                    {
                        HttpContext.Session.SetString("UserTokenInfo", tokenInfo);

                    }



                    // STEP 1: Pass the JWT token in authorization header for subsequent requests
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);


                        //response.Status = "S";
                        //response.Message = "Login successful.";
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


        [HttpPost("ChangePassword1")]
        public async Task<IActionResult> ChangePassword(AdminModel UserData)
        {
            ResponseModel response = new ResponseModel();

            try
            {
                string requestJson = JsonConvert.SerializeObject(UserData);
                var client = _apiHelper.CreateClient();
                client.BaseAddress = new Uri("https://localhost:7061/"); // Ensure the base address is set

                HttpContent httpContent = new StringContent(requestJson, Encoding.UTF8, "application/json");

                var responseResult = await client.PostAsync("api/Authen/ChangePassword", httpContent);

                if (responseResult.IsSuccessStatusCode)
                {
                    response = await responseResult.Content.ReadAsAsync<ResponseModel>();           
                }
               
            }
            catch (Exception ex)
            {
                response.Status = "E";
                response.Message = ex.Message;
            }


            return Json(new { status = response.Status, success = response.Success, message = response.Message });
        }


        [HttpPost("ChangePasswordByToken1")]
        public async Task<IActionResult> ChangePasswordByToken(AdminModel UserData , string token)
        {
            ResponseModel response = new ResponseModel();

            try
            {
                string requestJson = JsonConvert.SerializeObject(UserData);
                var client = _apiHelper.CreateClient();
                client.BaseAddress = new Uri("https://localhost:7061/"); // Ensure the base address is set

                HttpContent httpContent = new StringContent(requestJson, Encoding.UTF8, "application/json");

                var responseResult = await client.PostAsync($"api/Authen/ChangePasswordByToken?token={token}", httpContent);

                if (responseResult.IsSuccessStatusCode)
                {
                    response = await responseResult.Content.ReadAsAsync<ResponseModel>();
                }

            }
            catch (Exception ex)
            {
                response.Status = "E";
                response.Message = ex.Message;
            }


            return Json(new { status = response.Status, success = response.Success, message = response.Message });
        }

        [HttpPost("ForgetPassword1")]
        public async Task<IActionResult> ForgetPassword(AdminModel UserData)
        {
            ResponseModel response = new ResponseModel();

            try
            {
                string requestJson = JsonConvert.SerializeObject(UserData);
                var client = _apiHelper.CreateClient();
                client.BaseAddress = new Uri("https://localhost:7061/"); // Ensure the base address is set

                HttpContent httpContent = new StringContent(requestJson, Encoding.UTF8, "application/json");

                var responseResult = await client.PostAsync("api/Authen/ForgetPassword", httpContent);

                if (responseResult.IsSuccessStatusCode)
                {
                    response = await responseResult.Content.ReadAsAsync<ResponseModel>();
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
            ResponseModel resp = new ResponseModel();
            // Clear the user's session
            HttpContext.Session.Clear();



            // Delete specific cookies
            var cookieNamesToDelete = new List<string> { "UserId", "UserToken", "UserTokenInfo", "AuthToken" };

            foreach (var cookieName in cookieNamesToDelete)
            {
                if (Request.Cookies[cookieName] != null)
                {
                    Response.Cookies.Delete(cookieName);
                }
            }



            //// Loop through all cookies in the request and delete them
            //foreach (var cookie in Request.Cookies)
            //{
            //    Response.Cookies.Delete(cookie.Key);
            //}


            resp.Status = "S";
            resp.Message = "Log out Success";


            return Json(new { status = resp.Status, success = resp.Success, message = resp.Message });
        }
    }
}
