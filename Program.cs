using InsuranceAPI.DAL.DBContexts;
using InsuranceAPI.DAL.Repositories.Implementations;
using InsuranceAPI.DAL.Repositories.Interfaces;
using InsuranceAPI.Services.Implementations;
using InsuranceAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddScoped<IInsuranceRepositories, InsuranceRepositories>();
builder.Services.AddTransient<IInsuranceServices, InsuranceServices>();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDBContexts>(options => options.UseSqlServer("name=DefaultConnection"));

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
