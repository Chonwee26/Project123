﻿using Microsoft.AspNetCore.Mvc;
//using Project123Api.Models;
using Project123.Dto;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Project123Api.Repositories;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Project123Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private DataDbContext _dbContext;

        public UserController(DataDbContext dbContext) 
        { 
            _dbContext = dbContext; 
        }

        // GET: api/<ValuesController>
        [HttpGet]
        public IActionResult Get()
        {

            return Ok(_dbContext.Tb_User);
        }

        // GET api/<ValuesController>/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            if (_dbContext.Tb_User == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Database context is not available.");
            }

            var user = _dbContext.Tb_User.Find(id);

            if (user == null)
            {
                return NotFound(); 
            }

            return Ok(user); 
        }


        // POST api/<ValuesController>
        [HttpPost]
        public IActionResult Post([FromBody] dataModel userObj)
        {
            if (_dbContext.Tb_User == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Database context is not available.");
            }

            _dbContext.Tb_User.Add(userObj);
            _dbContext.SaveChanges();
            return StatusCode(StatusCodes.Status201Created);
        }

        // PUT api/<ValuesController>/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] dataModel userObj)
        {
            if (_dbContext.Tb_User == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Database context is not available.");
            }

            var user = _dbContext.Tb_User.Find(id);
            if (user == null)
            {
                return NotFound("Cant't update user.");
            }
            else
            {
                user.Name = userObj.Name;
                user.Age = userObj.Age;
                _dbContext.SaveChanges();
            }         
            return Ok("Update User Success");
        }

        // DELETE api/<ValuesController>/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            if (_dbContext.Tb_User == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Database context is not available.");
            }

            var user = _dbContext.Tb_User.Find(id);
            if (user == null)
            {
                return NotFound("Can't delete user.");
            }
            else
            {
                _dbContext.Tb_User.Remove(user);
                _dbContext.SaveChanges();
            }
          return Ok("Delete Success");
        }
    }
}
