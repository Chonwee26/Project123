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

        public IActionResult Privacy()
        {
            return View();
        }

     
    }
}