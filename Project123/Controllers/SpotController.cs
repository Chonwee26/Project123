using Microsoft.AspNetCore.Mvc;
using Project123.Dto;
using Newtonsoft.Json;
using System.Text;

namespace Project123.Controllers
{
    public class SpotController : BaseController
    {

        private readonly IHttpClientFactory _httpClientFactory;
        
        public SpotController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult SpotAdminPage()
        {
            return View();
        }

        [HttpPost("Admin/CreateUser123")]
        public async Task<IActionResult> CreateSong(SongModel SongData)
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
                        string requestJson = JsonConvert.SerializeObject(SongData);
                        HttpContent httpContent = new StringContent(requestJson, Encoding.UTF8, "application/json");
                        if (SongData.SongFile != null)
                        {
                            SongData.SongFilePath = await SaveFile(SongData.SongFile);
                        }
                        if (SongData.SongImage != null)
                        {
                            SongData.SongImagePath = await SaveFile(SongData.SongImage);
                        }
                        var responseResult = await client.PostAsync("api/Spot/CreateSong", httpContent);
                        if (responseResult.IsSuccessStatusCode)
                        {
                            this.response = await responseResult.Content.ReadAsAsync<ResponseModel>();

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
        private async Task<string> SaveFile(IFormFile file)
        {
            if (file == null || file.Length == 0) return null;

            var filePath = Path.Combine("uploads", file.FileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            return filePath;
        }

        //public async Task<IActionResult> CreateSong(SongModel SongData)
        //{
        //    using (HttpClientHandler handler = new HttpClientHandler())
        //    {
        //        // Temporarily bypass SSL certificate validation (not for production use)
        //        handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;

        //        using (HttpClient client = new HttpClient(handler))
        //        {
        //            client.BaseAddress = new Uri("https://localhost:7061/");



        //            try
        //            {
        //                string requestJson = JsonConvert.SerializeObject(SongData);
        //                HttpContent httpContent = new StringContent(requestJson, Encoding.UTF8, "application/json");

        //                var responseResult = await client.PostAsync("api/Spot/CreateSong", httpContent);
        //                if (responseResult.IsSuccessStatusCode)
        //                {
        //                    this.response = await responseResult.Content.ReadAsAsync<ResponseModel>();

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
    }
}
