﻿using Microsoft.AspNetCore.Mvc;
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
using Azure;
using Microsoft.EntityFrameworkCore;

namespace Project123Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
 
    public class AdminController : BaseController      
    {
        private readonly IAdminRepository _adminRepo;
        private DataDbContext _dbContext;
        private IConfiguration _configuration;
        private readonly AuthService _auth;

        public AdminController(DataDbContext dbContext, IConfiguration configuration, IAdminRepository adminRepository)
        {
            _configuration = configuration;
            _dbContext = dbContext;
            _auth = new AuthService(_configuration);
            _adminRepo = adminRepository;
        }


        //[AllowAnonymous]
        //[HttpPost("login")]
        //public IActionResult Login([FromBody] LoginDto loginDto)
        //{
        //    var user = _dbContext.Tb_User.SingleOrDefault(u => u.Username == loginDto.Username && u.Password == loginDto.Password);

        //    if (user == null)
        //    {
        //        return Unauthorized();
        //    }

        //    var token = GenerateJwtToken(user.Id.ToString());
        //    return Ok(new { Token = token });


        //}


        [HttpPost("Login1")]
        public async Task<IActionResult> Login1(AdminModel admin)
        {
            ResponseModel resp = new ResponseModel();

            try
            {
                // Ensure _dbContext and Tb_Admin are not null
                if (_dbContext == null || _dbContext.Tb_Admin == null)
                {
                    resp.Status = "E";
                    resp.Message = "An error occurred: Database context is not available.";
                    return Ok(resp); // Return an OkObjectResult for consistency
                }

                var adminEmail = await _dbContext.Tb_Admin.FirstOrDefaultAsync(a => a.Email == admin.Email);

                if (adminEmail == null || !SecurePasswordHasherHelper.Verify(admin.Password, adminEmail.Password))
                {
                    resp.Status = "E";
                    resp.Message = "Invalid email or password.";
                    return Ok(resp); // Return an OkObjectResult for consistency
                }

                var claims = new[]
                {
            new Claim(JwtRegisteredClaimNames.Email, admin.Email??string.Empty),
            new Claim(ClaimTypes.Email, admin.Email ?? string.Empty),
            new Claim(ClaimTypes.Role, adminEmail.Role ?? string.Empty) // Add the user's role to the claims
        };

                var token = _auth.GenerateAccessToken(claims);

                resp.Status = "S";
                resp.Message = "Authentication successful";

                // Return the result with the token details
                return Ok(new
                {
                    status = resp.Status,
                    message = resp.Message,
                    access_token = token.AccessToken,
                    expires_in = token.ExpiresIn,
                    token_type = token.TokenType,
                    creation_time = token.ValidFrom,
                    user_id = adminEmail.Id
                });
            }
            catch (Exception ex)
            {
                resp.Status = "E";
                resp.Message = $"An error occurred: {ex.Message}";
                return Ok(resp); // Return an OkObjectResult for consistency
            }
        }




        //[HttpPost("Login1")]
        //public async Task<ResponseModel> Login1(AdminModel admin)
        //{
        //    ResponseModel resp = new ResponseModel();

        //    try
        //    {
        //        var adminEmail = await _dbContext.Tb_Admin.FirstOrDefaultAsync(a => a.Email == admin.Email);

        //        if (adminEmail == null || !SecurePasswordHasherHelper.Verify(admin.Password, adminEmail.Password))
        //        {
        //            resp.Status = "E";
        //            resp.Message = "Invalid email or password.";
        //            return resp;
        //        }

        //        var claims = new[]
        //        {
        //    new Claim(JwtRegisteredClaimNames.Email, admin.Email),
        //    new Claim(ClaimTypes.Email, admin.Email),
        //};

        //        var token = _auth.GenerateAccessToken(claims);

        //        resp.Status = "S";
        //        resp.Message = "" + token.AccessToken;
        //        new ObjectResult(new
        //        {
        //            access_token = token.AccessToken,
        //            expires_in = token.ExpiresIn,
        //            token_type = token.TokenType,
        //            creation_time = token.ValidFrom,
        //            user_id = adminEmail.Id,
        //        });

        //        string userToken = token.AccessToken;

        //        HttpContext.Session.SetString("UserToken", userToken);
        //    }
        //    catch (Exception ex)
        //    {
        //        resp.Status = "E";
        //        resp.Message = $"An error occurred: {ex.Message}";
        //    }

        //    return resp;
        //}

        //[HttpPost("Login1")]
        //public IActionResult Login1([FromBody] AdminModel admin)
        //{
        //    var adminEmail = _dbContext.Tb_Admin.FirstOrDefault(a => a.Email == admin.Email);

        //    if (adminEmail == null)
        //    {
        //        return NotFound("Not found Email");
        //    }
        //    if (!SecurePasswordHasherHelper.Verify(admin.Password, adminEmail.Password))
        //    {
        //        return Unauthorized("Can't login");
        //    }

        //    var claims = new[]
        //    {
        //        new Claim(JwtRegisteredClaimNames.Email, admin.Email),
        //        new Claim(ClaimTypes.Email, admin.Email),
        //    };

        //    var token = _auth.GenerateAccessToken(claims);

        //    return new ObjectResult(new
        //    {
        //        access_token = token.AccessToken,
        //        expires_in = token.ExpiresIn,
        //        token_type = token.TokenType,
        //        creation_Time = token.ValidFrom,
        //        user_id = adminEmail.Id,
        //    });


        //}



        [HttpPost("Register1")]
        public IActionResult Register1([FromBody] AdminModel admin)
        {
            // Ensure _dbContext and Tb_Admin are not null
            if (_dbContext == null || _dbContext.Tb_Admin == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Database context is not available.");
            }

            var adminWithEmail = _dbContext.Tb_Admin.SingleOrDefault(a => a.Email == admin.Email);

            if (adminWithEmail != null)
            {
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


        [HttpPost("CreateUser123")]
        public async Task<ResponseModel> CreateUser(dataModel UserData)
        {
            ResponseModel response = new ResponseModel();

            try
                            {
                response = await _adminRepo.CreateUser(UserData);
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


       
        [HttpDelete("DeleteUser/{id}")]
        public async Task<ResponseModel> DeleteUser(int id)
        {
            ResponseModel resp = await _adminRepo.DeleteUser(id);
            return resp;


        }
        [Authorize(Roles = "Admin")]
        [HttpPost("SearchUser1")]
        public async Task<IEnumerable<dataModel>> SearchUser(dataModel UserData)
        {
            ResponseModel resp = new ResponseModel();
          

            //if (string.IsNullOrEmpty(token))
            //{
            //    resp.Status = "E";
            //    resp.Message = "Error";

            //    return (IEnumerable<ShipmentModel>)resp;
            //}


            IEnumerable<dataModel> userList = await _adminRepo.SearchUser(UserData);

            return userList;
        }
        // PUT api/<TestController>/5

        [HttpPost("Register2")]
        public async Task<ResponseModel> Register(AdminModel UserData)
        {
            ResponseModel response = new ResponseModel();

            try
            {
                response = await _adminRepo.Register(UserData);
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

        //[HttpPost("[action]")]
        //public async Task<ResponseModel> SearchUser(dataModel UserData)
        //{
        //    ResponseModel response = new ResponseModel();

        //    try
        //                    {
        //        response = await _adminRepo.SearchUser(UserData);
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




        //[HttpPost("[action]")]
        //public async Task<AdminModel> Login(AdminModel UserData)
        //{
        //    ResponseModel response = new ResponseModel();
        //    AdminModel admin = null; 
        //    try
        //    {
        //        response = await _adminRepo.Login(UserData);
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

        [HttpPost("[action]")]
        public ResponseModel Hello(dataModel UserData)
        {
            ResponseModel response = new ResponseModel();

            try
            {
                response.Status = "S";
                response.Message = "Success" + UserData;

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
