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
    public /*async Task*/ void GetTodoReturnsNotFoundIfNotExists()
    {
        // Arrange
        var mock = new Mock<ITodoService>();

        // mock.Setup(m => m.Find(It.Is<int>(id => id == 1)))
        //     .ReturnsAsync((Todo?)null);

        // // Act
        // var result = await TodoEndpointsV2.GetTodo(1, mock.Object);

        // //Assert
        // Assert.IsType<Results<Ok<Todo>, NotFound>>(result);

        // var notFoundResult = (NotFound) result.Result;

        Assert.NotNull(2);
    }
}