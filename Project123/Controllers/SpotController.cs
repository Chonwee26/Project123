using Microsoft.AspNetCore.Mvc;
using Project123.Dto;
using Newtonsoft.Json;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Project123Api.Repositories;
using Microsoft.AspNetCore.Authorization;
using System.Net.Http.Headers;
using static Project123.Services.IAuthenticationService;
using System.Net.Http;
using Xunit;
using Moq;
using Moq.Protected;
using System.Net;


namespace Project123.Controllers
{
  
    public class SpotController : BaseController
    {
        private readonly DataDbContext _db;
        //private string? _connectionString; // Make the field nullable
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly MyService _myService;
        private readonly ApiHelper _apiHelper;
        private readonly ILogger<SpotController> _logger;


        public SpotController(ApiHelper apiHelper, MyService myService, IHttpClientFactory httpClientFactory, ILogger<SpotController> logger, DataDbContext db):base(httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
             _myService = myService;  // Inject MyService
            _apiHelper = apiHelper;
            _logger = logger;
            _db = db;
        }
      
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> SpotAdminPage()
        {
            List<GenreModel> genreList = new List<GenreModel>();
            using (HttpClientHandler handler = new HttpClientHandler())
            {
                // Temporarily bypass SSL certificate validation (not for production use)
                handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
                var client = _apiHelper.CreateClient();
                client.BaseAddress = new Uri("https://localhost:7061/");
                //using (var client = _httpClientFactory.CreateClient("BaseClient"))
                try
                {
                    var responseG = await client.GetAsync("/api/Spot/GetGenre");
                    if (responseG.IsSuccessStatusCode)
                    {
                        genreList = await responseG.Content.ReadAsAsync<List<GenreModel>>();
                    }
                 
                }
                catch (HttpRequestException ex)
                {
                    this.response.Status = "E";
                    this.response.Message = ex.Message;
                }
            }
            ViewBag.GenreList = genreList;
            return View();                    
        }


        public IActionResult SpotProfilePage()
        {

            var userId = Convert.ToInt32(HttpContext.Session.GetString("UserId") ?? Request.Cookies["UserId"]);
            if (userId <= 0)
            {
                return Json(new { status = "E", success = false, message = "Could not find this User" });
            }

            // Check if _db.Song is null before querying
            if (_db.Tb_Admin == null || !_db.Tb_Admin.Any())
            {
                return Json(new { status = "E", success = false, message = "No User available in the database" });
            }

            var user = _db.Tb_Admin
                            .Where(s => s.Id == userId)
                            .Select(s => new AdminModel
                            {
                                Id = s.Id,
                                Name = s.Name,
                                Email = s.Email,
                                Age = s.Age,
                                Role = s.Role,
                                ProfileImagePath = s.ProfileImagePath,


                            })
                            .FirstOrDefault(s => s.Id == userId);

            if (user == null) // Correct condition to check if the artist data is found
            {
                return Json(new { status = "E", success = false, message = "user not found" });
            }

            return View("SpotProfilePage", user);


            //  return View();
        }

        public IActionResult FavoriteSong()
        {
            
            var userId = HttpContext.Session.GetString("UserId") ?? Request.Cookies["UserId"];
            ViewBag.UserId = Convert.ToInt32(userId);
            return View();
        }

        public IActionResult Search()
        {
            return View();
        }

        public IActionResult Genre(int genreId, bool showall = false)
        {
            if (genreId <= 0)
            {
                return Json(new { status = "E", success = false, message = "Could not find this genre" });
            }

            // Check if _db.Genre is null or empty
            if (_db.Genre == null || !_db.Genre.Any())
            {
                return Json(new { status = "E", success = false, message = "No genre available in the database" });
            }

            // Handle the "showall" functionality
            if (showall)
            {
                // Fetch all items related to this genre
                var genreItems = _db.Albums?
                                    .Where(s => s.AlbumGenre == genreId)
                                    .Select(s => new AlbumModel
                                    {
                                        
                                        AlbumId = s.AlbumId,
                                        AlbumName = s.AlbumName,
                                        ArtistName = s.ArtistName, // Assuming you have navigation properties
                                        AlbumImagePath = s.AlbumImagePath,
                                        AlbumGenre = s.AlbumGenre
                                    })
                                    .ToList();
                // test  การจอย table ด้วย linq สามารถ ทำ album แล้ว มีหลาย table ได้ไหม

                var testresult = (from song in _db.Song
                              join album in _db.Albums on song.AlbumId equals album.AlbumId
                              join artist in _db.Artist on song.ArtistName equals artist.ArtistName
                              where album.AlbumGenre == genreId
                              select new AlbumModel
                              {
                                  AlbumId = album.AlbumId,
                                  AlbumName = album.AlbumName,
                                  ArtistName = artist.ArtistName,
                                  AlbumImagePath = album.AlbumImagePath,
                                  AlbumGenre = album.AlbumGenre
                              }).ToList();



                if (genreItems == null || !genreItems.Any())
                {
                    return Json(new { status = "E", success = false, message = "No Album found for this genre" });
                }

                return View("ShowAll", genreItems); // A dedicated "ShowAll" view to display all items
              //  return View("ShowAll",  testresult); // A dedicated "ShowAll" view to display all items
            }

            // Default behavior to fetch genre details
            var genre = _db.Genre?
                            .Where(s => s.GenreId == genreId)
                            .Select(s => new GenreModel
                            {
                                GenreId = s.GenreId,
                                GenreName = s.GenreName,
                                GenreImagePath = s.GenreImagePath,
                            })
                            .FirstOrDefault();

            if (genre == null)
            {
                return Json(new { status = "E", success = false, message = "Genre not found" });
            }

            return View("Genre", genre);
        }


        //public IActionResult Genre(int genreId)
        //{
        //    if (genreId <= 0)
        //    {
        //        return Json(new { status = "E", success = false, message = "Could not find this genre" });
        //    }

        //    // Check if _db.Song is null before querying
        //    if (_db.Genre == null || !_db.Genre.Any())
        //    {
        //        return Json(new { status = "E", success = false, message = "No genre available in the database" });
        //    }

        //    var genre = _db.Genre?
        //                    .Where(s => s.GenreId == genreId)
        //                    .Select(s => new GenreModel
        //                    {
        //                        GenreId = s.GenreId,
        //                        GenreName = s.GenreName,
        //                        GenreImagePath = s.GenreImagePath,
                              

        //                    })
        //                    .FirstOrDefault();

        //    if (genre == null) // Correct condition to check if the artist data is found
        //    {
        //        return Json(new { status = "E", success = false, message = "Genre not found" });
        //    }
         
        //    return View("Genre", genre);
        //}

        public IActionResult AlbumDetails(int albumId)
        {
            if (albumId <= 0)
            {
                return Json(new { status = "E", success = false, message = "Invalid Album ID" });
            }

            // Check if _db.Albums is null before querying
            if (_db.Albums == null)
            {
                return Json(new { status = "E", success = false, message = "No albums available in the database" });
            }

            var album = _db.Albums
                           .Where(s => s.AlbumId == albumId)
                           .Select(s => new Project123.Dto.AlbumModel
                           {
                               AlbumId = s.AlbumId,
                               AlbumName = s.AlbumName,
                               AlbumImagePath = s.AlbumImagePath,
                               ArtistName = s.ArtistName
                           })
                           .FirstOrDefault();

            if (album == null)
            {
                return Json(new { status = "E", success = false, message = "Album not found" });
            }

            // Debugging statement
            System.Diagnostics.Debug.WriteLine($"Album details: {album.AlbumName}, {album.AlbumImagePath}");

            return View("AlbumDetails", album);
        }


        public IActionResult ArtistDetails(string artistname)
        {
            if (string.IsNullOrEmpty(artistname))
            {
                return Json(new { status = "E", success = false, message = "Could not find Artist" });
            }

            // Check if _db.Song is null before querying
            if (_db.Artist == null)
            {
                return Json(new { status = "E", success = false, message = "No Artist available in the database" });
            }

            var artist = _db.Artist?
                            .Where(s => s.ArtistName == artistname)
                            .Select(s => new ArtistModel
                            {
                               ArtistId = s.ArtistId,
                                ArtistBio = s.ArtistBio,
                                ArtistName = s.ArtistName,
                                ArtistGenres = s.ArtistGenres,
                               ArtistImagePath = s.ArtistImagePath,
                              
                            })
                            .FirstOrDefault();

            if (artist == null) // Correct condition to check if the artist data is found
            {
                return Json(new { status = "E", success = false, message = "Artist not found" });
            }

            return View("ArtistDetails", artist);
        }



        //[HttpGet("Spot/AlbumDetails/{id}")]
        //public async Task<IActionResult> AlbumDetails()
        //{
        //    List<ShipmentLocationModel> storageList = new List<ShipmentLocationModel>();
        //    using (HttpClientHandler handler = new HttpClientHandler())
        //    {
        //        // Temporarily bypass SSL certificate validation (not for production use)
        //        handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;

        //        using (HttpClient client = new HttpClient(handler))
        //        {
        //            client.BaseAddress = new Uri("https://localhost:7061/");

        //            try
        //            {
        //                var response = await client.GetAsync("/api/Test/GetShipmentLocationAsync");
        //                if (response.IsSuccessStatusCode)
        //                {
        //                    storageList = await response.Content.ReadAsAsync<List<ShipmentLocationModel>>();
        //                }
        //            }
        //            catch (HttpRequestException ex)
        //            {
        //                this.response.Status = "E";
        //                this.response.Message = ex.Message;
        //            }
        //        }

        //        return Json(new { success = this.response.Success, message = this.response.Message, Data = storageList });
        //    }
        //}

        //[HttpGet("Spot/AlbumDetails/{id}")]
        //public async Task<IActionResult> AlbumDetails(int id)
        //{
        //    ResponseModel resp = new ResponseModel();

        //    using (HttpClientHandler handler = new HttpClientHandler())
        //    {
        //        // Temporarily bypass SSL certificate validation (not for production use)
        //        handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;

        //        var client = _httpClientFactory.CreateClient();
        //        client.BaseAddress = new Uri("https://localhost:7061/");


        //        try
        //        {
        //            //string requestJson = JsonConvert.SerializeObject(id);
        //            //HttpContent httpContent = new StringContent(requestJson, Encoding.UTF8, "application/json");
        //            // Log the request URL
        //            var requestUrl = $"/api/Spot/SearchAlbum/{id}";

        //            var response = await client.GetAsync(requestUrl);

        //            if (response.IsSuccessStatusCode)
        //            {
        //                resp = await response.Content.ReadAsAsync<ResponseModel>();

        //                ////this.response = System.Text.Json.JsonSerializer.Deserialize<ResponseModel>(responseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        //                resp.Status = "S";
        //                resp.Message = "Delete Success";
        //            }
        //            else
        //            {
        //                resp.Status = "E";
        //                resp.Message = $"Error:";
        //            }
        //        }

        //        catch (Exception ex)
        //        {
        //            this.response.Status = "E";
        //            this.response.Message = ex.Message;
        //        }
        //    }

        //    return Json(new { status = resp.Status, success = resp.Success, message = resp.Message });
        //}

        [HttpPost("Spot/CreateGenre1")]
        public async Task<IActionResult> CreateGenre(GenreModel genreData)
        {
            // Check if genre name is provided
            if (string.IsNullOrEmpty(genreData.GenreName))
            {
                return Json(new { status = "E", success = false, message = "Genre GenreName is missing." });
            }

            // Save files and update paths in SongData
            if (genreData.GenreImage != null)
            {
                var (filePath, error, oldFolderPath) = await SaveFileGenre(genreData.GenreImage, genreData.GenreName, genreData.GenreName, null, genreData.GenreImagePath);
                if (error != null)
                {
                    return Json(new { status = "E", success = false, message = error });
                }
                genreData.GenreImagePath = filePath;
            }

            // Create a copy of genre data without IFormFile properties
            var genreDataCopy = new
            {
                genreData.GenreId,
                genreData.GenreName,
                genreData.GenreImagePath
            };

            try
            {
                // Use ApiHelper to create the HttpClient
                var client = _apiHelper.CreateClient();
                client.BaseAddress = new Uri("https://localhost:7061/"); // Ensure the base address is set

                string requestJson = JsonConvert.SerializeObject(genreDataCopy);
                HttpContent httpContent = new StringContent(requestJson, Encoding.UTF8, "application/json");

                var responseResult = await client.PostAsync("api/Spot/CreateGenre", httpContent);

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

            return Json(new { status = this.response.Status, success = this.response.Success, message = this.response.Message });
        }

        [HttpPost("Spot/CreateArtist1")]
        public async Task<IActionResult> CreateArtist(ArtistModel artistData)
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

                        if (string.IsNullOrEmpty(artistData.ArtistName))
                        {
                            return Json(new { status = "E", success = false, message = "Artist ArtistName is missing." });
                        }
                     



                        // Save files and update paths in SongData

                        if (artistData.ArtistImage != null)
                        {
                            var (filePath, error, oldFolderPath) = await SaveFile(artistData.ArtistImage, artistData.ArtistName, artistData.ArtistName, null, artistData.ArtistImagePath);
                            if (error != null)
                            {
                                return Json(new { status = "E", success = false, message = error });
                            }
                            artistData.ArtistImagePath = filePath;
                        }
                        //if (string.IsNullOrEmpty(artistData.ArtistImagePath))
                        //{
                        //    return Json(new { status = "E", success = false, message = "Artist ArtistImagePath is missing." });
                        //}

                        // Create a copy of SongData with nullified IFormFile properties for serialization
                        var artistDataCopy = new
                        {
                            artistData.ArtistId,
                            artistData.ArtistName,
                            artistData.ArtistImagePath,
                            artistData.ArtistGenres,
                            artistData.ArtistBio
                         

                        };

                        string requestJson = JsonConvert.SerializeObject(artistDataCopy);
                        HttpContent httpContent = new StringContent(requestJson, Encoding.UTF8, "application/json");

                        var responseResult = await client.PostAsync("api/Spot/CreateArtist", httpContent);
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

                        if (string.IsNullOrEmpty(AlbumData.ArtistName))
                        {
                            return Json(new { status = "E", success = false, message = "Album ArtistName is missing." });
                        }
                        if (string.IsNullOrEmpty(AlbumData.AlbumName))
                        {
                            return Json(new { status = "E", success = false, message = "Album AlbumName is missing." });
                        }
                       

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

                        //if (string.IsNullOrEmpty(AlbumData.AlbumImagePath))
                        //{
                        //    return Json(new { status = "E", success = false, message = "Album AlbumImagePath is missing." });
                        //}

                        // Create a copy of SongData with nullified IFormFile properties for serialization
                        var AlbumDataCopy = new
                        {
                            AlbumData.AlbumId,
                            AlbumData.ArtistName,
                            AlbumData.AlbumName,
                            AlbumData.AlbumImagePath,
                            AlbumData.AlbumGenre
                           
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


        [HttpPost("Spot/EditAlbum1")]
        public async Task<IActionResult> EditAlbum(AlbumModel AlbumData)
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
                        if (string.IsNullOrEmpty(AlbumData.ArtistName))
                        {
                            return Json(new { status = "E", success = false, message = "Album ArtistName is missing." });
                        }
                        if (string.IsNullOrEmpty(AlbumData.AlbumName))
                        {
                            return Json(new { status = "E", success = false, message = "Album AlbumName is missing." });
                        }
                        //if (string.IsNullOrEmpty(AlbumData.AlbumImagePath))
                        //{
                        //    return Json(new { status = "E", success = false, message = "Album AlbumImagePath is missing." });
                        //}




                        // Save files and update paths in SongData

                        if (AlbumData.AlbumImage != null)
                        {
                            var (filePath, error, oldFolderPath) = await SaveFile(AlbumData.AlbumImage, AlbumData.ArtistName, AlbumData.AlbumName, AlbumData.AlbumId, AlbumData.AlbumImagePath);
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
                            AlbumData.AlbumImagePath,
                            AlbumData.AlbumGenre


                        };

                        string requestJson = JsonConvert.SerializeObject(AlbumDataCopy);
                        HttpContent httpContent = new StringContent(requestJson, Encoding.UTF8, "application/json");

                        var responseResult = await client.PostAsync("api/Spot/EditAlbum", httpContent);
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
                        if (string.IsNullOrEmpty(SongData.SongName))
                        {
                            return Json(new { status = "E", success = false, message = "Song name is missing." });
                        }

                        if (SongData.SongFile == null)
                        {
                            return Json(new { status = "E", success = false, message = "Song file is missing." });
                        }

                        if (string.IsNullOrEmpty(SongData.ArtistName))
                        {
                            return Json(new { status = "E", success = false, message = "Artist name is missing." });
                        }

                        //if (string.IsNullOrEmpty(SongData.SongFilePath))
                        //{
                        //    return Json(new { status = "E", success = false, message = "Song file path is missing." });
                        //}
                      

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
                        //if (string.IsNullOrEmpty(SongData.SongImagePath))
                        //{
                        //    return Json(new { status = "E", success = false, message = "Song Image path is missing." });
                        //}

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
                            SongData.SongLength,
                            SongData.FavoriteSong,
                            SongData.CreateSongDate
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



        [HttpPost("Spot/UpdateGenre1")]
        public async Task<IActionResult> UpdateGenre(GenreModel genreData)
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
                        if (string.IsNullOrEmpty(genreData.GenreName))
                        {
                            return Json(new { status = "E", success = false, message = "Genre GenreName is missing." });
                        }
                      





                        // Save files and update paths in SongData

                        if (genreData.GenreImage != null)
                        {
                            var (filePath, error, oldFolderPath) = await SaveFileGenre(genreData.GenreImage, genreData.GenreName, genreData.GenreName, null, genreData.GenreImagePath);
                            if (error != null)
                            {
                                return Json(new { status = "E", success = false, message = error });
                            }
                            genreData.GenreImagePath = filePath;
                        }
                        //if (string.IsNullOrEmpty(artistModel.ArtistImagePath))
                        //{
                        //    return Json(new { status = "E", success = false, message = "Artist ArtistImagePath is missing." });
                        //}

                        // Create a copy of SongData with nullified IFormFile properties for serialization
                        var genreDataCopy = new
                        {
                            genreData.GenreId,
                            genreData.GenreName,
                            genreData.GenreImagePath,



                        };


                        string requestJson = JsonConvert.SerializeObject(genreDataCopy);
                        HttpContent httpContent = new StringContent(requestJson, Encoding.UTF8, "application/json");

                        var responseResult = await client.PostAsync("api/Spot/UpdateGenre", httpContent);
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



        [HttpPost("Spot/DeleteProfile1")]
        public async Task<IActionResult> DeleteProfile(AdminModel adminData)
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
                        if (string.IsNullOrEmpty(adminData.Name))
                        {
                            return Json(new { status = "E", success = false, message = " UserName is missing." });
                        }

                        // Save files and update paths in SongData

                        var (filePath, error, oldFolderPath) = await DeleteFileProfile(adminData.ProfileImage, adminData.Name, adminData.Name, null, adminData.ProfileImagePath);
                        if (error != null)
                        {
                            return Json(new { status = "E", success = false, message = error });
                        }
                
                        string requestJson = JsonConvert.SerializeObject(adminData);
                        HttpContent httpContent = new StringContent(requestJson, Encoding.UTF8, "application/json");

                        var responseResult = await client.PostAsync("api/Spot/DeleteProfile", httpContent);
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

        [HttpPost("Spot/UpdateProfile1")]
        public async Task<IActionResult> UpdateProfile(AdminModel adminData)
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
                        if (string.IsNullOrEmpty(adminData.Name))
                        {
                            return Json(new { status = "E", success = false, message = " UserName is missing." });
                        }

                        // Save files and update paths in SongData

                        if (adminData.ProfileImage != null)
                        {
                            var (filePath, error, oldFolderPath) = await SaveFileProfile(adminData.ProfileImage, adminData.Name, adminData.Name, null, adminData.ProfileImagePath);
                            if (error != null)
                            {
                                return Json(new { status = "E", success = false, message = error });
                            }
                            adminData.ProfileImagePath = filePath;
                        }
                        //if (string.IsNullOrEmpty(artistModel.ArtistImagePath))
                        //{
                        //    return Json(new { status = "E", success = false, message = "Artist ArtistImagePath is missing." });
                        //}

                        // Create a copy of SongData with nullified IFormFile properties for serialization
                        var userProfileCopy = new
                        {
                            adminData.Id,
                            adminData.Name,
                            adminData.Email,
                            adminData.Password,
                            adminData.ProfileImagePath,



                        };


                        string requestJson = JsonConvert.SerializeObject(userProfileCopy);
                        HttpContent httpContent = new StringContent(requestJson, Encoding.UTF8, "application/json");

                        var responseResult = await client.PostAsync("api/Spot/UpdateProfile", httpContent);
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


        [HttpPost("Spot/UpdateArtist1")]
        public async Task<IActionResult> UpdateArtist(ArtistModel artistData)
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
                        if (string.IsNullOrEmpty(artistData.ArtistName))
                        {
                            return Json(new { status = "E", success = false, message = "Artist ArtistName is missing." });
                        }
                        if (string.IsNullOrEmpty(artistData.ArtistGenres))
                        {
                            return Json(new { status = "E", success = false, message = "Artist ArtistGenres is missing." });
                        }


                        // Save files and update paths in SongData

                        if (artistData.ArtistImage != null)
                        {
                            var (filePath, error, oldFolderPath) = await SaveFile(artistData.ArtistImage, artistData.ArtistName, artistData.ArtistName, null, artistData.ArtistImagePath);
                            if (error != null)
                            {
                                return Json(new { status = "E", success = false, message = error });
                            }
                            artistData.ArtistImagePath = filePath;
                        }
                        //if (string.IsNullOrEmpty(artistModel.ArtistImagePath))
                        //{
                        //    return Json(new { status = "E", success = false, message = "Artist ArtistImagePath is missing." });
                        //}

                        // Create a copy of SongData with nullified IFormFile properties for serialization
                        var artistDataCopy = new
                        {
                            artistData.ArtistId,
                            artistData.ArtistName,
                            artistData.ArtistImagePath,
                            artistData.ArtistGenres,
                            artistData.ArtistBio
                        };

                        string requestJson = JsonConvert.SerializeObject(artistDataCopy);
                        HttpContent httpContent = new StringContent(requestJson, Encoding.UTF8, "application/json");

                        var responseResult = await client.PostAsync("api/Spot/UpdateArtist", httpContent);
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
                        if (string.IsNullOrEmpty(SongData.SongName))
                        {
                            return Json(new { status = "E", success = false, message = "Song name is missing." });
                        }

                        if (SongData.SongFile == null)
                        {
                            return Json(new { status = "E", success = false, message = "Song file is missing." });
                        }

                        if (string.IsNullOrEmpty(SongData.ArtistName))
                        {
                            return Json(new { status = "E", success = false, message = "Artist name is missing." });
                        }

                        if (string.IsNullOrEmpty(SongData.SongFilePath))
                        {
                            return Json(new { status = "E", success = false, message = "Song file path is missing." });
                        }
                      
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

        [HttpPost("Spot/AddSong123")]
        public async Task<IActionResult> AddSong(SongModel songData)
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
                        // Check if SongName is null before proceeding
                        if (string.IsNullOrEmpty(songData.SongName))
                        {
                            return Json(new { status = "E", success = false, message = "Song name is missing." });
                        }                     

                        if (string.IsNullOrEmpty(songData.ArtistName))
                        {
                            return Json(new { status = "E", success = false, message = "Artist name is missing." });
                        }

                        if (string.IsNullOrEmpty(songData.SongFilePath))
                        {
                            return Json(new { status = "E", success = false, message = "Song file path is missing." });
                        }

                        // Update song file path
                        var (songFilePath, songFileError) = MoveFileToAlbumUpdate(songData.SongFilePath, songData.ArtistName, songData.SongName, songData.AlbumId);
                        if (songFileError != null)
                        {
                            return Json(new { status = "E", success = false, message = songFileError });
                        }
                        songData.SongFilePath = songFilePath;

                        // Update song image path
                        if (songData.SongImagePath != null)
                        {
                            var (imageFilePath, imageFileError) = MoveFileToAlbumUpdate(songData.SongImagePath, songData.ArtistName, songData.SongName, songData.AlbumId);
                            if (imageFileError != null)
                            {
                                return Json(new { status = "E", success = false, message = imageFileError });
                            }
                            songData.SongImagePath = imageFilePath;
                        }
                                      
                        // Create a copy of songData with nullified IFormFile properties for serialization
                        var songDataCopy = new
                        {
                            songData.AlbumId,
                            songData.ArtistName,
                            songData.SongFilePath,
                            songData.SongGenres,
                            songData.SongId,
                            songData.SongImagePath,
                            songData.SongName,
                            songData.SongLength
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

        [HttpPost("Spot/SearchSpot1/")]
        public async Task<IActionResult> SearchSpot(SearchSpotModal searchData)
        {
           
            searchData.UserId = Convert.ToInt32(HttpContext.Session.GetString("UserId") ?? Request.Cookies["UserId"]);
            // Check if SongName is null before proceeding
            List<SearchSpotModal> searchSpotList = new List<SearchSpotModal>();

            using (HttpClientHandler handler = new HttpClientHandler())
            {
                // Temporarily bypass SSL certificate validation (not for production use)
                handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;

                using (HttpClient client = new HttpClient(handler))
                {
                    client.BaseAddress = new Uri("https://localhost:7061/");
                    try
                    {             
                        // Create a copy of songData with nullified IFormFile properties for serialization
                        string requestJson = JsonConvert.SerializeObject(searchData);
                        HttpContent httpContent = new StringContent(requestJson, Encoding.UTF8, "application/json");

                        var responseResult = await client.PostAsync("api/Spot/SearchSpot", httpContent);
                        if (responseResult.IsSuccessStatusCode)
                        {
                            //this.response = await responseResult.Content.ReadAsAsync<ResponseModel>();
                            searchSpotList = await responseResult.Content.ReadAsAsync<List<SearchSpotModal>>();

                            if (searchSpotList.Count > 0)
                            {
                                this.response.Status = "S";
                                this.response.Message = "Success";
                            }
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
            return Json(new { status = this.response.Status, success = this.response.Success, message = this.response.Message ,data = searchSpotList });
        }

        [HttpPost("Spot/FavoriteSong1")]
        public async Task<IActionResult> FavoriteSong(SongModel songData)
        {

            songData.UserId = Convert.ToInt32(HttpContext.Session.GetString("UserId") ?? Request.Cookies["UserId"]);
            using (HttpClientHandler handler = new HttpClientHandler())
            {
                // Temporarily bypass SSL certificate validation (not for production use)
                handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;

                using (HttpClient client = new HttpClient(handler))
                {
                    client.BaseAddress = new Uri("https://localhost:7061/");

                    try
                    {
                        // Check if SongName is null before proceeding
                        if (string.IsNullOrEmpty(songData.SongName))
                        {
                            return Json(new { status = "E", success = false, message = "Song name is missing." });
                        }

                        if (string.IsNullOrEmpty(songData.ArtistName))
                        {
                            return Json(new { status = "E", success = false, message = "Artist name is missing." });
                        }

                        if (string.IsNullOrEmpty(songData.SongFilePath))
                        {
                            return Json(new { status = "E", success = false, message = "Song file path is missing." });
                        }
                                   
                        // Create a copy of songData with nullified IFormFile properties for serialization
                 
                        string requestJson = JsonConvert.SerializeObject(songData);
                        HttpContent httpContent = new StringContent(requestJson, Encoding.UTF8, "application/json");

                        //var responseResult = await client.PostAsync("api/Spot/UpdateSong", httpContent);
                        var responseResult = await client.PostAsync("api/Spot/FavoriteSong", httpContent);
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


        [HttpPost("Spot/FavoriteAlbum1")]
        public async Task<IActionResult> FavoriteAlbum(SpotSidebarModel albumData)
        {
            List<SpotSidebarModel> albumList = new List<SpotSidebarModel>();
            albumData.UserId = Convert.ToInt32(HttpContext.Session.GetString("UserId") ?? Request.Cookies["UserId"]);
            using (HttpClientHandler handler = new HttpClientHandler())
            {
                // Temporarily bypass SSL certificate validation (not for production use)
                handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;

                using (HttpClient client = new HttpClient(handler))
                {
                    client.BaseAddress = new Uri("https://localhost:7061/");

                    try
                    {
                        // Create a copy of songData with nullified IFormFile properties for serialization
                        string requestJson = JsonConvert.SerializeObject(albumData);
                        HttpContent httpContent = new StringContent(requestJson, Encoding.UTF8, "application/json");

                        //var responseResult = await client.PostAsync("api/Spot/UpdateSong", httpContent);
                        var responseResult = await client.PostAsync("api/Spot/FavoriteAlbum", httpContent);
                        if (responseResult.IsSuccessStatusCode)
                        {
                            albumList = await responseResult.Content.ReadAsAsync<List<SpotSidebarModel>>();
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

            return Json(new { status = this.response.Status, success = this.response.Success, message = this.response.Message, Data = albumList });
        }

        [HttpPost("Spot/FavoriteArtist1")]
        public async Task<IActionResult> FavoriteArtist(SpotSidebarModel artistData)
        {
            List<SpotSidebarModel> artistList = new List<SpotSidebarModel>();
            artistData.UserId = Convert.ToInt32(HttpContext.Session.GetString("UserId") ?? Request.Cookies["UserId"]);
            using (HttpClientHandler handler = new HttpClientHandler())
            {
                // Temporarily bypass SSL certificate validation (not for production use)
                handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;

                using (HttpClient client = new HttpClient(handler))
                {
                    client.BaseAddress = new Uri("https://localhost:7061/");

                    try
                    {               
                        string requestJson = JsonConvert.SerializeObject(artistData);
                        HttpContent httpContent = new StringContent(requestJson, Encoding.UTF8, "application/json");

                        //var responseResult = await client.PostAsync("api/Spot/UpdateSong", httpContent);
                        var responseResult = await client.PostAsync("api/Spot/FavoriteArtist", httpContent);
                        if (responseResult.IsSuccessStatusCode)
                        {
                            artistList = await responseResult.Content.ReadAsAsync <List<SpotSidebarModel>>();
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

            return Json(new { status = this.response.Status, success = this.response.Success, message = this.response.Message,Data = artistList });
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
                    if (SongData.SongFilePath == null)
                    {
                        return Json(new { status = "E", success = false, message = "Song file not found" });
                    }

                    if (SongData.ArtistName == null)
                    {
                        return Json(new { status = "E", success = false, message = "Song artist name not found" });
                    }



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

        [HttpPost("Spot/DeleteAlbum1")]

        public async Task<IActionResult>DeleteAlbum(AlbumModel AlbumData)
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
                    //if (AlbumData.AlbumImage == null)
                    //{
                    //    return Json(new { status = "E", success = false, message = "Album image not found" });
                    //}

                    if (AlbumData.ArtistName == null)
                    {
                        return Json(new { status = "E", success = false, message = "Album artist name not found" });
                    }

                    var (filePath, error, oldFolderPath) = await DeleteFile(AlbumData.AlbumImage, AlbumData.ArtistName, AlbumData.AlbumName, AlbumData.AlbumId, AlbumData.AlbumImagePath);
                    if (error != null)
                    {
                        return Json(new { status = "E", success = false, message = error });
                    }

                    string requestJson = JsonConvert.SerializeObject(AlbumData);
                    HttpContent httpContent = new StringContent(requestJson, Encoding.UTF8, "application/json");

                    var responseResult = await client.PostAsync("api/Spot/DeleteAlbum", httpContent);
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

        [HttpPost("Spot/DeleteGenre1")]

        public async Task<IActionResult> DeleteGenre(GenreModel genreData)
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
                 
                    if (genreData.GenreName == null)
                    {
                        return Json(new { status = "E", success = false, message = "Could not find Genre Name" });
                    }
                    var (filePath, error, oldFolderPath) = await DeleteFileGenre(genreData.GenreImage, genreData.GenreName, genreData.GenreName, null, genreData.GenreImagePath);

                    string requestJson = JsonConvert.SerializeObject(genreData);
                    HttpContent httpContent = new StringContent(requestJson, Encoding.UTF8, "application/json");

                    var responseResult = await client.PostAsync("api/Spot/DeleteGenre", httpContent);
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

        [HttpPost("Spot/DeleteArtist1")]

        public async Task<IActionResult> DeleteArtist(ArtistModel artistData)
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

                    if (artistData.ArtistName == null)
                    {
                        return Json(new { status = "E", success = false, message = "Can not Find Artist Name" });
                    }
                    var (filePath, error, oldFolderPath) = await DeleteFile(artistData.ArtistImage, artistData.ArtistName, artistData.ArtistName, null, artistData.ArtistImagePath);
                    if (error != null)
                    {
                        return Json(new { status = "E", success = false, message = error });
                    }



                    string requestJson = JsonConvert.SerializeObject(artistData);
                    HttpContent httpContent = new StringContent(requestJson, Encoding.UTF8, "application/json");

                    var responseResult = await client.PostAsync("api/Spot/DeleteArtist", httpContent);
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


        [HttpPost("Spot/RemoveSong1")]

        public async Task<IActionResult> RemoveSong(SongModel SongData)
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
                    string requestJson = JsonConvert.SerializeObject(SongData);
                    HttpContent httpContent = new StringContent(requestJson, Encoding.UTF8, "application/json");

                    var responseResult = await client.PostAsync("api/Spot/RemoveSong", httpContent);
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
        private async Task<(string? filePath, string? error, string? oldFolderPath)> SaveFileGenre(IFormFile file, string artistName, string? name, int? albumId, string? existingFilePath)
        {
            if (file == null || file.Length == 0)
            {
                return (null, "File is empty", null); // Allow null values
            }

            // Sanitize artist name and song name for use in file paths
            var sanitizedArtistName = SanitizeFileName(artistName);
            if (name == null)
            {
                name = "unname";
            }
            var sanitizedName = SanitizeFileName(name);

            // Create the directory path for the artist and song
            var artistFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/genre", sanitizedArtistName);
            var albumFolderPath = albumId != null
                ? Path.Combine(artistFolderPath, "Album_" + albumId.ToString())
                : Path.Combine(artistFolderPath, sanitizedName);

            // Ensure the directories exist
            Directory.CreateDirectory(artistFolderPath);

            // Generate a unique file name and get the full file path
            string fileName;
            if (file.ContentType == "audio/mpeg")
            {
                fileName = $"{name}.mp3";
            }
            else if (file.ContentType == "audio/flac")
            {
                fileName = $"{name}.flac";
            }
            else
            {
                fileName = file.FileName;
            }
            var filePath = Path.Combine(artistFolderPath, fileName);

            var existingFiles = Directory.GetFiles(artistFolderPath, fileName);
            if (existingFiles.Length > 0)
            {
                System.IO.File.Delete(existingFiles[0]);
            }

            string? oldFolderPath = null; // Declare oldFolderPath as nullable

            // Check if there's an existing file and delete it
            if (!string.IsNullOrEmpty(existingFilePath))
            {
                var fullExistingFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", existingFilePath.TrimStart('/').Replace("/", "\\"));
                if (System.IO.File.Exists(fullExistingFilePath))
                {
                    oldFolderPath = Path.GetDirectoryName(fullExistingFilePath); // Assigning null if the path is null
                    System.IO.File.Delete(fullExistingFilePath);
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
                    return (null, moveError, oldFolderPath); // Return nullable values
                }
                filePath = newFilePath;
            }

            // Return the relative path to the saved file
            return (Path.Combine("/uploads/Genre", sanitizedArtistName, albumId != null ? $"Album_{albumId}" : fileName).Replace("\\", "/"), null, oldFolderPath);
        }
        private async Task<(string? filePath, string? error, string? oldFolderPath)> SaveFile(IFormFile file, string artistName, string? name, int? albumId, string? existingFilePath)
        {
            if (file == null || file.Length == 0)
            {
                return (null, "File is empty", null); // Allow null values
            }

            // Sanitize artist name and song name for use in file paths
            var sanitizedArtistName = SanitizeFileName(artistName);
            if (name == null)
            {
                name = "unname";
            }
            var sanitizedName = SanitizeFileName(name);

            // Create the directory path for the artist and song
            var artistFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/ArtistName", sanitizedArtistName);
            var albumFolderPath = albumId != null
                ? Path.Combine(artistFolderPath, "Album_" + albumId.ToString())
                : Path.Combine(artistFolderPath, sanitizedName);

            // Ensure the directories exist
            Directory.CreateDirectory(albumFolderPath);

            // Generate a unique file name and get the full file path
            string fileName;
            if (file.ContentType == "audio/mpeg")
            {
                fileName = $"{name}.mp3";
            }
            else if (file.ContentType == "audio/flac")
            {
                fileName = $"{name}.flac";
            }
            else
            {
                fileName = file.FileName;
            }
            var filePath = Path.Combine(albumFolderPath, fileName);

            var existingFiles = Directory.GetFiles(albumFolderPath, fileName);
            if (existingFiles.Length > 0)
            {
                System.IO.File.Delete(existingFiles[0]);
            }

            string? oldFolderPath = null; // Declare oldFolderPath as nullable

            // Check if there's an existing file and delete it
            if (!string.IsNullOrEmpty(existingFilePath))
            {
                var fullExistingFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", existingFilePath.TrimStart('/').Replace("/", "\\"));
                if (System.IO.File.Exists(fullExistingFilePath))
                {
                    oldFolderPath = Path.GetDirectoryName(fullExistingFilePath); // Assigning null if the path is null
                    System.IO.File.Delete(fullExistingFilePath);
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
                    return (null, moveError, oldFolderPath); // Return nullable values
                }
                filePath = newFilePath;
            }

            // Return the relative path to the saved file
            return (Path.Combine("/uploads/ArtistName", sanitizedArtistName, albumId != null ? $"Album_{albumId}" : sanitizedName, fileName).Replace("\\", "/"), null, oldFolderPath);
        }




        private Task<(string? filePath, string? error, string? oldFolderPath)> DeleteFileGenre(IFormFile? file, string artistName, string? name, int? albumId, string? existingFilePath)
        {
            //if (file == null || file.Length == 0)
            //{
            //    return Task.FromResult<(string? filePath, string? error, string? oldFolderPath)>((null, "File is empty", null)); // Allow null values
            //}

            // Sanitize artist name and song name for use in file paths



            // Sanitize artist name and song name for use in file paths

            if (name == null)
            {
                name = "unname";
            }
            var sanitizedArtistName = SanitizeFileName(artistName);
            var sanitizedName = SanitizeFileName(name);



            // Create the directory path for the artist and song
            var artistFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/genre", sanitizedArtistName);
            //   var albumFolderPath = albumId != null ? Path.Combine(artistFolderPath, "Album_" + albumId.ToString()) : Path.Combine(artistFolderPath, sanitizedName);
            var albumFolderPath = artistFolderPath;

            // Ensure the directories exist
            Directory.CreateDirectory(albumFolderPath);

            // Variable to store the old folder path
            string? oldFolderPath = null;

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
            // Check if the artist folder is also empty (if album was deleted)
            if (!string.IsNullOrEmpty(oldFolderPath) && IsFolderEmpty(oldFolderPath))
            {
                // Delete the folder if it is empty
                Directory.Delete(oldFolderPath);
            }

            // Check if the artist folder is also empty (if album was deleted)
            if (IsFolderEmpty(artistFolderPath))
            {
                // Delete the artist folder if it's empty
                Directory.Delete(artistFolderPath);
            }

            // Return the old folder path
            return Task.FromResult<(string? filePath, string? error, string? oldFolderPath)>((null, null, oldFolderPath));


        }


        private Task<(string? filePath, string? error, string? oldFolderPath)> DeleteFileProfile(IFormFile? file, string artistName, string? name, int? albumId, string? existingFilePath)
        {
            //if (file == null || file.Length == 0)
            //{
            //    return Task.FromResult<(string? filePath, string? error, string? oldFolderPath)>((null, "File is empty", null)); // Allow null values
            //}

            // Sanitize artist name and song name for use in file paths



            // Sanitize artist name and song name for use in file paths

            if (name == null)
            {
                name = "unname";
            }
            var sanitizedArtistName = SanitizeFileName(artistName);
            var sanitizedName = SanitizeFileName(name);



            // Create the directory path for the artist and song
            var artistFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/UserNameId", sanitizedArtistName);
            var albumFolderPath = albumId != null ? Path.Combine(artistFolderPath, "Album_" + albumId.ToString()) : Path.Combine(artistFolderPath, sanitizedName);

            // Ensure the directories exist
            Directory.CreateDirectory(albumFolderPath);

            // Variable to store the old folder path
            string? oldFolderPath = null;

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
            // Check if the artist folder is also empty (if album was deleted)
            if (!string.IsNullOrEmpty(oldFolderPath) && IsFolderEmpty(oldFolderPath))
            {
                // Delete the folder if it is empty
                Directory.Delete(oldFolderPath);
            }

            // Check if the artist folder is also empty (if album was deleted)
            if (IsFolderEmpty(artistFolderPath))
            {
                // Delete the artist folder if it's empty
                Directory.Delete(artistFolderPath);
            }

            // Return the old folder path
            return Task.FromResult<(string? filePath, string? error, string? oldFolderPath)>((null, null, oldFolderPath));


        }

        private Task <(string? filePath, string? error, string? oldFolderPath)> DeleteFile(IFormFile? file, string artistName, string? name, int? albumId, string? existingFilePath)
        {
            //if (file == null || file.Length == 0)
            //{
            //    return Task.FromResult<(string? filePath, string? error, string? oldFolderPath)>((null, "File is empty", null)); // Allow null values
            //}

            // Sanitize artist name and song name for use in file paths



            // Sanitize artist name and song name for use in file paths

            if (name == null)
            {
                name = "unname";
            }
            var sanitizedArtistName = SanitizeFileName(artistName);
            var sanitizedName = SanitizeFileName(name);



            // Create the directory path for the artist and song
            var artistFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/ArtistName", sanitizedArtistName);
            var albumFolderPath = albumId != null ? Path.Combine(artistFolderPath, "Album_" + albumId.ToString()) : Path.Combine(artistFolderPath, sanitizedName);

            // Ensure the directories exist
            Directory.CreateDirectory(albumFolderPath);

            // Variable to store the old folder path
            string? oldFolderPath = null;

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
            // Check if the artist folder is also empty (if album was deleted)
            if (!string.IsNullOrEmpty(oldFolderPath) && IsFolderEmpty(oldFolderPath))
            {
                // Delete the folder if it is empty
                Directory.Delete(oldFolderPath);
            }

            // Check if the artist folder is also empty (if album was deleted)
            if (IsFolderEmpty(artistFolderPath))
            {
                // Delete the artist folder if it's empty
                Directory.Delete(artistFolderPath);
            }

            // Return the old folder path
            return Task.FromResult<(string? filePath, string? error, string? oldFolderPath)>((null, null, oldFolderPath));


        }
        private bool IsFolderEmpty(string path)
        {
            // Ensure the directory exists before checking
            if (!Directory.Exists(path))
                return true; // Consider it empty if it doesn't exist

            return Directory.GetFiles(path).Length == 0 && Directory.GetDirectories(path).Length == 0;
        }


        // Helper method to move file to album
        private (string? newFilePath, string? error) MoveFileToAlbum(string filePath, string artistFolderPath, string? sanitizedName, int albumId)
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

        // แก้ชื่อไฟล์
        private string SanitizeFileName(string name)
        {
            foreach (var c in Path.GetInvalidFileNameChars())
            {
                name = name.Replace(c, '_');
            }
            return name;
        }

        private (string? newFilePath, string? error) MoveFileToAlbumUpdate(string existingFilePath, string artistName, string songName, int? albumId)
        {
            try
            {
                // Sanitize artist name and song name for use in file paths
                var sanitizedArtistName = SanitizeFileName(artistName);
                var sanitizedName = SanitizeFileName(songName);

                // Create the directory path for the artist and song
                var artistFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/ArtistName", sanitizedArtistName);
                var albumFolderPath = albumId != null ? Path.Combine(artistFolderPath, "Album_" + albumId.ToString()) : Path.Combine(artistFolderPath, sanitizedName);

                // Ensure the directories exist
                Directory.CreateDirectory(albumFolderPath);

                // Generate a unique file name and get the full file path
                var fileName = Path.GetFileName(existingFilePath);
                var newFilePath = Path.Combine(albumFolderPath, fileName);

                // Move the file to the new location
                
                var fullExistingFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", existingFilePath.TrimStart('/').Replace("/", "\\"));
                // Return the new file path
                System.IO.File.Move(fullExistingFilePath, newFilePath);
               
                return (Path.Combine("/uploads/ArtistName", sanitizedArtistName, albumId != null ? $"Album_{albumId}" : sanitizedName, fileName).Replace("\\", "/"), null);
            }
            catch (Exception ex)
            {
                return (null, $"Error moving file to album: {ex.Message}");
            }
        }

        private async Task<(string? filePath, string? error, string? oldFolderPath)> SaveFileProfile(IFormFile file, string artistName, string? name, int? albumId, string? existingFilePath)
        {
            if (file == null || file.Length == 0)
            {
                return (null, "File is empty", null); // Allow null values
            }

            // Sanitize artist name and song name for use in file paths
            var sanitizedArtistName = SanitizeFileName(artistName);
            if (name == null)
            {
                name = "unname";
            }
            var sanitizedName = SanitizeFileName(name);

            // Create the directory path for the artist and song
            var artistFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/UserNameId", sanitizedArtistName);
            var albumFolderPath = albumId != null
                ? Path.Combine(artistFolderPath, "Album_" + albumId.ToString())
                : Path.Combine(artistFolderPath, sanitizedName);

            // Ensure the directories exist
            Directory.CreateDirectory(albumFolderPath);

            // Generate a unique file name and get the full file path
            string fileName;
            if (file.ContentType == "audio/mpeg")
            {
                fileName = $"{name}.mp3";
            }
            else if (file.ContentType == "audio/flac")
            {
                fileName = $"{name}.flac";
            }
            else
            {
                fileName = file.FileName;
            }
            var filePath = Path.Combine(albumFolderPath, fileName);

            var existingFiles = Directory.GetFiles(albumFolderPath, fileName);
            if (existingFiles.Length > 0)
            {
                System.IO.File.Delete(existingFiles[0]);
            }

            string? oldFolderPath = null; // Declare oldFolderPath as nullable

            // Check if there's an existing file and delete it
            if (!string.IsNullOrEmpty(existingFilePath))
            {
                var fullExistingFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", existingFilePath.TrimStart('/').Replace("/", "\\"));
                if (System.IO.File.Exists(fullExistingFilePath))
                {
                    oldFolderPath = Path.GetDirectoryName(fullExistingFilePath); // Assigning null if the path is null
                    System.IO.File.Delete(fullExistingFilePath);
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
                    return (null, moveError, oldFolderPath); // Return nullable values
                }
                filePath = newFilePath;
            }

            // Return the relative path to the saved file
            return (Path.Combine("/uploads/UserNameId", sanitizedArtistName, albumId != null ? $"Album_{albumId}" : sanitizedName, fileName).Replace("\\", "/"), null, oldFolderPath);
        }


        [HttpPost("Spot/SearchSongNotInAlbum1")]
        public async Task<IActionResult> SearchSongNotInAlbum(SongModel SongData)
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

                    var response = await client.PostAsync("/api/Spot/SearchSongNotInAlbum", httpContent);
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


        [HttpPost("Spot/SearchSong1")]
        public async Task<IActionResult> SearchSong(SongModel SongData)
        {
            ResponseModel resp = new ResponseModel();
            List<SongModel> SongList = new List<SongModel>();
            SongData.UserId = Convert.ToInt32(HttpContext.Session.GetString("UserId") ?? Request.Cookies["UserId"]);
            using (HttpClientHandler handler = new HttpClientHandler())
            {
                // Temporarily bypass SSL certificate validation (not for production use)
                handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;

                var client = _apiHelper.CreateClient();
                client.BaseAddress = new Uri("https://localhost:7061/"); // Ensure the base address is set
  

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

        [HttpPost("Spot/GetFavAlbumAndArtistByUser1")]
        public async Task<IActionResult> GetFavAlbumAndArtistByUser(string? userId)
        {
            userId = HttpContext.Session.GetString("UserId") ?? Request.Cookies["UserId"];

            List<SpotSidebarModel> spotSidebarList = new List<SpotSidebarModel>();

            using (HttpClientHandler handler = new HttpClientHandler())
            {
                // Temporarily bypass SSL certificate validation (not for production use)
                handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;

                try
                {
                    var client = _apiHelper.CreateClient();
                    client.BaseAddress = new Uri("https://localhost:7061/"); // Ensure the base address is set





                    string requestJson = JsonConvert.SerializeObject(userId);

                    HttpContent httpContent = new StringContent(requestJson, Encoding.UTF8, "application/json");

                    var response = await client.PostAsync("/api/Spot/GetFavAlbumAndArtistByUser", httpContent);
                    if (response.IsSuccessStatusCode)
                    {
                        spotSidebarList = await response.Content.ReadAsAsync <List<SpotSidebarModel>>();

                        if (spotSidebarList != null && spotSidebarList.Count > 0)
                        {
                            this.response.Status = "S";
                            this.response.Message = "Successfully retrieved Album and Artist .";
                        }
                        else
                        {
                            this.response.Status = "E";
                            this.response.Message = "No Album and Artist found.";
                        }
                    }
                    else
                    {
                        this.response.Status = "E";
                        this.response.Message = $"Error: {response.ReasonPhrase}";
                    }
                }
                catch (HttpRequestException ex)
                {
                    this.response.Status = "E";
                    this.response.Message = $"Request failed: {ex.Message}";
                }
            }

            return Json(new { success = this.response.Success, message = this.response.Message, Data = spotSidebarList });
        }


        [HttpGet("Spot/GetGenre")]

        public async Task<IActionResult> GetGenre()
        {
            List<GenreModel> genreList = new List<GenreModel>();
            using (HttpClientHandler handler = new HttpClientHandler())
            {
                handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
                var client =_apiHelper.CreateClient();
                client.BaseAddress = new Uri("https://localhost:7061/");

                try
                {
                    var response = await client.GetAsync("/api/Spot/GetGenre");
                    if (response.IsSuccessStatusCode)
                    {
                        genreList = await response.Content.ReadAsAsync<List<GenreModel>>();
                    }
                }
                catch (HttpRequestException ex)
                {
                    this.response.Status = "E";
                    this.response.Message = ex.Message;
                }
            }
            return Json(new { success = this.response.Success, message = this.response.Message, Data = genreList });
        }


        [HttpPost("Spot/GetProfileImage1")]
        public async Task<IActionResult> GetProfileImage(AdminModel adminData)
        {

            ResponseModel resp = new ResponseModel();
            List<AdminModel> adminList = new List<AdminModel>();

            using (HttpClientHandler handler = new HttpClientHandler())
            {
                // Temporarily bypass SSL certificate validation (not for production use)
                handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;

                var client = _apiHelper.CreateClient();
                client.BaseAddress = new Uri("https://localhost:7061/");


                try
                {

                    adminData.Id = Convert.ToInt32(HttpContext.Session.GetString("UserId") ?? Request.Cookies["UserId"]);
                    string requestJson = JsonConvert.SerializeObject(adminData);
                    HttpContent httpContent = new StringContent(requestJson, Encoding.UTF8, "application/json");


                    var response = await client.PostAsync("/api/Spot/GetProfileImage", httpContent);
                    if (response.IsSuccessStatusCode)
                    {
                        adminList = await response.Content.ReadAsAsync<List<AdminModel>>();

                        if (adminList.Count > 0)
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

            return Json(new { status = resp.Status, success = resp.Success, message = resp.Message, Data = adminList });
        }


        //[HttpPost("Spot/GetProfileImage1")]
        //public async Task<IActionResult> GetProfileImage(AdminModel adminData)
        //{

        //    ResponseModel resp = new ResponseModel();
        //    List<AdminModel> adminList = new List<AdminModel>();

        //    using (HttpClientHandler handler = new HttpClientHandler())
        //    {
        //        // Temporarily bypass SSL certificate validation (not for production use)
        //        handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;

        //        var client = _apiHelper.CreateClient();
        //        client.BaseAddress = new Uri("https://localhost:7061/");


        //        try
        //        {
        //            string requestJson = JsonConvert.SerializeObject(adminData);
        //            HttpContent httpContent = new StringContent(requestJson, Encoding.UTF8, "application/json");


        //            var response = await client.PostAsync("/api/Spot/GetProfileImage", httpContent);
        //            if (response.IsSuccessStatusCode)
        //            {
        //                adminList = await response.Content.ReadAsAsync<List<AdminModel>>();

        //                if (adminList.Count > 0)
        //                {
        //                    resp.Status = "S";
        //                    resp.Message = "Success";
        //                }

        //                else
        //                {
        //                    resp.Status = "E";
        //                    resp.Message = $"Error:";
        //                }


        //                ////this.response = System.Text.Json.JsonSerializer.Deserialize<ResponseModel>(responseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        //            }
        //            else
        //            {
        //                resp.Status = "E";
        //                resp.Message = $"Error:";
        //            }
        //        }

        //        catch (Exception ex)
        //        {
        //            this.response.Status = "E";
        //            this.response.Message = ex.Message;
        //        }
        //    }

        //    return Json(new { status = resp.Status, success = resp.Success, message = resp.Message, Data = adminList });
        //}


        [HttpPost("Spot/GetFavSongByUser1")]
        public async Task<IActionResult> GetFavSongByUser( string userId)
        {
            List<SongModel> songDataList = new List<SongModel>();

            using (HttpClientHandler handler = new HttpClientHandler())
            {
                // Temporarily bypass SSL certificate validation (not for production use)
                handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;

                try
                {
                    var client = _apiHelper.CreateClient();
                    client.BaseAddress = new Uri("https://localhost:7061/"); // Ensure the base address is set
              
                    string requestJson = JsonConvert.SerializeObject(userId);

                    HttpContent httpContent = new StringContent(requestJson, Encoding.UTF8, "application/json");

                    var response = await client.PostAsync("/api/Spot/GetFavSongByUser", httpContent);
                    if (response.IsSuccessStatusCode)
                    {
                        songDataList = await response.Content.ReadAsAsync<List<SongModel>>();

                        if (songDataList != null && songDataList.Count > 0)
                        {
                            this.response.Status = "S";
                            this.response.Message = "Successfully retrieved favorite songs.";
                        }
                        else
                        {
                            this.response.Status = "E";
                            this.response.Message = "No favorite songs found.";
                        }
                    }
                    else
                    {
                        this.response.Status = "E";
                        this.response.Message = $"Error: {response.ReasonPhrase}";
                    }
                }
                catch (HttpRequestException ex)
                {
                    this.response.Status = "E";
                    this.response.Message = $"Request failed: {ex.Message}";
                }
            }

            return Json(new { success = this.response.Success, message = this.response.Message, Data = songDataList });
        }

        [HttpPost("Spot/GetFavoriteAlbum1")]
        public async Task<IActionResult> GetFavoriteAlbum(SpotSidebarModel albumData)
        {
            //SongData.FavoriteDate = null;
            //SongData.CreateSongDate = null;
            albumData.UserId = Convert.ToInt32(HttpContext.Session.GetString("UserId") ?? Request.Cookies["UserId"]);
            List<SpotSidebarModel> albumDataList = new List<SpotSidebarModel>();

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

                    var response = await client.PostAsync("/api/Spot/GetFavoriteAlbum", httpContent);
                    if (response.IsSuccessStatusCode)
                    {
                        albumDataList = await response.Content.ReadAsAsync<List<SpotSidebarModel>>();

                        if (albumDataList.Count > 0)
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
            return Json(new { success = this.response.Success, message = this.response.Message, Data = albumDataList });
        }


        [HttpPost("Spot/GetFavoriteArtist1")]
        public async Task<IActionResult> GetFavoriteArtist(SpotSidebarModel artistData)
        {
            //SongData.FavoriteDate = null;
            //SongData.CreateSongDate = null;
            artistData.UserId = Convert.ToInt32(HttpContext.Session.GetString("UserId") ?? Request.Cookies["UserId"]);
            
            List<SpotSidebarModel> artistDataList = new List<SpotSidebarModel>();

            using (HttpClientHandler handler = new HttpClientHandler())
            {
                // Temporarily bypass SSL certificate validation (not for production use)
                handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;

                var client = _httpClientFactory.CreateClient();
                client.BaseAddress = new Uri("https://localhost:7061/");
                try
                {

                    string requestJson = JsonConvert.SerializeObject(artistData);
                    HttpContent httpContent = new StringContent(requestJson, Encoding.UTF8, "application/json");

                    var response = await client.PostAsync("/api/Spot/GetFavoriteArtist", httpContent);
                    if (response.IsSuccessStatusCode)
                    {
                        artistDataList = await response.Content.ReadAsAsync<List<SpotSidebarModel>>();

                        if (artistDataList.Count > 0)
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
            return Json(new { success = this.response.Success, message = this.response.Message, Data = artistDataList });
        }


        [HttpPost("Spot/GetFavoriteSongs1")]
        public async Task<IActionResult> GetFavoriteSongs(SongModel SongData)
        {
            SongData.FavoriteDate = null;
            SongData.CreateSongDate = null;
            List<SongModel> songDataList = new List<SongModel>();

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

                    var response = await client.PostAsync("/api/Spot/GetFavoriteSongs", httpContent);
                    if (response.IsSuccessStatusCode)
                    {
                        songDataList = await response.Content.ReadAsAsync<List<SongModel>>();

                        if (songDataList.Count > 0)
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
            return Json(new { success = this.response.Success, message = this.response.Message, Data = songDataList });
        }



        [HttpPost("Spot/SearchDataFromGenre1")]
        public async Task<IActionResult> SearchDataFromGenre(AlbumModel albumData)
        {
            ResponseModel resp = new ResponseModel();
            List<AlbumModel> albumList = new List<AlbumModel>();
            using (HttpClientHandler handler = new HttpClientHandler())
            {
                // Temporarily bypass SSL certificate validation (not for production use)
                handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;

                var client = _apiHelper.CreateClient();
                client.BaseAddress = new Uri("https://localhost:7061/");


                try
                {
                    string requestJson = JsonConvert.SerializeObject(albumData);
                    HttpContent httpContent = new StringContent(requestJson, Encoding.UTF8, "application/json");


                    var response = await client.PostAsync("/api/Spot/SearchDataFromGenre", httpContent);
                    if (response.IsSuccessStatusCode)
                    {
                        albumList = await response.Content.ReadAsAsync<List<AlbumModel>>();

                        if (albumList.Count > 0)
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

            return Json(new { status = resp.Status, success = resp.Success, message = resp.Message, Data = albumList });
        }


        [HttpPost("Spot/SearchGenre1")]
        public async Task<IActionResult> SearchGenre(GenreModel genreData)
        {
           
            ResponseModel resp = new ResponseModel();
            List<GenreModel> genreList = new List<GenreModel>();
            using (HttpClientHandler handler = new HttpClientHandler())
            {
                // Temporarily bypass SSL certificate validation (not for production use)
                handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;

                var client = _httpClientFactory.CreateClient();
                client.BaseAddress = new Uri("https://localhost:7061/");


                try
                {
                    string requestJson = JsonConvert.SerializeObject(genreData);
                    HttpContent httpContent = new StringContent(requestJson, Encoding.UTF8, "application/json");
                    

                    var response = await client.PostAsync("/api/Spot/SearchGenre", httpContent);
                    if (response.IsSuccessStatusCode)
                    {
                        genreList = await response.Content.ReadAsAsync<List<GenreModel>>();

                        if (genreList.Count > 0)
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

            return Json(new { status = resp.Status, success = resp.Success, message = resp.Message, Data = genreList });
        }


        [HttpPost("Spot/SearchArtist1")]
        public async Task<IActionResult> SearchArtist(ArtistModel artistData)
        {
            ResponseModel resp = new ResponseModel();
            List<ArtistModel> artistList = new List<ArtistModel>();
            using (HttpClientHandler handler = new HttpClientHandler())
            {
                // Temporarily bypass SSL certificate validation (not for production use)
                handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;

                var client = _httpClientFactory.CreateClient();
                client.BaseAddress = new Uri("https://localhost:7061/");


                try
                {
                    string requestJson = JsonConvert.SerializeObject(artistData);
                    HttpContent httpContent = new StringContent(requestJson, Encoding.UTF8, "application/json");

                    var response = await client.PostAsync("/api/Spot/SearchArtist", httpContent);
                    if (response.IsSuccessStatusCode)
                    {
                        artistList = await response.Content.ReadAsAsync<List<ArtistModel>>();

                        if (artistList.Count > 0)
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

            return Json(new { status = resp.Status, success = resp.Success, message = resp.Message, Data = artistList });
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

    }


}
