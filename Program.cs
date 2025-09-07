using WebAPI.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient<GoogleMapsService>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.MapGet("/", () => "API works");

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/api/swagger/v1/swagger.json", "My API V1");
    c.RoutePrefix = "api/swagger"; 
});

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();
app.Run();
