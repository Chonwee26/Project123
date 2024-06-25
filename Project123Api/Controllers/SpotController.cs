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
    }
}
