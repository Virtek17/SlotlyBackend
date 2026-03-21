using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Slotly.AutoMapper;
using Slotly.Data;
using Slotly.Entities;
using Slotly.Interfaces;
using Slotly.Repositories;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var services = builder.Services;

services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

/* Database */
string connectionString = builder
    .Configuration
    .GetConnectionString("SlotlyDataBase") ??
    throw new Exception("Not found connection string");

services.AddDbContext<SlotlyContext>(optionsAction:
    options => options.UseSqlServer(connectionString));

services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

// Enum => строка
services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

services.AddAutoMapper(MappingProfile.Configuration);



services.AddCors();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseCors(builder => builder
     .AllowAnyOrigin()
     .AllowAnyMethod()
     .AllowAnyHeader());

app.UseHttpsRedirection();


//app.UseAuthorization();

app.MapControllers();

app.Run();
