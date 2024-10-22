using Microsoft.AspNetCore.Authorization;
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

     
        [HttpPost("CreateGenre")]
        public async Task<ResponseModel> CreateGenre(GenreModel genreData)
        {
            ResponseModel response = new ResponseModel();

            try
            {
                response = await _spotRepo.CreateGenre(genreData);
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


        [HttpPost("CreateArtist")]
        public async Task<ResponseModel> CreateArtist(ArtistModel artistData)
        {
            ResponseModel response = new ResponseModel();

            try
            {
                response = await _spotRepo.CreateArtist(artistData);
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

        //[HttpPost("FaoriteSong")]
        //public async Task<ResponseModel> FaoriteSong(ArtistModel artistData)
        //{
        //    ResponseModel response = new ResponseModel();

        //    try
        //    {
        //        response = await _spotRepo.FaoriteSong(artistData);
        //        //response.Status = "S";
        //        //response.Message = "User created successfully.";
        //    }
        //    catch (Exception ex)
        //    {
        //        response.Message = ex.Message;
        //        response.Status = "E";
        //    }

        //    return response;
        //}


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


        [HttpPost("UpdateArtist")]
        public async Task<ResponseModel> UpdateArtist(ArtistModel artistData)
        {
            ResponseModel response = new ResponseModel();

            try
            {
                response = await _spotRepo.UpdateArtist(artistData);
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

        [HttpPost("UpdateGenre")]
        public async Task<ResponseModel> UpdateGenre(GenreModel genreData)
        {
            ResponseModel response = new ResponseModel();

            try
            {
                response = await _spotRepo.UpdateGenre(genreData);
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


        [HttpPost("DeleteArtist")]
        public async Task<ResponseModel> DeleteArtist(ArtistModel artistData)
        {
            ResponseModel resp = await _spotRepo.DeleteArtist(artistData);
            return resp;


        }



        [HttpPost("DeleteGenre")]
        public async Task<ResponseModel> DeleteGenre(GenreModel genreData)
        {
            ResponseModel resp = await _spotRepo.DeleteGenre(genreData);
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


        [HttpPost("GetFavoriteSongs")]
        public async Task<IEnumerable<SongModel>> GetFavoriteSongs(SongModel SongData)
        {
            ResponseModel resp = new ResponseModel();

            IEnumerable<SongModel> SongList = await _spotRepo.GetFavoriteSongs(SongData);

            return SongList;
        }


        [HttpPost("SearchSpot")]
        public async Task<IEnumerable<SearchSpotModal>> SearchSpot(SearchSpotModal searchData)
        {
            ResponseModel resp = new ResponseModel();

            IEnumerable<SearchSpotModal> searchSpotList = await _spotRepo.SearchSpot(searchData);

            return searchSpotList;
        }


        [HttpPost("SearchAlbum")]
        public async Task<IEnumerable<AlbumModel>> SearchAlbum(AlbumModel AlbumData)
        {
            ResponseModel resp = new ResponseModel();

            IEnumerable<AlbumModel> albumList = await _spotRepo.SearchAlbum(AlbumData);

            return albumList;
        }

        [HttpPost("SearchArtist")]
        public async Task<IEnumerable<ArtistModel>> SearchArtist(ArtistModel artistData)
        {
            ResponseModel resp = new ResponseModel();

            IEnumerable<ArtistModel> artistList = await _spotRepo.SearchArtist(artistData);

            return artistList;
        }
      

        [HttpPost("SearchGenre")]
        public async Task<IEnumerable<GenreModel>> SearchGenre(GenreModel genreData)
        {
            ResponseModel resp = new ResponseModel();

            IEnumerable<GenreModel> genreList = await _spotRepo.SearchGenre(genreData);

            return genreList;
        }
    }
}
