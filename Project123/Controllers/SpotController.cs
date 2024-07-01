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
        [HttpPost("Spot/CreateAlbum1")]
        public async Task<IActionResult> CreateAlbum(AlbumModel AlbumData)
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
                      
                        if (AlbumData.AlbumImage != null)
                        {
                            var (filePath, error, oldFolderPath) = await SaveFile(AlbumData.AlbumImage, AlbumData.ArtistName, AlbumData.AlbumName, AlbumData.AlbumId,AlbumData.AlbumImagePath);
                            if (error != null)
                            {
                                return Json(new { status = "E", success = false, message = error });
                            }
                            AlbumData.AlbumImagePath = filePath;
                        }

                        // Create a copy of SongData with nullified IFormFile properties for serialization
                        var AlbumDataCopy = new
                        {
                            AlbumData.AlbumId,
                            AlbumData.ArtistName,
                            AlbumData.AlbumName,
                            AlbumData.AlbumImagePath
                            

                        };

                        string requestJson = JsonConvert.SerializeObject(AlbumDataCopy);
                        HttpContent httpContent = new StringContent(requestJson, Encoding.UTF8, "application/json");

                        var responseResult = await client.PostAsync("api/Spot/CreateAlbum123", httpContent);
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

        //[HttpPost("Spot/CreateAlbum123")]
        //public async Task<IActionResult> CreateAlbumId(AlbumModel AlbumData)
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
        //                // Step 1: Create album entry in the database to generate the AlbumId
        //                var initialAlbumData = new
        //                {
        //                    AlbumData.ArtistName,
        //                    AlbumData.AlbumName
        //                };

        //                string initialRequestJson = JsonConvert.SerializeObject(initialAlbumData);
        //                HttpContent initialHttpContent = new StringContent(initialRequestJson, Encoding.UTF8, "application/json");

        //                var initialResponseResult = await client.PostAsync("api/Spot/CreateAlbumInitial", initialHttpContent);
        //                if (!initialResponseResult.IsSuccessStatusCode)
        //                {
        //                    return Json(new { status = "E", success = false, message = $"Error: {initialResponseResult.StatusCode}" });
        //                }

        //                var initialResponse = await initialResponseResult.Content.ReadAsAsync<ResponseModel<AlbumModel>>();
        //                AlbumData.AlbumId = initialResponse.Data.AlbumId;

        //                // Step 2: Save files and update paths in AlbumData
        //                if (AlbumData.AlbumImage != null)
        //                {
        //                    var (filePath, error, oldFolderPath) = await SaveFile(AlbumData.AlbumImage, AlbumData.ArtistName, AlbumData.AlbumName, AlbumData.AlbumId, AlbumData.AlbumImagePath);
        //                    if (error != null)
        //                    {
        //                        return Json(new { status = "E", success = false, message = error });
        //                    }
        //                    AlbumData.AlbumImagePath = filePath;
        //                }

        //                // Create a copy of AlbumData with nullified IFormFile properties for serialization
        //                var AlbumDataCopy = new
        //                {
        //                    AlbumData.AlbumId,
        //                    AlbumData.ArtistName,
        //                    AlbumData.AlbumName,
        //                    AlbumData.AlbumImagePath
        //                };

        //                string requestJson = JsonConvert.SerializeObject(AlbumDataCopy);
        //                HttpContent httpContent = new StringContent(requestJson, Encoding.UTF8, "application/json");

        //                // Step 3: Finalize album creation with file paths
        //                var responseResult = await client.PostAsync("api/Spot/CreateAlbumFinalize", httpContent);
        //                if (responseResult.IsSuccessStatusCode)
        //                {
        //                    this.response = await responseResult.Content.ReadAsAsync<ResponseModel>();
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
                            var (filePath, error, oldFolderPath) = await SaveFile(SongData.SongFile, SongData.ArtistName, SongData.SongName, SongData.AlbumId, SongData.SongFilePath);
                            if (error != null)
                            {
                                return Json(new { status = "E", success = false, message = error });
                            }

                            SongData.SongFilePath = filePath;
                        }

                        if (SongData.SongImage != null)
                        {
                            var (filePath, error, oldFolderPath) = await SaveFile(SongData.SongImage, SongData.ArtistName, SongData.SongName, SongData.AlbumId,SongData.SongImagePath);
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
                            SongData.SongName,
                            SongData.SongLength
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


        [HttpPost("Spot/UpdateSong1")]
        public async Task<IActionResult> UpdateSong(SongModel SongData)
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
                            var (filePath, error, oldFolderPath) = await SaveFile(SongData.SongFile, SongData.ArtistName, SongData.SongName, SongData.AlbumId, SongData.SongFilePath);
                            if (error != null)
                            {
                                return Json(new { status = "E", success = false, message = error });
                            }

                            SongData.SongFilePath = filePath;
                        }

                        if (SongData.SongImage != null)
                        {
                            var (filePath, error, oldFolderPath) = await SaveFile(SongData.SongImage, SongData.ArtistName, SongData.SongName, SongData.AlbumId, SongData.SongImagePath);
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
                            SongData.SongName,
                            SongData.SongLength
                        };

                        string requestJson = JsonConvert.SerializeObject(songDataCopy);
                        HttpContent httpContent = new StringContent(requestJson, Encoding.UTF8, "application/json");

                        var responseResult = await client.PostAsync("api/Spot/UpdateSong", httpContent);
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


        [HttpPost("Spot/DeleteSong1")]

        public async Task<IActionResult> DeleteSong(SongModel SongData)
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

                    var (filePath, error, oldFolderPath) = await DeleteFile(SongData.SongFile, SongData.ArtistName, SongData.SongName, SongData.AlbumId, SongData.SongFilePath);
                    if (error != null)
                    {
                        return Json(new { status = "E", success = false, message = error });
                    }
                  

                    string requestJson = JsonConvert.SerializeObject(SongData);
                    HttpContent httpContent = new StringContent(requestJson, Encoding.UTF8, "application/json");

                    var responseResult = await client.PostAsync("api/Spot/DeleteSong", httpContent);
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

            return Json(new { status = this.response.Status, success = this.response.Success, message = this.response.Message });
        }

        private async Task<(string filePath, string error, string oldFolderPath)> SaveFile(IFormFile file, string artistName, string name, int? albumId, string existingFilePath)
        {
            if (file == null || file.Length == 0)
            {
                return (null, "File is empty", null);
            }

            // Sanitize artist name and song name for use in file paths
            var sanitizedArtistName = SanitizeFileName(artistName);
            var sanitizedName = SanitizeFileName(name);

            // Create the directory path for the artist and song
            var artistFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", sanitizedArtistName);
            var albumFolderPath = "";

            if (albumId != null)
            {
                albumFolderPath = Path.Combine(artistFolderPath, "Album_" + albumId.ToString());
            }
            else
            {
                albumFolderPath = Path.Combine(artistFolderPath, sanitizedName);
            }

            // Ensure the directories exist
            Directory.CreateDirectory(albumFolderPath);

            // Variable to store the old folder path
         

            // Generate a unique file name and get the full file path
            var fileName = file.ContentType == "audio/mpeg" ? $"{name}.mp3" : file.FileName;
            var filePath = Path.Combine(albumFolderPath, fileName);

            var existingFiles = Directory.GetFiles(albumFolderPath, fileName);
            if (existingFiles.Length > 0)
            {
                return (null, "Song already exists", null);
            }
            string oldFolderPath = null;

            // Check if there's an existing file and delete it
            if (!string.IsNullOrEmpty(existingFilePath))
            {
                var fullExistingFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", existingFilePath.TrimStart('/').Replace("/", "\\"));
                if (System.IO.File.Exists(fullExistingFilePath))
                {
                    oldFolderPath = Path.GetDirectoryName(fullExistingFilePath);
                    if (albumId != null)
                    {


                        // Delete the existing file
                        System.IO.File.Delete(fullExistingFilePath);
                    }
                    // Get the old folder path
                    else
                    {
                        System.IO.File.Delete(fullExistingFilePath);
                        //System.IO.Directory.Delete(oldFolderPath);
                    }

                }
            }
            // Save the file to the generated path
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Move file to album if AlbumId is provided
            if (albumId != null)
            {
                var (newFilePath, moveError) = MoveFileToAlbum(filePath, artistFolderPath, sanitizedName, albumId.Value);
                if (moveError != null)
                {
                    return (null, moveError, oldFolderPath);
                }
                filePath = newFilePath;
            }

            // Return the relative path to the saved file
            return (Path.Combine("/uploads", sanitizedArtistName, albumId != null ? $"Album_{albumId}" : sanitizedName, fileName).Replace("\\", "/"), null, oldFolderPath);
        }


        private async Task<(string filePath, string error, string oldFolderPath)> DeleteFile(IFormFile file, string artistName, string name, int? albumId, string existingFilePath)
        {
            

            // Sanitize artist name and song name for use in file paths
            var sanitizedArtistName = SanitizeFileName(artistName);
            var sanitizedName = SanitizeFileName(name);

            // Create the directory path for the artist and song
            var artistFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", sanitizedArtistName);
            var albumFolderPath = "";

            if (albumId != null)
            {
                albumFolderPath = Path.Combine(artistFolderPath, "Album_" + albumId.ToString());
            }
            else
            {
                albumFolderPath = Path.Combine(artistFolderPath, sanitizedName);
            }

            // Ensure the directories exist
            Directory.CreateDirectory(albumFolderPath);

            // Variable to store the old folder path
            string oldFolderPath = null;

            // Check if there's an existing file and delete it
            if (!string.IsNullOrEmpty(existingFilePath))
            {
                var fullExistingFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", existingFilePath.TrimStart('/').Replace("/", "\\"));
                if (System.IO.File.Exists(fullExistingFilePath))
                {
                    oldFolderPath = Path.GetDirectoryName(fullExistingFilePath);

                    // Delete the existing file
                    System.IO.File.Delete(fullExistingFilePath);
                }
            }

            // Return the old folder path
            return (null, null, oldFolderPath);
        }


        // Helper method to move file to album
        private (string newFilePath, string error) MoveFileToAlbum(string filePath, string artistFolderPath, string sanitizedName, int albumId)
        {
            try
            {
                var albumFolderPath = Path.Combine(artistFolderPath, "Album_" + albumId.ToString());
                Directory.CreateDirectory(albumFolderPath);
                var newFilePath = Path.Combine(albumFolderPath, Path.GetFileName(filePath));
                System.IO.File.Move(filePath, newFilePath);
                return (newFilePath, null);
            }
            catch (Exception ex)
            {
                return (null, $"Error moving file to album: {ex.Message}");
            }
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




        [HttpPost("Spot/SearchSong1")]
        public async Task<IActionResult> SearchSong(SongModel SongData)
        {
            ResponseModel resp = new ResponseModel();
            List<SongModel> SongList = new List<SongModel>();
            using (HttpClientHandler handler = new HttpClientHandler())
            {
                // Temporarily bypass SSL certificate validation (not for production use)
                handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;

                var client = _httpClientFactory.CreateClient();
                client.BaseAddress = new Uri("https://localhost:7061/");


                try
                {
                    string requestJson = JsonConvert.SerializeObject(SongData);
                    HttpContent httpContent = new StringContent(requestJson, Encoding.UTF8, "application/json");

                    var response = await client.PostAsync("/api/Spot/SearchSong", httpContent);
                    if (response.IsSuccessStatusCode)
                    {
                        SongList = await response.Content.ReadAsAsync<List<SongModel>>();

                        if (SongList.Count > 0)
                        {
                            resp.Status = "S";
                            resp.Message = "Success";
                        }

                        else
                        {
                            resp.Status = "E";
                            resp.Message = $"Error:";
                        }


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

            return Json(new { status = resp.Status, success = resp.Success, message = resp.Message, Data = SongList });
        }


        [HttpPost("Spot/GetAlbum1")]
        public async Task<IActionResult> GetAlbum(AlbumModel albumData)
        {
            List<AlbumModel> albumList = new List<AlbumModel>();
            
            using (HttpClientHandler handler = new HttpClientHandler())
            {
                // Temporarily bypass SSL certificate validation (not for production use)
                handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;

                var client = _httpClientFactory.CreateClient();
                client.BaseAddress = new Uri("https://localhost:7061/");
                try
                {

                    string requestJson = JsonConvert.SerializeObject(albumData);
                    HttpContent httpContent = new StringContent(requestJson, Encoding.UTF8, "application/json");

                    var response = await client.PostAsync("/api/Spot/GetAlbum", httpContent);
                    if (response.IsSuccessStatusCode)
                    {
                        albumList = await response.Content.ReadAsAsync<List<AlbumModel>>();

                        if (albumList.Count > 0)
                        {
                            this.response.Status = "S";
                            this.response.Message = "Success";
                        }

                        else
                        {
                            this.response.Status = "E";
                            this.response.Message = $"Error:";
                        }
                    }
                    else
                    {
                        this.response.Status = "E";
                        this.response.Message = $"Error:";
                    }
                }


                catch (HttpRequestException ex)
                {
                    this.response.Status = "E";
                    this.response.Message = ex.Message;
                }
            }
            return Json(new { success = this.response.Success, message = this.response.Message, Data = albumList });
        }

        [HttpPost("Spot/SearchAlbum1")]
        public async Task<IActionResult> SearchAlbum(AlbumModel AlbumData)
        {
            ResponseModel resp = new ResponseModel();
            List<AlbumModel> AlbumList = new List<AlbumModel>();
            using (HttpClientHandler handler = new HttpClientHandler())
            {
                // Temporarily bypass SSL certificate validation (not for production use)
                handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;

                var client = _httpClientFactory.CreateClient();
                client.BaseAddress = new Uri("https://localhost:7061/");


                try
                {
                    string requestJson = JsonConvert.SerializeObject(AlbumData);
                    HttpContent httpContent = new StringContent(requestJson, Encoding.UTF8, "application/json");

                    var response = await client.PostAsync("/api/Spot/SearchAlbum", httpContent);
                    if (response.IsSuccessStatusCode)
                    {
                        AlbumList = await response.Content.ReadAsAsync<List<AlbumModel>>();

                        if (AlbumList.Count > 0)
                        {
                            resp.Status = "S";
                            resp.Message = "Success";
                        }

                        else
                        {
                            resp.Status = "E";
                            resp.Message = $"Error:";
                        }


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

            return Json(new { status = resp.Status, success = resp.Success, message = resp.Message, Data = AlbumList });
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
