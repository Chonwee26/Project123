using Microsoft.EntityFrameworkCore;

using System.Text.Json.Serialization;
//using Project123.Data;
using Project123Api.Repositories;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();
builder.Services.AddControllersWithViews().AddJsonOptions(options => {

    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    options.JsonSerializerOptions.PropertyNamingPolicy = null;
});

builder.Services.AddDbContext<DataDbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IAdminRepository, AdminRepository>();
builder.Services.AddScoped<IShipmentRepository, ShipmentRepository>();

builder.Services.AddHttpClient();
builder.Services.AddControllersWithViews();
builder.Services.AddControllers();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Issuer"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Tokens:Key"]))
        };
    });
// Add authorization services
builder.Services.AddAuthorization();



builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
       builder => builder.WithOrigins("https://localhost:7166")
       .AllowAnyMethod()
       .AllowAnyHeader());
});


builder.Services.AddHttpClient("BaseClient", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["BaseAddress"]);
    client.DefaultRequestHeaders.Accept.Clear();
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
});

var app = builder.Build();

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
app.UseCors("AllowSpecificOrigin");
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication(); // Enable authentication middleware
app.UseAuthorization(); // Enable authorization middleware

app.MapControllers();
//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Shipment}/{action=SearchShipment}/{id?}");

app.UseEndpoints(endpoints =>
{
    // Web routes
    endpoints.MapControllerRoute(
        name: "default",
     pattern: "{controller=Home}/{action=Index}/{id?}");

    // API routes
    endpoints.MapControllerRoute(
        name: "api",
        pattern: "api/{controller}/{action}/{id?}");

    //endpoints.MapControllerRoute(
    //   name: "test",
    //   pattern: "Test/{action=GetShipmentLocationAsync}",
    //   defaults: new { controller = "Test", action = "GetShipmentLocationAsync" });
});

app.Run();
