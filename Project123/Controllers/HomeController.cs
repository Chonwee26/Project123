using Microsoft.AspNetCore.Mvc;
//using Project123.Models;
using Project123.Dto;
using System.Diagnostics;

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
            return View();
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


    }
}