using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Supplier_Screening_Server.Context;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Get the connectionString of Database to be used for the DbContext EF library
var connectionString = builder.Configuration.GetConnectionString("Connection");
var apiUser = builder.Configuration.GetValue<string>("AppSettings:ApiUser");
var apiPassword = builder.Configuration.GetValue<string>("AppSettings:ApiPassword");
var apiUrl = builder.Configuration.GetValue<string>("AppSettings:ApiUrl");



builder.Services.AddDbContext<APIDBContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddHttpClient();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", policyBuilder =>
    {
        policyBuilder.AllowAnyOrigin()
                     .AllowAnyMethod()
                     .AllowAnyHeader();
    });
});

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new CustomDateTimeConverter("MM/dd/yyyy HH:mm:ss"));
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
