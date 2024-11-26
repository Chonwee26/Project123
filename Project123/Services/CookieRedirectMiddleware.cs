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

        //public async Task InvokeAsync(HttpContext context)
        //{
        //    // Redirect to login if accessing protected pages without a UserToken cookie
        //    if (context.Request.Path.StartsWithSegments("/home/index") &&
        //        string.IsNullOrEmpty(context.Request.Cookies["UserToken"]))
        //    {
        //        context.Response.Redirect("/admin/login");
        //        return;
        //    }

        //    await _next(context);
        //}
    }
}