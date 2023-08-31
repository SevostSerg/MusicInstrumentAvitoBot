using AvitoMusicInstrumentsBot;
using Serilog;
using Serilog.Extensions.Logging;
using AvitoBot.Database;
using Microsoft.EntityFrameworkCore;
using Sevost.AvitoBotAPI.Controllers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

var confuguration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

builder.Services.AddDbContext<BotDbContext>(options => options.UseNpgsql(new DBConfig(confuguration).ConnectionString));

builder.Services.AddSingleton<UserController, UserController>();
builder.Services.AddMvc().AddControllersAsServices();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

var serilog = new LoggerConfiguration().MinimumLevel.Information().WriteTo.File("log.txt").CreateLogger();

app.Run();
