using Microsoft.AspNetCore.Mvc;
//using Project123.Models;
using Project123.Dto;
using System.Diagnostics;
using System.Security.Claims;
using Xunit;

namespace Project123.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult MusicPlayerPage()
        {
      
            return View();
        }

        public IActionResult MusicSpotPage()
        {

            if (HttpContext.Session.GetString("UserId") == null &&!HttpContext.Request.Cookies.ContainsKey("AuthToken"))
            {
                return RedirectToAction("LoginPage", "Admin");
            }
            string? userId = HttpContext.Session.GetString("UserId") ?? Request.Cookies["UserId"];
                 
            ViewBag.UserId = Convert.ToInt32(userId);
         
            return View();
          
        }

        public IActionResult GetUserId()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            // Do something with userId
           
            return Ok(userId);
        }

        public IActionResult ExportExcel()
        {
            
            return View();
        }


        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Calendar()
        {
            return View();
        }

        public IActionResult PictureInPicture()
        {
            return View();
        }
        public IActionResult DigitalArt()
        {
            return View();
        }

        public IActionResult PushState()
        {
            return View();
        }

        public IActionResult DragAndDrop()
        {
            return View();
        }

        public IActionResult TestCode()
        {
            return View();
        }

        public class Calculator
        {
            public int Add(int a, int b)
            {
                return a + b;
            }
        }

        public class CalculatorTests
        {
            [Fact] // This attribute marks the method as a test
            public void Add_ShouldReturnCorrectSum()
            {
                // Arrange
                var calculator = new Calculator();
                int num1 = 2;
                int num2 = 3;

                // Act
                int result = calculator.Add(num1, num2);

                // Assert
                Assert.Equal(5, result); // Checks if the result matches the expected value
            }
        }

    }
}