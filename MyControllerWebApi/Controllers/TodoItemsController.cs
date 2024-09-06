using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyControllerWebApi.Models;
using MyControllerWebApi.Services;

//GET //PATCH //POST //PUT

namespace MyControllerWebApi.Controllers;

    [Route("api/[controller]")]
    [ApiController]
    public class TodoItemsController : ControllerBase
    {
        private readonly ITodoService _todoService;
        public TodoItemsController(ITodoService todoService)
        {
            _todoService = todoService;
        }

        // GET: api/TodoItems
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoItemDTO>>> GetTodoItems()
        {
            // _context.TodoItems.Select(x => ItemToDTO(x)).ToListAsync();;
            var result = await _todoService.GetTodoItems();
               // _context.TodoItems.Select(x => ItemToDTO(x)).ToListAsync();;
            
            return  Ok(result.Select(x => ItemToDTO(x)));

//Cannot implicitly convert type 'System.Collections.Generic.IEnumerable<MyControllerWebApi.Models.TodoItemDTO>'
// to 'Microsoft.AspNetCore.Mvc.ActionResult<System.Collections.Generic.IEnumerable<MyControllerWebApi.Models

        }

        // GET: api/TodoItems/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TodoItemDTO>> GetTodoItem(long id)
        {
        var todoItem = await _todoService.GetTodoItem(id);
        
        if (todoItem != null)
             return  Ok(ItemToDTO(todoItem));
        
        else
            return NotFound("todoItem not found");
        }

        // PUT: api/TodoItems/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTodoItems(long id, TodoItemDTO todoDTO)
        {
            if (id != todoDTO.Id)
                return BadRequest();

            var todoItem = await _todoService.GetTodoItem(id);

            if (todoItem == null)
                return NotFound();

            todoItem.Name = todoDTO.Name;
            todoItem.IsComplete = todoDTO.IsComplete;
         
            if(!await _todoService.PutTodoItems(id, todoItem))
                return NotFound();
            
            else 
                return NoContent();
        }

        // POST: api/TodoItems
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<TodoItemDTO>> PostTodoItems(TodoItemDTO todoDTO)
        {
            var todoItem = new TodoItem
            {
               IsComplete = todoDTO.IsComplete,
               Name = todoDTO.Name,
               Secret = "secret"
            };

            await _todoService.PostTodoItems(todoItem);

            return CreatedAtAction(nameof(GetTodoItem),new { id = todoItem.Id },ItemToDTO(todoItem));
           // return CreatedAtAction("GetTodoItems", new { id = todoItems.Id }, todoItems);
        }

        // DELETE: api/TodoItems/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodoItems(long id)
        {
            if(!await _todoService.DeleteTodoItems(id))
                return NotFound();   

            return NoContent();
        }

        private static TodoItemDTO ItemToDTO(TodoItem todoItem) =>
            new TodoItemDTO
            {
                Id = todoItem.Id,
                Name = todoItem.Name,
                IsComplete = todoItem.IsComplete
            };
    }
