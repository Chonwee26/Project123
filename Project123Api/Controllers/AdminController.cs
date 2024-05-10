using Microsoft.AspNetCore.Mvc;

using Project123Api.Database;
using Project123Api.Models;
using System.Security.Claims;
using AuthenticationPlugin;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Project123Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
   
    public class AdminController : ControllerBase 
       
    {
        private DataDbContext _dbContext;
        private IConfiguration _configuration;
        private readonly AuthService _auth;


        public AdminController(DataDbContext dbContext, IConfiguration configuration)
        {
            _configuration = configuration;
            _dbContext = dbContext;
            _auth = new AuthService(_configuration);

        }

        [HttpPost("Login")]
        public IActionResult Login([FromBody] AdminModel admin)
         {
            var adminEmail = _dbContext.Tb_Admin.FirstOrDefault(a => a.Email == admin.Email);

            if (adminEmail == null)
            {
                return NotFound("Not found Email");
            }
            if (!SecurePasswordHasherHelper.Verify(admin.Password, adminEmail.Password))
            {
                return Unauthorized("Can't login");
            }

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Email, admin.Email),
                new Claim(ClaimTypes.Email, admin.Email),
            };

            var token = _auth.GenerateAccessToken(claims);

            return new ObjectResult(new
            {
                access_token = token.AccessToken,
                expires_in = token.ExpiresIn,
                token_type = token.TokenType,
                creation_Time = token.ValidFrom,
                user_id = admin.Id,
            });


            //     var claims = new[]
            //{

            //     new Claim(ClaimTypes.Email, admin.Email),
            // };

            //     var tokenHandler = new JwtSecurityTokenHandler();
            //     var key = Encoding.ASCII.GetBytes("your_secret_key_which_should_be_at_least_128_bits_long"); // Change this to your secret key
            //     var tokenDescriptor = new SecurityTokenDescriptor
            //     {
            //         Subject = new ClaimsIdentity(claims),
            //         Expires = DateTime.UtcNow.AddHours(1), // Set expiration time
            //         SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            //     };

            //     var token = tokenHandler.CreateToken(tokenDescriptor);
            //     var tokenString = tokenHandler.WriteToken(token);

            //     return new ObjectResult(new
            //     {
            //         access_token = tokenString,
            //         expires_in = tokenDescriptor.Expires,
            //         token_type = "Bearer",
            //         creation_Time = token.ValidFrom,
            //         user_id = adminEmail.Id,
            //     });

        }

        [HttpPost("Register")]
        public IActionResult Register([FromBody] AdminModel admin) 
        {
            var adminWithEmail = _dbContext.Tb_Admin.Where(a => a.Email == admin.Email).SingleOrDefault();
            if (adminWithEmail != null) {
                return BadRequest("User with same email already exists");
            }
            var adminObj = new AdminModel
            {
                Name = admin.Name,
                Email = admin.Email,
                Password = SecurePasswordHasherHelper.Hash(admin.Password),
                Role = admin.Role
            };                 
            _dbContext.Tb_Admin.Add(adminObj);
            _dbContext.SaveChanges();
            return StatusCode(StatusCodes.Status201Created);
        }
    }
}
