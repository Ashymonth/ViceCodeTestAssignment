using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using CrudTestAssignment.Api.Api.V1.Models;
using Xunit;

namespace CrudTestAssignment.Api.Integration.Test.ControllersTest.UserController
{
    public class ControllerAddTest : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;

        private const string MethodToCall = "/api/v1/users";

        public ControllerAddTest(WebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task CreateUser_WithValidData_ShouldBe_OK()
        {
            // Arrange
            var client = _factory.CreateClient();
            var expected = new UserModel
            {
                Name = Guid.NewGuid().ToString()
            };

            // Act
            var response = await client.PostAsJsonAsync(MethodToCall, expected);
            var actual = await response.Content.ReadAsAsync<UserModel>();

            // Assert
            response.EnsureSuccessStatusCode();
            
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.NotNull(actual);
            Assert.Equal(expected.Name, actual.Name);

            await client.DeleteAsync($"{MethodToCall}/{actual.Id}");
        }

        [Fact]
        public async Task CreateUser_WithInvalidData_ShouldBe_BadRequest()
        {
            // Arrange
            var client = _factory.CreateClient();
            var expected = new UserModel
            {
                Name = null
            };

            // Act
            var response = await client.PostAsJsonAsync(MethodToCall, expected);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}