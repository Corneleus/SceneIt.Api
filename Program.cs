using Microsoft.EntityFrameworkCore;
using SceneIt.Api.Data;
using SceneIt.Api.Interfaces;
using SceneIt.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddUserSecrets<Program>(optional: true);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<SceneItDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular",
        policy =>
        {
            policy
                .WithOrigins("http://localhost:4200")
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

builder.Services.Configure<ImportAutomationOptions>(builder.Configuration.GetSection("Imports:Automation"));
builder.Services.Configure<OmdbOptions>(builder.Configuration.GetSection("Omdb"));

builder.Services.AddHttpClient<IOmdbImportClient, OmdbImportClient>(client =>
{
    var baseUrl = builder.Configuration["Omdb:BaseUrl"] ?? "https://www.omdbapi.com/";
    client.BaseAddress = new Uri(baseUrl);
});

builder.Services.AddScoped<IMovieService, MovieService>();
builder.Services.AddScoped<IMovieImportService, MovieImportService>();
builder.Services.AddHostedService<ImportAutomationService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAngular");
app.UseAuthorization();
app.MapControllers();

app.Run();
