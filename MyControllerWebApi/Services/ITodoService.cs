using Microsoft.AspNetCore.Mvc;
using MyControllerWebApi.Models;

namespace MyControllerWebApi.Services;

public interface ITodoService
{
     Task<IEnumerable<TodoItem>> GetTodoItems();
     Task<TodoItem?> GetTodoItem(long id);
     Task<bool> PutTodoItems(long id, TodoItem todo);
     Task<bool> PostTodoItems(TodoItem todo);
     Task<bool> DeleteTodoItems(long id);
}
