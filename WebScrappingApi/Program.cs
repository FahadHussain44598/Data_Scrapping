using Microsoft.AspNetCore.Mvc;
using WebScrappingApi.Routng_Configuration;
using WebScrappingApi.Scrapping_Manager.Ali_Baba_Scrapping_Manager;
using WebScrappingApi.Utilities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IScrapping_Manager, Scrapping_Manager>();
builder.Services.Configure<MvcOptions>(options =>
{
    options.Conventions.Add(new ControllerRouteConvention());
});

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

app.Run();
