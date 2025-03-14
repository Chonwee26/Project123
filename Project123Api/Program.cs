using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
//using Project123Api.Models;
using System.Xml.Linq;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using AuthenticationPlugin;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Http;
using Project123Api.Repositories;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Project123Api.Middlewares;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Authentication.Cookies;


using Project123Api.Services;
using Project123Api.Hubs;
using Microsoft.AspNetCore.SignalR;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("Project123ApiSettings.json", optional: false, reloadOnChange: true) // Main settings file
    .AddJsonFile($"Project123ApiSettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true) // Environment-specific settings
    .AddEnvironmentVariables();

//builder.Services.AddControllersWithViews();
builder.Services.AddControllers();


//builder.Services.AddSession();

builder.Services.AddHttpContextAccessor();
// Add logging services
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.AddEventLog();
// Add services to the container.
builder.Services.AddHttpClient();
builder.Logging.SetMinimumLevel(LogLevel.Information); // Set minimum logging level

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(120); // Set the timeout as needed
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
}); // Add this line
builder.Services.AddDistributedMemoryCache(); // Add this line for in-memory cache



builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Project123 API", Version = "v1" });
});
//builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<DataDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IAdminRepository, AdminRepository>();
builder.Services.AddScoped<IShipmentRepository, ShipmentRepository>();
builder.Services.AddScoped<ISpotRepository, SpotRepository>();
builder.Services.AddScoped<IAuthenRepository, AuthenRepository>();
builder.Services.AddSingleton<EmailService>();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddSingleton<IUserIdProvider, CustomUserIdProvider>(); // Register user ID provider
builder.Services.AddSignalR();

//var key = builder.Configuration.GetValue<string>("Tokens:Key");

//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//    .AddJwtBearer(options =>
//    {
//        options.TokenValidationParameters = new TokenValidationParameters
//        {
//            ValidateIssuer = true,
//            ValidateAudience = true,
//            ValidateLifetime = true,
//            ValidateIssuerSigningKey = true,
//            ValidIssuer = builder.Configuration["Tokens:Issuer"],
//            ValidAudience = builder.Configuration["Tokens:Issuer"],
//            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
//            ClockSkew = TimeSpan.Zero,
//        };
//    });


var key = builder.Configuration.GetValue<string>("Tokens:Key");
var issuer = builder.Configuration.GetValue<string>("Tokens:Issuer");
var accessExpireSeconds = builder.Configuration.GetValue<int>("Tokens:AccessExpireSeconds");


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;

        options.TokenValidationParameters = new TokenValidationParameters

 

        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            ValidAudience = issuer,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
            RoleClaimType = ClaimTypes.Role, // Specify that the role is included in the token
            ClockSkew = TimeSpan.Zero
           
        };
        options.TokenValidationParameters.RoleClaimType = ClaimTypes.Role;

    });


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
{
    options.LoginPath = "/Admin/LoginPage";
    options.ExpireTimeSpan = TimeSpan.FromDays(30);
    options.SlidingExpiration = true;
});
//builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
//    .AddCookie(options =>
//    {
//        options.LoginPath = "/Admin/LoginPage"; // Path to the login page
//        options.ExpireTimeSpan = TimeSpan.FromDays(30); // Set a long expiration for "Remember Me"
//        options.SlidingExpiration = false; // Prevents resetting expiration on every request

//    });

//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("AllowSpecificOrigin",
//       builder => builder.WithOrigins("https://localhost:7166")
//       .AllowAnyMethod()
//       .AllowAnyHeader());
//});


// allow anyorigin
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", policy =>
        policy.AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader());
});

builder.Services.AddAuthorization();


var app = builder.Build();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Project123 API v1"));
}






app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseCors("AllowSpecificOrigin");


app.UseSession(); // Add this line


app.UseAuthentication();

app.UseAuthorization();

app.MapHub<NotificationHub>("/notificationHub");

app.MapControllers();

app.Run();