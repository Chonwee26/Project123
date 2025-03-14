using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Project123Api.Repositories;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using static Project123.Services.IAuthenticationService;
using static Project123.Services.CookieRedirectMiddleware;
using Project123.Services;
using Microsoft.AspNetCore.SignalR;

using Project123Api.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("Project123Settings.json", optional: false, reloadOnChange: true) // Main settings file
    .AddJsonFile($"Project123Settings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true) // Environment-specific settings
    .AddEnvironmentVariables();

// Register ApiHelper
builder.Services.AddSingleton<ApiHelper>();
builder.Services.AddSingleton<MyService>();
//builder.Services.AddSingleton<EmailService>();

// Register IHttpContextAccessor (needed to access session)
builder.Services.AddHttpContextAccessor();

// Add services to the container.
//builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();
//builder.Services.AddControllersWithViews().AddJsonOptions(options =>
//{
//    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
//    options.JsonSerializerOptions.PropertyNamingPolicy = null;
//});

builder.Services.AddControllersWithViews()
    .AddRazorRuntimeCompilation()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
    });

builder.Services.AddDbContext<DataDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IAdminRepository, AdminRepository>();
builder.Services.AddScoped<IShipmentRepository, ShipmentRepository>();


builder.Services.AddHttpClient();
builder.Services.AddSignalR();
//builder.Services.AddControllersWithViews();
//builder.Services.AddControllers();

// Configure JWT authentication
var key = builder.Configuration.GetValue<string>("Tokens:Key");
var issuer = builder.Configuration.GetValue<string>("Tokens:Issuer");
var accessExpireSeconds = builder.Configuration.GetValue<int>("Tokens:AccessExpireSeconds");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
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
        ClockSkew = TimeSpan.Zero,
    };
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine("Authentication failed: " + context.Exception.Message);
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                Console.WriteLine("Token validated for user: " + context?.Principal?.Identity?.Name);
                return Task.CompletedTask;
            }
        };
    });

// Add authorization services
builder.Services.AddAuthorization();

// Add CORS policy
//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("AllowSpecificOrigin",
//       builder => builder.WithOrigins("https://localhost:7166")
//       .AllowAnyMethod()
//       .AllowAnyHeader());
//});
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", policy =>
        policy.AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader());
});


// Add HttpClient configuration
builder.Services.AddHttpClient("BaseClient", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["BaseAddress"]);
    client.DefaultRequestHeaders.Accept.Clear();
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
});

// Add session services
builder.Services.AddDistributedMemoryCache(); // Required for session storage
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(120); // Set session timeout //FromHours(2);
    options.Cookie.HttpOnly = true; // Set cookie as HttpOnly
    options.Cookie.IsEssential = true; // Make the session cookie essential
});
builder.Services.AddAuthentication("Cookies").AddCookie(); // If using cookies for authentication



var app = builder.Build();


//app.Use(async (context, next) =>
//{
//    // Check for "UserToken" cookie
//    var userToken = context.Request.Cookies["UserToken"];

//    if (string.IsNullOrEmpty(userToken) && !context.Request.Path.StartsWithSegments("/Admin/LoginPage"))
//    {
//        // If no cookie, redirect to login page
//        context.Response.Redirect("/Admin/LoginPage");
//    }
//    else
//    {
//        // If cookie exists, proceed to the next middleware
//        await next();
//    }
//});


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}




app.UseRouting();
app.UseCors("AllowSpecificOrigin");
//app.UseCors("AllowAll");
app.MapHub<NotificationHub>("/notificationHub");
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();



app.UseAuthentication(); // Enable authentication middleware
app.UseAuthorization();   // Enable authorization middleware


// Enable session middleware

//app.UseMiddleware<CookieRedirectMiddleware>(); // Add your custom middleware

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "api",
    pattern: "api/{controller}/{action}/{id?}");

app.MapControllers();


app.Run();
