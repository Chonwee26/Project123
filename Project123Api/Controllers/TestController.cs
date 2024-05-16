using Microsoft.AspNetCore.Mvc;
//using Project123Api.Models;
using Project123.Dto;
using System.Security.Claims;
using AuthenticationPlugin;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Data.SqlClient;
using Project123Api.Repositories;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Project123Api.Controllers
{


    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly IShipmentRepository _shipmentRepo;
        private DataDbContext _dbContext;
        private IConfiguration _configuration;
        private readonly AuthService _auth;

        public TestController(DataDbContext dbContext, IConfiguration configuration, IShipmentRepository shipmentRepository)
        {
            _configuration = configuration;
            _dbContext = dbContext;
            _auth = new AuthService(_configuration);
            _shipmentRepo = shipmentRepository;
        }
        // GET: api/<TestController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<TestController>/5
        [HttpGet("[action]")]
        public async Task<IEnumerable<ShipmentLocationModel>> GetShipmentLocation()
        {
            IEnumerable<ShipmentLocationModel> dataModel = await _shipmentRepo.GetShipmentLocation();
            return await Task.FromResult(dataModel);
        }

        // POST api/<TestController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<TestController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<TestController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
