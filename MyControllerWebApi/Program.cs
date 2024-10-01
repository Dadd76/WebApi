using MyControllerWebApi.Models;
using Microsoft.EntityFrameworkCore;
using MyControllerWebApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

// add DB context
builder.Services.AddDbContext<TodoContext>(opt =>
    opt.UseInMemoryDatabase("TodoList"));


// Add a custom scoped service.
builder.Services.AddScoped<ITodoService, TodoService>();

builder.Services.AddHttpClient();
//The method call builder.Services.AddEndpointsApiExplorer() registers services that expose information about our endpoints:
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
///
// Html javascript page
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();


public partial class Program
{ }