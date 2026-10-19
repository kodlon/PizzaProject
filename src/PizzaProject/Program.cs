using Microsoft.EntityFrameworkCore;
using PizzaProject.Data;
using PizzaProject.Services;
using PizzaProject.Utils;

var builder = WebApplication.CreateBuilder(args);

// Handles "application/json-patch+json" only; every other request keeps using System.Text.Json.
builder.Services.AddControllers(options =>
{
    options.InputFormatters.Insert(0, JsonPatchInputFormatterFactory.Create());
});

builder.Services.AddDbContext<PizzaContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("Pizza")));

builder.Services.AddScoped<IPizzaService, PizzaService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();

// Exposed so WebApplicationFactory<Program> can reach the entry point from the test assembly.
public partial class Program;
