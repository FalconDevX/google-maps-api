using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using WebAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using WebAPI.DTOs;
using System.IdentityModel.Tokens.Jwt;


var builder = WebApplication.CreateBuilder(args);
builder.Logging.AddDebug();
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
builder.Services.AddScoped<UserRecommendationService>();

var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<RefreshTokenDto>();
var key = Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Key"]?? throw new ArgumentNullException("Key not find"));

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

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
        ClockSkew = TimeSpan.Zero,

        NameClaimType = "name",

        RoleClaimType = "role"
    };


    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = async context =>
        {
            if (!context.Response.HasStarted)
            {
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = 401;

                if (context.Exception is SecurityTokenExpiredException)
                {
                    await context.Response.WriteAsJsonAsync(new { reason = "AccessTokenExpired" });
                }
                else
                {
                    await context.Response.WriteAsJsonAsync(new { reason = "InvalidToken" });
                }
            }
        },

        OnChallenge = async context =>
        {
            if (!context.Response.HasStarted)
            {
                context.HandleResponse();
                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";

                if (string.IsNullOrEmpty(context.Request.Headers["Authorization"]))
                {
                    await context.Response.WriteAsJsonAsync(new { reason = "MissingToken" });
                }
                else
                {
                    await context.Response.WriteAsJsonAsync(new { reason = "UnauthorizedAccess" });
                }
            }
        }
    };
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.WithOrigins("http://localhost:5173", "http://34.56.66.163")
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

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter: Bearer {your token}"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
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

app.UseRouting();

app.UseCors("AllowFrontend");  

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();