using Microsoft.AspNetCore.Mvc;
using Project123Api.Database;
using Project123Api.Models;
namespace Project123Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase 
       
    {
        private DataDbContext _dbContext;
        public AdminController(DataDbContext dbContext)
        {
            _dbContext = dbContext;

        }
        [HttpPost]
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
                Password = admin.Password,
                Role = "Users"
            };                 
            _dbContext.Tb_Admin.Add(adminObj);
            _dbContext.SaveChanges();
            return StatusCode(StatusCodes.Status201Created);
        }
    }
}
