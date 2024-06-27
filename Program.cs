using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Supplier_Screening_Server.Context;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Get the connectionString of Database to be used for the DbContext EF library
var connectionString = builder.Configuration.GetConnectionString("Connection");
builder.Services.AddDbContext<APIDBContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", policyBuilder =>
    {
        policyBuilder.AllowAnyOrigin()
                     .AllowAnyMethod()
                     .AllowAnyHeader();
    });
});


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseCors("AllowAllOrigins");

app.MapControllers();

app.Run();
