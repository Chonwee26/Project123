using Microsoft.AspNetCore.Mvc;
using Project123.Dto;

namespace Project123.Controllers
{
    public class BaseController : Controller
    {
        protected ResponseModel response;
        protected string[] uriParams;
        public BaseController()
        {
            response = new ResponseModel();
            uriParams = new string[0];
        }
    }
}
