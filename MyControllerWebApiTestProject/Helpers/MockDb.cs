using Microsoft.EntityFrameworkCore;
using MyControllerWebApi.Models;

namespace UnitTests.Helpers;

public class MockDb : IDbContextFactory<TodoContext>
{
    public TodoContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<TodoContext>()
            .UseInMemoryDatabase($"InMemoryTestDb-{DateTime.Now.ToFileTimeUtc()}")
            .Options;

        return new TodoContext(options);
    }
}


