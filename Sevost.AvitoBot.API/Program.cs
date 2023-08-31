using Microsoft.EntityFrameworkCore;
using Serilog;
using Sevost.AvitoBot.API;
using Sevost.AvitoBot.API.Controllers;
using Sevost.AvitoBot.API.Services;
using Sevost.AvitoBot.Database;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers().AddNewtonsoftJson(options =>
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
); ;

var confuguration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

builder.Services.AddDbContext<BotDbContext>(options => options.UseNpgsql(new DBConfig(confuguration).ConnectionString), ServiceLifetime.Singleton);

builder.Services.AddDataProtection();

builder.Services.AddSingleton<AppConfig>();
builder.Services.AddSingleton<APIService>();

var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

var serilog = new LoggerConfiguration().MinimumLevel.Information().WriteTo.File("log.txt").CreateLogger();

app.Run();
