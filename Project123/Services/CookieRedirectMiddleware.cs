using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Project123.Services
{
    public class CookieRedirectMiddleware
    {
        private readonly RequestDelegate _next;

        public CookieRedirectMiddleware(RequestDelegate next)
        {
            _next = next;
        }

      
    }
}