﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;                                                                        


using Project123Api.Database;
using Project123Api.Models;
using System.Security.Claims;
using AuthenticationPlugin;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Project123Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]


    public class BlogController : ControllerBase 
    {
        private DataDbContext _dbContext;

        public BlogController(DataDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        [HttpGet]
        public IActionResult Get()
        {


            return Ok("hi");
        }


        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var user = _dbContext.Tb_User.Find(id);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

    }
}