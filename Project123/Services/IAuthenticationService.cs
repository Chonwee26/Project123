using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Net.Http.Headers;

namespace Project123.Services
{
    public class IAuthenticationService : Controller
    {
        public class ApiHelper
        {
            private readonly IHttpContextAccessor _httpContextAccessor;

            public ApiHelper(IHttpContextAccessor httpContextAccessor)
            {
                _httpContextAccessor = httpContextAccessor;
            }

            public HttpClient CreateClient()
            {
                var client = new HttpClient();
                var token = _httpContextAccessor.HttpContext?.Session.GetString("UserToken") ?? string.Empty;
                if (!string.IsNullOrEmpty(token))
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }
                return client;
            }
        }

        public class MyService
        {
            private readonly ApiHelper _apiHelper;

            public MyService(ApiHelper apiHelper)
            {
                _apiHelper = apiHelper;
            }

            public async Task CallApiAsync()
            {
                var client = _apiHelper.CreateClient();
                var response = await client.GetAsync("https://api.example.com/data");

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    // Process result
                }
                else
                {
                    // Handle error
                }
            }
        }
    }
}
