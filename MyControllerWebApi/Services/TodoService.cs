using Microsoft.EntityFrameworkCore;
using MyControllerWebApi.Models;

namespace MyControllerWebApi.Services;

public class TodoService : ITodoService
{   private readonly TodoContext _context;

    public TodoService(TodoContext context)
    {
         _context = context;
    }

    public async Task<bool> DeleteTodoItems(long id)
    {
        var todoItems = await _context.TodoItems.FindAsync(id);
        
        if (todoItems == null)
        {
            return false;
        }

        _context.TodoItems.Remove(todoItems);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<TodoItem?> GetTodoItem(long id)
    {
        return await _context.TodoItems.FindAsync(id);
    }

    public async Task<IEnumerable<TodoItem>> GetTodoItems()
    {
        return await _context.TodoItems.ToListAsync();
    }

    public async Task<bool> PostTodoItems(TodoItem todoItem)
    {
        _context.TodoItems.Add(todoItem);

        try
        {
            await _context.SaveChangesAsync();
        }

        catch (Exception)
        {
            return false;
        }
    
        return true;
    }

    public async Task<bool> PutTodoItems(long id, TodoItem todoItem)
    {
        _context.Entry(todoItem).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }

        catch (DbUpdateConcurrencyException)  when(!TodoItemExists(id))
        {
            return false;
        }

        return true;
    }

    private static TodoItemDTO ItemToDTO(TodoItem todoItem) =>
    new TodoItemDTO
    {
        Id = todoItem.Id,
        Name = todoItem.Name,
        IsComplete = todoItem.IsComplete
    };

    private  bool TodoItemExists(long id)
    {
        return _context.TodoItems.Any(e => e.Id == id);
    }
}
