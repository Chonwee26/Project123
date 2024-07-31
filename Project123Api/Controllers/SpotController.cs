using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project123.Dto;
using Project123Api.Repositories;

namespace Project123Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SpotController : BaseController
    {

        private readonly ISpotRepository _spotRepo;
        private DataDbContext _dbContext;
        private IConfiguration _configuration;
       

        public SpotController(DataDbContext dbContext, IConfiguration configuration, ISpotRepository spotRepository)
        {
            _configuration = configuration;
            _dbContext = dbContext;
           
            _spotRepo = spotRepository;
        }

      

        [HttpPost("CreateAlbum123")]
        public async Task<ResponseModel> CreateAlbum(AlbumModel AlbumData)
        {
            ResponseModel response = new ResponseModel();

            try
            {
                response = await _spotRepo.CreateAlbum(AlbumData);
                //response.Status = "S";
                //response.Message = "User created successfully.";
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                response.Status = "E";
            }

            return response;
        }


        [HttpPost("EditAlbum")]
        public async Task<ResponseModel> EditAlbum(AlbumModel AlbumData)
        {
            ResponseModel response = new ResponseModel();

            try
            {
                response = await _spotRepo.EditAlbum(AlbumData);
                //response.Status = "S";
                //response.Message = "User created successfully.";
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                response.Status = "E";
            }

            return response;
        }

        [HttpPost("CreateSong123")]
        public async Task<ResponseModel> CreateSong(SongModel SongData)
        {
            ResponseModel response = new ResponseModel();

            try
            {
                response = await _spotRepo.CreateSong(SongData);
                //response.Status = "S";
                //response.Message = "User created successfully.";
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                response.Status = "E";
            }

            return response;
        }
        [HttpPost("UpdateSong")]
        public async Task<ResponseModel> UpdateSong(SongModel SongData)
        {
            ResponseModel response = new ResponseModel();

            try
            {
                response = await _spotRepo.UpdateSong(SongData);
                //response.Status = "S";
                //response.Message = "User created successfully.";
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                response.Status = "E";
            }

            return response;
        }
        [HttpPost("RemoveSong")]
        public async Task<ResponseModel> RemoveSong(SongModel SongData)
        {
            ResponseModel response = new ResponseModel();

            try
            {
                response = await _spotRepo.RemoveSong(SongData);
                //response.Status = "S";
                //response.Message = "User created successfully.";
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                response.Status = "E";
            }

            return response;
        }


        [HttpPost("DeleteSong")]
        public async Task<ResponseModel> DeleteSong(SongModel SongData)
        {
            ResponseModel resp = await _spotRepo.DeleteSong(SongData);
            return resp;


        }

        [HttpPost("DeleteAlbum")]
        public async Task<ResponseModel> DeleteAlbum(AlbumModel AlbumData)
        {
            ResponseModel resp = await _spotRepo.DeleteAlbum(AlbumData);
            return resp;


        }



        [HttpPost("SearchSong")]
        public async Task<IEnumerable<SongModel>> SearchSong(SongModel SongData)
        {
            ResponseModel resp = new ResponseModel();      
      
            IEnumerable<SongModel> songList = await _spotRepo.SearchSong(SongData);

            return songList;
        }


        [HttpPost("SearchSongNotInAlbum")]
        public async Task<IEnumerable<SongModel>> SearchSongNotInAlbum(SongModel SongData)
        {
            ResponseModel resp = new ResponseModel();

            IEnumerable<SongModel> songList = await _spotRepo.SearchSongNotInAlbum(SongData);

            return songList;
        }


        [HttpPost("GetAlbum")]
        public async Task<IEnumerable<AlbumModel>> GetAlbum(AlbumModel AlbumData)
        {
            ResponseModel resp = new ResponseModel();

            IEnumerable<AlbumModel> albumList = await _spotRepo.GetAlbum(AlbumData);

            return albumList;
        }


        [HttpPost("SearchAlbum")]
        public async Task<IEnumerable<AlbumModel>> SearchAlbum(AlbumModel AlbumData)
        {
            ResponseModel resp = new ResponseModel();

            IEnumerable<AlbumModel> albumList = await _spotRepo.SearchAlbum(AlbumData);

            return albumList;
        }
    }
}
