using Microsoft.EntityFrameworkCore;

namespace MyControllerWebApi.Models;

public class TodoContext : DbContext
{
 public TodoContext(DbContextOptions<TodoContext> options): base(options)
    {
    }

  //public DbSet<TodoItem> TodoItems { get; set; } = null!;
  public DbSet<TodoItem> TodoItems => Set<TodoItem>();
}
