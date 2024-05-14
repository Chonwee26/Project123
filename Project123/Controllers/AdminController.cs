using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Project123.Dto;
using System.Text;
namespace Project123.Controllers
{
    public class AdminController : BaseController
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IAuthenticationService _authenSvc;
        public AdminController(IHttpClientFactory clientFactory, IAuthenticationService authenticationService)
        {
            _clientFactory = clientFactory;
            _authenSvc = authenticationService;
        }
        public  IActionResult Index()
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
            using (var client = _clientFactory.CreateClient("BaseClient"))
            {
                try
                {
                    string requestJson = JsonConvert.SerializeObject(UserData);
                    HttpContent httpContent = new StringContent(requestJson, Encoding.UTF8, "application/json");

                    var responseResult = await client.PostAsync("Admin/CreateUser", httpContent);
                    if (responseResult.IsSuccessStatusCode)
                    {
                        //this.response = await responseResult.Content.ReadAsAsync<ResponseModel>();
                    }
                }
                catch (HttpRequestException ex)
                {
                    this.response.Status = "E";
                    this.response.Message = ex.Message;
                }
            }

            return Json(new { status = this.response.Status, success = this.response.Success, message = this.response.Message });
        }
    }
}
