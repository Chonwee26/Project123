//using Microsoft.AspNetCore.Mvc;
//using Project123.Dto;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Http;
//using AuthenticationPlugin;
//using Project123Api.Repositories;
//using System.Security.Claims;

//namespace Project123Api.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class AuthenController : ControllerBase
//    {
//        private readonly IAdminRepository _adminRepo;
//        private DataDbContext _dbContext;
//        private IConfiguration _configuration;
//        private readonly AuthService _auth;

//        public AuthenController(DataDbContext dbContext, IConfiguration configuration, IAdminRepository adminRepository)
//        {
//            _configuration = configuration;
//            _dbContext = dbContext;
//            _auth = new AuthService(_configuration);
//            _adminRepo = adminRepository;
//        }

//        [HttpPost("Login1")]
//        public async Task<IActionResult> Login1(AdminModel admin)
//        {
//            ResponseModel resp = new ResponseModel();

//            try
//            {
//                var adminEmail = await _dbContext.Tb_Admin.FirstOrDefaultAsync(a => a.Email == admin.Email);

//                if (adminEmail == null || !SecurePasswordHasherHelper.Verify(admin.Password, adminEmail.Password))
//                {
//                    resp.Status = "E";
//                    resp.Message = "Invalid email or password.";
//                    return Ok(resp);
//                }

//                var claims = new[]
//                {
//                    new Claim(JwtRegisteredClaimNames.Email, admin.Email),
//                    new Claim(ClaimTypes.Email, admin.Email),
//                    new Claim(ClaimTypes.Role, adminEmail.Role)
//                };

//                var token = _auth.GenerateAccessToken(claims);

//                resp.Status = "S";
//                resp.Message = "Authentication successful";

//                return Ok(new
//                {
//                    status = resp.Status,
//                    message = resp.Message,
//                    access_token = token.AccessToken,
//                    expires_in = token.ExpiresIn,
//                    token_type = token.TokenType,
//                    creation_time = token.ValidFrom,
//                    user_id = adminEmail.Id
//                });
//            }
//            catch (Exception ex)
//            {
//                resp.Status = "E";
//                resp.Message = $"An error occurred: {ex.Message}";
//                return Ok(resp);
//            }
//        }

//        [HttpPost("Register1")]
//        public IActionResult Register1([FromBody] AdminModel admin)
//        {
//            if (_dbContext == null || _dbContext.Tb_Admin == null)
//            {
//                return StatusCode(StatusCodes.Status500InternalServerError, "Database context is not available.");
//            }

//            var adminWithEmail = _dbContext.Tb_Admin.SingleOrDefault(a => a.Email == admin.Email);
//            if (adminWithEmail != null)
//            {
//                return BadRequest("User with the same email already exists");
//            }

//            var adminObj = new AdminModel
//            {
//                Name = admin.Name,
//                Email = admin.Email,
//                Password = SecurePasswordHasherHelper.Hash(admin.Password),
//                Role = admin.Role
//            };

//            _dbContext.Tb_Admin.Add(adminObj);
//            _dbContext.SaveChanges();

//            return StatusCode(StatusCodes.Status201Created);
//        }

//        [HttpPost("Logout")]
//        public IActionResult Logout()
//        {
//            // Clear session or any user-specific tokens
//            HttpContext.Session.Clear();
//            return Ok(new { status = "S", message = "Logged out successfully" });
//        }
//    }
//}
