using Microsoft.AspNetCore.Mvc;
using Project123.Dto;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using AuthenticationPlugin;
using Project123Api.Repositories;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using Project123Api.Services;

namespace Project123Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenController : ControllerBase
    {
        private readonly IAdminRepository _adminRepo;
        private readonly IAuthenRepository _authenRepo;
     
        private DataDbContext _dbContext;
        private IConfiguration _configuration;
        private readonly AuthService _auth;
        

        public AuthenController(DataDbContext dbContext, IConfiguration configuration, IAdminRepository adminRepository , IAuthenRepository authenRepository)
        {
            _configuration = configuration;
            _dbContext = dbContext;
            _auth = new AuthService(_configuration);
            _adminRepo = adminRepository;
            _authenRepo = authenRepository;
           
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login1(AdminModel admin)
        {
            ResponseModel resp = new ResponseModel();

            try
            {
                if (admin == null || string.IsNullOrEmpty(admin.Email) || string.IsNullOrEmpty(admin.Password) )
                {
                    resp.Status = "E";
                    resp.Message = "Invalid input data.";
                    return Ok(resp);
                }

                if (_dbContext == null || _dbContext.Tb_Admin == null)
                {
                    resp.Status = "E";
                    resp.Message = "Database context or Admin table is not available.";
                    return Ok(resp);
                }

                var adminEmail = await _dbContext.Tb_Admin.FirstOrDefaultAsync(a => a.Email == admin.Email);
             
                if (adminEmail == null || string.IsNullOrEmpty(adminEmail.Email) || string.IsNullOrEmpty(adminEmail.Role))
                {
                    resp.Status = "E";
                    resp.Message = "Invalid input data.";
                    return Ok(resp);
                }

                if (adminEmail == null || !SecurePasswordHasherHelper.Verify(admin.Password, adminEmail.Password))
                {
                    resp.Status = "E";
                    resp.Message = "Invalid email or password.";
                    return Ok(resp);
                }

                // Calculate ExpireDate based on RememberMe
                DateTime expireDate = admin.RememberMe ? DateTime.UtcNow.AddDays(1) : DateTime.UtcNow.AddHours(1);
                // Update ExpireDate in the database
                adminEmail.ExpireDate = expireDate;
                _dbContext.Tb_Admin.Update(adminEmail);
                await _dbContext.SaveChangesAsync();


                var claims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Email, admin.Email),
                    new Claim(ClaimTypes.Email, admin.Email),
                    new Claim(ClaimTypes.Role, adminEmail.Role),
                    new Claim("UserId", adminEmail.Id.ToString())
                };
                foreach (var claim in User.Claims)
                {
                    Console.WriteLine($"Claim Type: {claim.Type}, Claim Value: {claim.Value}");
                }

                var token = _auth.GenerateAccessToken(claims);

                resp.Status = "S";
                resp.Message = "Authentication successful";

              
               // Redirect to trigger cookie persistence
                return Ok(new
                {
                    status = resp.Status,
                    message = resp.Message,
                    access_token = token.AccessToken,
                    expires_in = token.ExpiresIn,
                    token_type = token.TokenType,
                    creation_time = token.ValidFrom,
                    user_id = adminEmail.Id,
                    user_roles = adminEmail.Role
                });
            }
            catch (Exception ex)
            {
                resp.Status = "E";
                resp.Message = $"An error occurred: {ex.Message}";
                return Ok(resp);
            }
        }

        [HttpPost("Register")]
        public IActionResult Register1([FromBody] AdminModel admin)
        {
            if (_dbContext == null || _dbContext.Tb_Admin == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Database context is not available.");
            }

            var adminWithEmail = _dbContext.Tb_Admin.SingleOrDefault(a => a.Email == admin.Email);
            if (adminWithEmail != null)
            {
                return BadRequest("User with the same email already exists");
            }

            var adminObj = new AdminModel
            {
                Name = admin.Name,
                Email = admin.Email,
                Password = SecurePasswordHasherHelper.Hash(admin.Password),
                Role = admin.Role,
                Age = admin.Age
            };

            _dbContext.Tb_Admin.Add(adminObj);
            _dbContext.SaveChanges();

            return StatusCode(StatusCodes.Status201Created);
        }

        [HttpPost("ChangePassword")]
        public async Task<ResponseModel> ChangePassword(AdminModel userData)
        {
            ResponseModel response = new ResponseModel();

            try
            {
                response = await _authenRepo.ChangePassword(userData);
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


        [HttpPost("ChangePasswordByToken")]
        public async Task<ResponseModel> ChangePasswordByToken(AdminModel userData, string token)
        {
            ResponseModel response = new ResponseModel();

            try
            {
                response = await _authenRepo.ChangePasswordByToken(userData,token);
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

        [HttpPost("ForgetPassword")]
        public async Task<ResponseModel> ForgetPassword(AdminModel userData)
        {
            ResponseModel response = new ResponseModel();

            try
            {
                response = await _authenRepo.ForgetPassword(userData);
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

        [HttpPost("Logout")]
        public IActionResult Logout()
        {
            // Clear session or any user-specific tokens
            HttpContext.Session.Clear();
            return Ok(new { status = "S", message = "Logged out successfully" });
        }
    }
}
