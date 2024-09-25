using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MyControllerWebApi.Models;
//using MyControllerWebApi.Services;


namespace IntegrationTestProject.Helpers;

public class TestWebApplicationFactory<TProgram>: WebApplicationFactory<TProgram> where TProgram : class
{
    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {   
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<TodoContext>));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }
     services.AddDbContext<TodoContext>(opt =>opt.UseInMemoryDatabase("TodoList"));
            // services.AddDbContext<TodoContext>(options =>
            // {

           
            //     // var path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            //     // options.UseSqlite($"Data Source={Path.Join(path, "WebMinRouteGroup_tests.db")}");
            // });
        });

        return base.CreateHost(builder);
    }
}
