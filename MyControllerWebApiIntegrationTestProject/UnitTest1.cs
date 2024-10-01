using IntegrationTestProject.Helpers;
using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using MyControllerWebApi.Models;

namespace IntegrationTestProject;


    //[Collection("Sequential")]
    //L'attribut [Collection("CollectionName")] associe plusieurs classes de tests à une même "collection".
    //Une collection dans xUnit regroupe plusieurs classes de tests qui partagent des ressources (comme une base de données ou des fichiers).
    public class UnitTest1 : IClassFixture<TestWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly TestWebApplicationFactory<Program> _appFactory;

        public UnitTest1()
        {
            _appFactory = new TestWebApplicationFactory<Program>();
            _client = _appFactory.CreateClient();
        }

        //[Fact]: Représente un test simple qui ne prend pas de paramètre et est exécuté une seule fois.
        //[Theory]: Représente un test qui peut être exécuté plusieurs fois avec différents jeux de données.

        public static IEnumerable<object[]> InvalidTodos => new List<object[]>
        {
            new object[] { new TodoItemDTO { Name = "", Id = 1, IsComplete = false }, "Name is empty" },
            new object[] { new TodoItemDTO { Name = "no", Id = 2, IsComplete = true }, "Name length < 3" }
        };

        [Fact]
        public async Task Test1()
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/TodoItems");

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            Assert.Contains("expectedContent", responseString);
        }
    
    [Fact]
    public async Task PostTodoWithValidParameters()
    {
        using (var scope = _appFactory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetService<TodoContext>();
            if (db != null && db.TodoItems.Any())
            {
                db.TodoItems.RemoveRange(db.TodoItems);
                await db.SaveChangesAsync();
            }
        }

        var response = await _client.PostAsJsonAsync("/api/TodoItems", new TodoItemDTO
        {
            Name = "Test title",
            IsComplete = false,
            Id = 1
        });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var todoItems = await _client.GetFromJsonAsync<List<TodoItemDTO>>("/api/TodoItems");

        Assert.NotNull(todoItems);
        Assert.Single(todoItems);

        Assert.Collection(todoItems, (todo) =>
        {
            Assert.Equal("Test title", todo.Name);
            Assert.Equal(1, todo.Id);
            Assert.False(todo.IsComplete);
        });
    }

    }
