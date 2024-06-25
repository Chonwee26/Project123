using Microsoft.AspNetCore.Mvc;
using Project123.Dto;
using Newtonsoft.Json;
using System.Text;
using Microsoft.Extensions.Logging;


namespace Project123.Controllers
{
    public class SpotController : BaseController
    {

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<SpotController> _logger;
        public SpotController(IHttpClientFactory httpClientFactory, ILogger<SpotController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult SpotAdminPage()
        {
            return View();
        }

        //[HttpPost("Spot/CreateSong1")]
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
        //                if (SongData.SongFile != null)
        //                {
        //                    SongData.SongFilePath = await SaveFile(SongData.SongFile);
        //                }
        //                if (SongData.SongImage != null)
        //                {
        //                    SongData.SongImagePath = await SaveFile(SongData.SongImage);
        //                }

        //                //SongData = new SongModel
        //                //{
        //                //    AlbumId = SongData.AlbumId,
        //                //    ArtistName = SongData.ArtistName,
        //                //    SongFile = null,
        //                //    SongFilePath = SongData.SongFilePath,
        //                //    SongGenres = SongData.SongGenres,
        //                //    SongId = 1,
        //                //    SongImage = null,
        //                //    SongImagePath = SongData.SongImagePath,
        //                //    SongName = SongData.SongName
        //                //};
        //                var responseResult = await client.PostAsync("api/Spot/CreateSong123", httpContent);
        //                if (responseResult.IsSuccessStatusCode)
        //                {
        //                    _logger.LogInformation("CreateSong method called successfully");
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

        //                _logger.LogError(ex, "An error occurred in CreateSong method");
        //                this.response.Status = "E";
        //                this.response.Message = ex.Message;
        //            }
        //        }
        //    }

        //    return Json(new { status = this.response.Status, success = this.response.Success, message = this.response.Message });
        //}

        [HttpPost("Spot/CreateSong1")]
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
                        // Save files and update paths in SongData
                        if (SongData.SongFile != null)
                        {
                            var (filePath, error) = await SaveFile(SongData.SongFile, SongData.ArtistName, SongData.SongName);
                            if (error != null)
                            {
                                return Json(new { status = "E", success = false, message = error });
                            }
                            SongData.SongFilePath = filePath;
                        }

                        if (SongData.SongImage != null)
                        {
                            var (filePath, error) = await SaveFile(SongData.SongImage, SongData.ArtistName, SongData.SongName);
                            if (error != null)
                            {
                                return Json(new { status = "E", success = false, message = error });
                            }
                            SongData.SongImagePath = filePath;
                        }

                        // Create a copy of SongData with nullified IFormFile properties for serialization
                        var songDataCopy = new
                        {
                            SongData.AlbumId,
                            SongData.ArtistName,
                            SongData.SongFilePath,
                            SongData.SongGenres,
                            SongData.SongId,
                            SongData.SongImagePath,
                            SongData.SongName
                        };

                        string requestJson = JsonConvert.SerializeObject(songDataCopy);
                        HttpContent httpContent = new StringContent(requestJson, Encoding.UTF8, "application/json");

                        var responseResult = await client.PostAsync("api/Spot/CreateSong123", httpContent);
                        if (responseResult.IsSuccessStatusCode)
                        {
                            this.response = await responseResult.Content.ReadAsAsync<ResponseModel>();
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



        private async Task<(string filePath, string error)> SaveFile(IFormFile file, string artistName, string songName)
        {
            if (file == null || file.Length == 0)
            {
                return (null, "File is empty");
            }

            // Sanitize artist name and song name for use in file paths
            var sanitizedArtistName = SanitizeFileName(artistName);
            var sanitizedSongName = SanitizeFileName(songName);

            // Create the directory path for the artist and song
            var artistFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", sanitizedArtistName);
            var songFolderPath = Path.Combine(artistFolderPath, sanitizedSongName);

            // Ensure the directories exist
            Directory.CreateDirectory(songFolderPath);

            // Check for duplicate file
            var existingFiles = Directory.GetFiles(songFolderPath, file.FileName);
            if (existingFiles.Length > 0)
            {
                return (null, "Song already exists");
            }

            // Generate a unique file name and get the full file path
            var uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
            var filePath = Path.Combine(songFolderPath, uniqueFileName);

            // Save the file to the generated path
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Return the relative path to the saved file
            return (Path.Combine("/uploads", sanitizedArtistName, sanitizedSongName, uniqueFileName).Replace("\\", "/"), null);
        }

        // Helper method to sanitize file names
        private string SanitizeFileName(string name)
        {
            foreach (var c in Path.GetInvalidFileNameChars())
            {
                name = name.Replace(c, '_');
            }
            return name;
        }




        //private async Task<string> SaveFile(IFormFile file)
        //{
        //    if (file == null || file.Length == 0) return null;

        //    // Define the path to the uploads directory
        //    var uploadDirectory = Path.Combine("D:\\git\\Project123\\Project123\\uploads");

        //    // Check if the directory exists, if not, create it
        //    if (!Directory.Exists(uploadDirectory))
        //    {
        //        Directory.CreateDirectory(uploadDirectory);
        //    }

        //    // Combine the upload directory with the file name
        //    var filePath = Path.Combine(uploadDirectory, file.FileName);

        //    // Save the file to the specified path
        //    using (var stream = new FileStream(filePath, FileMode.Create))
        //    {
        //        await file.CopyToAsync(stream);
        //    }

        //    return filePath;
        //}


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
