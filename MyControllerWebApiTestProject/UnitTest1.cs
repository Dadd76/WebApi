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
    //    Assert.IsType<IEnumerable<TodoItemDTO>>(result);
    // ActionResult<IEnumerable<TodoItemDTO>>
    //   Expected: typeof(Microsoft.AspNetCore.Http.HttpResults.Ok<TodoItem[]>)
    //   Actual:   typeof(Microsoft.AspNetCore.Mvc.ActionResult<IEnumerable<TodoItemDTO>>)


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

    // [Fact]
    // public async Task GetTodoReturnsTodoFromDatabase()
    // {
    //     // Arrange
    //     await using var context = new MockDb().CreateDbContext();

    //     context.TodoItems.Add(new TodoItem
    //     {
    //         Id = 2,
    //         Name = "Test title 2",
    //         IsComplete = true
    //     });

    //     await context.SaveChangesAsync();

    //     // Act
    //     var controller = new TodoItemsController(context);
    //     // Act
    //     var result = await controller.GetTodoItems();

    //     //Assert
    //     Assert.IsType<Results<Ok<TodoItem>, NotFound>>(result);

    //    //ActionResult<TodoItemDTO>

    //     var okResult = (Ok<TodoItem>)result.Result;

    //     Assert.NotNull(okResult.Value);
    //     Assert.Equal(1, okResult.Value.Id);
    // }

}