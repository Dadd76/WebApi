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

        [Fact]
    public async Task GetTodoReturns()
    {
        // Arrange
        var mock = new Mock<ITodoService>();

        mock.Setup(service => service.GetTodoItem(1))
            .ReturnsAsync(new TodoItem
            {
                Id = 1,
                Name = "Test title",
                IsComplete = false
            });

        var controller = new TodoItemsController(mock.Object);

        // Act
        var result = await controller.GetTodoItem(1);

        //Assert
        Assert.IsAssignableFrom<ActionResult<TodoItemDTO>>(result);
        var okResult = Assert.IsType<OkObjectResult>(result.Result);  // Vérifiez qu'il s'agit d'un OkObjectResult
        var okResult2 = (OkObjectResult) result.Result;
        
        Assert.NotNull(okResult2);
    }

    [Fact]
    public async Task CreateTodoCreatesTodoInDatabase()
    {
         //Arrange
        var todos = new List<TodoItem>();

        var newTodo = new TodoItem
        {
                 Id = 1,
                Name = "Test title",
                IsComplete = false
        };

        var newTodoDto = new TodoItemDTO
        {
                 Id = 1,
                Name = "Test title",
                IsComplete = false
        };


        var mock = new Mock<ITodoService>();

        mock.Setup(m => m.PostTodoItems(It.Is<TodoItem>(t=> t.Name == newTodo.Name && t.Id == newTodo.Id && t.IsComplete == newTodo.IsComplete)))          
            .Callback<TodoItem>(todo => todos.Add(todo))
            .ReturnsAsync(true);

        var controller = new TodoItemsController(mock.Object);
        var result = await controller.PostTodoItems(newTodoDto);   

        // Assert

        // Vérifier que le résultat est de type ActionResult<TodoItemDTO>
        var actionResult = Assert.IsType<ActionResult<TodoItemDTO>>(result);

        // Extraire la partie `Result` du `ActionResult`
        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);

        // Vérifier que l'action appelée est correcte
        Assert.Equal(nameof(TodoItemsController.GetTodoItem), createdAtActionResult.ActionName);

        // Vérifier que les valeurs de retour sont correctes
        var returnValue = Assert.IsType<TodoItemDTO>(createdAtActionResult.Value);
        Assert.Equal(1, returnValue.Id);
        Assert.Equal(newTodoDto.Name, returnValue.Name);
    }

    
    [Fact]
    public async Task DeleteTodoCreatesTodoInDatabase()
    {
        var existingTodoTDO = new TodoItemDTO
        {
            Id=1,
            Name= "existingTodoTDO",
            IsComplete=false
        };

        var existingTodo = new TodoItem
        {
            Id=1,
            Name= "existingTodoTDO",
            IsComplete=false
        };


        var todos = new List<TodoItem> { existingTodo };

        var mock = new Mock<ITodoService>();

         mock.Setup(m => m.DeleteTodoItems(It.Is<long>(t=> ((int)t) == existingTodoTDO.Id)))          
            .Callback<long>(id => 
            {
                var todo = todos.FirstOrDefault(t => t.Id == id);
                if (todo != null)
                {
                    todos.Remove(todo); // Supprime le TodoItem de la liste
                }
            })
            .ReturnsAsync(true);

        var controller = new TodoItemsController(mock.Object);
        var result = await controller.DeleteTodoItems(existingTodoTDO.Id);   
        var actionResult = Assert.IsType<NoContentResult>(result);
        Assert.Empty(todos);
    }

[Fact]
public async Task PutTodoItems_ReturnsNoContent_WhenUpdateIsSuccessful()
{
    // Arrange
    var todoUpdateDTO = new TodoItemDTO
    {
        Id = 1,
        Name = "Updated Todo",
        IsComplete = true
    };

    var existingTodoItem = new TodoItem
    {
        Id = 1,
        Name = "Original Todo",
        IsComplete = false
    };

    var mockService = new Mock<ITodoService>();

    // Mock de GetTodoItem pour renvoyer un élément existant
    mockService.Setup(s => s.GetTodoItem(It.IsAny<long>()))
               .ReturnsAsync(existingTodoItem); // Retourne un TodoItem existant

    // Mock de PutTodoItems pour renvoyer true, simulant une mise à jour réussie
    mockService.Setup(s => s.PutTodoItems(It.IsAny<long>(), It.IsAny<TodoItem>()))
               .ReturnsAsync(true);

    var controller = new TodoItemsController(mockService.Object);

    // Act
    var result = await controller.PutTodoItems(1, todoUpdateDTO);

    // Assert
    Assert.IsType<NoContentResult>(result); // Vérifie que NoContent est retourné
}


[Fact]
public async Task PutTodoItems_ReturnsNotFound_WhenUpdateFails()
{
    // Arrange
    var todoUpdateDTO = new TodoItemDTO
    {
        Id = 1,
        Name = "Updated Todo",
        IsComplete = true
    };

    var mockService = new Mock<ITodoService>();

    // Simuler un échec de la méthode PutTodoItems du service
    mockService.Setup(s => s.PutTodoItems(It.IsAny<long>(), It.IsAny<TodoItem>()))
               .ReturnsAsync(false); // La mise à jour a échoué

    var controller = new TodoItemsController(mockService.Object);

    // Act
    var result = await controller.PutTodoItems(1, todoUpdateDTO);

    // Assert
    Assert.IsType<NotFoundResult>(result); // Vérifier que le contrôleur retourne NotFound
}
    // [Fact]
    // public async Task UpdateTodoCreatesTodoInDatabase()
    // {
    //     var TodoTDO = new TodoItemDTO
    //     {
    //         Id=1,
    //         Name= "existingTodoTDO",
    //         IsComplete=false
    //     };

    //     var TodoToUpdate = new TodoItem
    //     {
    //         Id=1,
    //         Name= "existingTodoTDO",
    //         IsComplete=false
    //     };

    //     var TodoUpdate = new TodoItem
    //     {
    //         Id=1,
    //         Name= "existingTodoTDO",
    //         IsComplete=true
    //     };

    //     var todos = new List<TodoItem> { TodoToUpdate };
    //     var mock = new Mock<ITodoService>();

    //    mock.Setup(m => m.TodoItemExists(It.IsAny<long>())).Returns(true);


    //     mock.Setup(m => m.GetTodoItem(It.Is<long>(id => id == 1)))
    //         .ReturnsAsync(TodoToUpdate);

    //      mock.Setup(m => m.PutTodoItems(It.Is<long>(t=> ((int)t) == TodoUpdate.Id),
    //                                     It.Is<TodoItem>(t=> t.Name == TodoUpdate.Name && t.Id == TodoUpdate.Id && t.IsComplete == TodoUpdate.IsComplete)))          
    //         .Callback<long,TodoItem>((id,TodoUpdate) => 
    //         {
    //            TodoToUpdate = TodoUpdate;

    //             // var todo = todos.FirstOrDefault(t => t.Id == id);
    //             // if (todo != null)
    //             // {
    //             //     todo.IsComplete = TodoUpdate.IsComplete;
    //             //     todo.Name= TodoUpdate.Name;
    //             // }
    //         }).ReturnsAsync(true);
    //       //  .Callback<TodoItem>(todo => TodoUpdate = todo)
        

    //     var controller = new TodoItemsController(mock.Object);
    //     var result = await controller.PutTodoItems(TodoTDO.Id,TodoTDO);   
    //     var actionResult = Assert.IsType<NoContentResult>(result);
        
    //     // Act
    //     var result2 = await controller.GetTodoItems();

    //     //Assert
    //     Assert.IsAssignableFrom<ActionResult<IEnumerable<TodoItemDTO>>>(result2);
    //     var okResult = Assert.IsType<OkObjectResult>(result2.Result);  // Vérifiez qu'il s'agit d'un OkObjectResult
    //     var items = Assert.IsAssignableFrom<IEnumerable<TodoItemDTO>>(okResult.Value);  // Vérifiez que le type est correct

    //     Assert.Collection(items, todo1 =>
    //     {
    //         Assert.Equal(1, todo1.Id);
    //         Assert.Equal("Test title 1", todo1.Name);
    //         Assert.False(todo1.IsComplete);
    //     });
    // }

}