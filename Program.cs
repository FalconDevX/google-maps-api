using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using WebAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient<GoogleMapsService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<TokenService>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<UserDb>(options =>
options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 43))));
builder.Services.Configure<RefreshTokenDto>(builder.Configuration.GetSection("JwtSettings"));
builder.Services.AddScoped<TokenService>();
builder.Services.AddSingleton<GoogleStorage>();

var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<RefreshTokenDto>();
var key = Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Key"]?? throw new ArgumentNullException("Key not find"));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = "MyApp",
        ValidAudience = "AppUsers",
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ClockSkew = TimeSpan.Zero
    };
});


//CORS for local development with frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.WithOrigins("http://localhost:5173") 
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "My API",
        Version = "v1.0",
        Description = "API documentation",
    });
});

var app = builder.Build();

app.MapGet("/", () => "API works");

app.UseSwagger(c =>
{
    c.RouteTemplate = "api/swagger/{documentName}/swagger.json";
});

app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/api/swagger/v1/swagger.json", "My API V1");
    c.RoutePrefix = "api/swagger";
    c.EnableDeepLinking();
    c.DisplayRequestDuration();
});

app.UseCors("AllowFrontend");

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();
app.Run();