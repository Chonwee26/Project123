using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Project123.Dto;
using System.Linq;
using System.Net.Http.Headers;
using static Project123.Services.IAuthenticationService;


namespace Project123.Controllers
{
    [Route("Base")]
    public class DataModel
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class BaseController : Controller
    {


        private readonly IHttpClientFactory _httpClientFactory;
        //private readonly ApiHelper _apiHelper;
        protected ResponseModel response;

        public BaseController(/*ApiHelper apiHelper,*/IHttpClientFactory httpClientFactory)
        {
            //_apiHelper = apiHelper;
            _httpClientFactory = httpClientFactory;
            response = new ResponseModel();
        }

      public override void OnActionExecuting(ActionExecutingContext context)
{
    var path = context.HttpContext.Request.Path.Value;

    // Exclude Login and Register pages from the check
    if (!string.IsNullOrEmpty(path) &&
        !path.Contains("LoginPage") && 
        !path.Contains("ForgetPasswordPage")&&
        !path.Contains("ChangePassword")&&
        !path.Contains("ForgetPassword")&&
        !path.Contains("RegisterPage") && 
        !path.Contains("Login1") && 
        !path.Contains("Register2"))
    {
        if (context.HttpContext != null) // Ensure HttpContext is not null
        {
            var session = context.HttpContext.Session;
            var cookies = context.HttpContext.Request.Cookies;

            if (session.GetString("UserId") == null && !cookies.ContainsKey("AuthToken"))
            {
                // Redirect to the Login page
                context.Result = new RedirectToActionResult("LoginPage", "Admin", null);
            }
        }
    }

    base.OnActionExecuting(context);
}

        // This method can be called from derived controllers
        protected async Task<IActionResult> GetProtectedData(string endpoint)
        {
            string token = HttpContext.Session.GetString("UserToken")??string.Empty;

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

        public string? UserId
        {
            get
            {
                return HttpContext.Session.GetString("UserId") ?? Request.Cookies["UserId"];
            }
        }
    }
}
