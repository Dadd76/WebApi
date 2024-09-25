using IntegrationTestProject.Helpers;
using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationTestProject;

    public class UnitTest1 : IClassFixture<TestWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public UnitTest1()
        {
            var appFactory = new TestWebApplicationFactory<Program>();
            _client = appFactory.CreateClient();
        }

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
    }


