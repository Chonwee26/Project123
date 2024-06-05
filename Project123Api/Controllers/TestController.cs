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
        [HttpGet("GetShipmentLocationAsync")]
        public async Task<IEnumerable<ShipmentLocationModel>> GetShipmentLocationAsync()
        {
            IEnumerable<ShipmentLocationModel> dataModel = await _shipmentRepo.GetShipmentLocationAsync();
            return await Task.FromResult(dataModel);
        }  
        
        [HttpGet("GetShipmentStatusAsync")]
        public async Task<IEnumerable<ShipmentLocationModel>> GetShipmentStatusAsync()
        {
            IEnumerable<ShipmentLocationModel> dataModel = await _shipmentRepo.GetShipmentStatusAsync();
            return await Task.FromResult(dataModel);
        }

        // POST api/<TestController>
        [HttpPost("SearchShipmentAsync")]
        public async Task<IEnumerable<ShipmentModel>> SearchShipmentAsync(ShipmentModel ShipmentData)
        {
            IEnumerable<ShipmentModel> shipmentList = await _shipmentRepo.SearchShipmentAsync(ShipmentData);

            return shipmentList;
        }
        // PUT api/<TestController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<TestController>/5
        [HttpDelete("DeleteShipmentAsync/{id}")]
        public async Task<ResponseModel> DeleteShipmentAsync(int id)
        {
            ResponseModel resp = await _shipmentRepo.DeleteShipmentAsync(id);
            return resp;


        }   // DELETE api/<TestController>/5
        [HttpPost("CreateShipmentAsync")]
        public async Task<ResponseModel> CreateShipmentAsync(ShipmentModel ShipmentData)
        {
            ShipmentData.OrderNumber = StringGenerator.GenerateRandomString(); //string
            ResponseModel resp  = await _shipmentRepo.CreateShipmentAsync(ShipmentData);
            return resp;
        }  
        
        [HttpPost("UpdateShipmentAsync")]
        public async Task<IEnumerable<ShipmentModel>> UpdateShipmentAsync(ShipmentModel ShipmentData)
        {
            
            IEnumerable<ShipmentModel> shipmentList = await _shipmentRepo.UpdateShipmentAsync(ShipmentData);
            return shipmentList;
        }

        public class StringGenerator
        {
            public static string GenerateRandomString()
            {
                // Define the characters from which to generate the random string
                const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
                int length = 10;
                // Create a StringBuilder to store the random string
                StringBuilder sb = new StringBuilder();

                // Create a Random object
                Random random = new Random();

                // Generate the random string
                for (int i = 0; i < length - 1; i++)
                {
                    // Append a random character from the 'chars' string
                    sb.Append(chars[random.Next(length)]);
                }
                sb.Append(random.Next(5));

                string orderNumber = sb.ToString();
                // Return the generated random string
                return orderNumber;
            }
        }

    }
}
