using Xunit;
using MyControllerWebApi.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using UnitTests.Helpers;
using MyControllerWebApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MyControllerWebApi.Services;

namespace TestProject;

public class UnitTestMoq
{

    [Fact]
    public async Task GetTodoReturnsNotFoundIfNotExists()
    {
        // Arrange
        var mock = new Mock<ITodoService>();
        mock.Setup(m => m.GetTodoItem(It.Is<long>(id => id == 1))).ReturnsAsync((TodoItem?)null);
        var controller = new TodoItemsController(mock.Object);

        // Act
        var result = await controller.GetTodoItem(1);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result.Result);
        Assert.True(result.Result is NotFoundObjectResult);
        var notFoundResult = (NotFoundObjectResult) result.Result;
        Assert.NotNull(notFoundResult);
    }


    [Fact]
    public async Task GetAllReturnsTodosFromDatabase()
    {
        // Arrange
        var mock = new Mock<ITodoService>();

        mock.Setup(m => m.GetTodoItems())
            .ReturnsAsync(new List<TodoItem> {
                new TodoItem
                {
                    Id = 1,
                    Name = "Test title 1",
                    IsComplete = false
                },
                new TodoItem
                {
                    Id = 2,
                    Name = "Test title 2",
                    IsComplete = true
                }
            });

        // Act
        var controller = new TodoItemsController(mock.Object);
        // Act
        var result = await controller.GetTodoItems();

        //Assert
        Assert.IsAssignableFrom<ActionResult<IEnumerable<TodoItemDTO>>>(result);
        var okResult = Assert.IsType<OkObjectResult>(result.Result);  // Vérifiez qu'il s'agit d'un OkObjectResult
        var items = Assert.IsAssignableFrom<IEnumerable<TodoItemDTO>>(okResult.Value);  // Vérifiez que le type est correct
        Assert.NotEmpty(items);  // Vérifiez que la liste n'est pas vide


        Assert.Collection(items, todo1 =>
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

}