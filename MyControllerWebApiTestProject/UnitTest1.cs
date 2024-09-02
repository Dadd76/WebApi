using Xunit;
using MyControllerWebApi.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using UnitTests.Helpers;
using MyControllerWebApi.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace TestProject;

public class UnitTest1
{

     [Fact]
    public async Task GetTodoReturnsNotFoundIfNotExists()
    {
        // Arrange
        await using var context = new MockDb().CreateDbContext();
        var controller = new TodoItemsController(context);
        
        // Act
        var result = await controller.GetTodoItem(1);
        //Assert
        Assert.IsAssignableFrom<ActionResult<TodoItemDTO>>(result);
      
        //other possible solution
        Assert.True(result.Result is NotFoundResult);
        var notFoundResult = (NotFoundResult) result.Result;
        Assert.NotNull(notFoundResult);
    }

    [Fact]
    public async Task GetAllReturnsTodosFromDatabase()
    {
        // Arrange

        await using var context = new MockDb().CreateDbContext();

        context.TodoItems.Add(new TodoItem
        {
            Id = 1,
            Name = "Test title 1",
            IsComplete = false
        });

        context.TodoItems.Add(new TodoItem
        {
            Id = 2,
            Name = "Test title 2",
            IsComplete = true
        });

        await context.SaveChangesAsync();

        var controller = new TodoItemsController(context);
        // Act
        var result = await controller.GetTodoItems();

        //Assert
        Assert.IsAssignableFrom<ActionResult<IEnumerable<TodoItemDTO>>>(result);
        Assert.NotNull(result.Value);
        Assert.NotEmpty(result.Value);

        Assert.Collection(result.Value, todo1 =>
        {
            Assert.Equal(1, todo1.Id);
            Assert.Equal("Test title 1", todo1.Name);
            Assert.False(todo1.IsComplete);
        }, todo2 =>
        {
            Assert.Equal(2, todo2.Id);
            Assert.Equal("Test title 2", todo2.Name);
            Assert.True(todo2.IsComplete);
        });
    }

    [Fact]
    public async Task GetTodoReturnsTodoFromDatabase()
    {
        // Arrange
        await using var context = new MockDb().CreateDbContext();

        context.TodoItems.Add(new TodoItem
        {
            Id = 2,
            Name = "Test title 2",
            IsComplete = true
        });

        await context.SaveChangesAsync();

        // Act
        var controller = new TodoItemsController(context);
        // Act
        var result = await controller.GetTodoItem(2);
  
        //Assert
        Assert.IsType<ActionResult<TodoItemDTO>>(result);
        Assert.NotNull(result.Value);
        Assert.Equal(2, result.Value.Id);
    }

    [Fact]
    public async Task CreateTodoCreatesTodoInDatabase()
    {
        //Arrange
        await using var context = new MockDb().CreateDbContext();

        var newTodo = new TodoItemDTO
        {
            Id = 2,
            Name = "Test title 2",
            IsComplete = true
        };

        //Act
        var controller = new TodoItemsController(context);
 
        // Act
        var result = await controller.PostTodoItems(newTodo);
        //Assert
        Assert.IsType<ActionResult<TodoItemDTO>>(result);
        
        var createdAtActionResult  = (CreatedAtActionResult)result.Result;
       
        Assert.NotNull(createdAtActionResult);
        Assert.NotNull(createdAtActionResult.Value);

        Assert.NotEmpty(context.TodoItems);
        Assert.Collection(context.TodoItems, todo =>
        {
            Assert.Equal("Test title 2", todo.Name);
            Assert.Equal("secret", todo.Secret);
        });
    }

    [Fact]
    public async Task UpdateTodoUpdatesTodoInDatabase()
    {
        //Arrange
        await using var context = new MockDb().CreateDbContext();

        context.TodoItems.Add(new TodoItem
        {
            Id = 2,
            Name = "Test title 2",
            IsComplete = true
        });

        await context.SaveChangesAsync();

        var updatedTodo = new TodoItemDTO
        {
            Id = 2,
            Name = "Updated Test title 2",
            IsComplete = true
        };

        //Act
        var controller = new TodoItemsController(context);

        //Act
        var result = await controller.PutTodoItems(updatedTodo.Id, updatedTodo);

        //Assert
        Assert.IsType<NoContentResult>(result);


       // var createdResult = (Created<TodoItemDTO>)result;
        var noContentResult  = (NoContentResult)result;
        
        Assert.NotNull(noContentResult);

        //Context
        var todoInDb = await context.TodoItems.FindAsync((long)2);
        Assert.NotNull(todoInDb);
        Assert.Equal("Updated Test title 2", todoInDb!.Name);
        Assert.True(todoInDb.IsComplete);
    }

/*
   var noContentResult = subGenreResult as NoContentResult;

    // Assert
    Assert.False(noContentResult.StatusCode is StatusCodes.Status204NoContent);
*/
}