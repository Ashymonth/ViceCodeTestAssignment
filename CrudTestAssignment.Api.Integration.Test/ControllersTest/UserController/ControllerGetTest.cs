using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using CrudTestAssignment.Api.Api.V1.Models;
using Xunit;

namespace CrudTestAssignment.Api.Integration.Test.ControllersTest.UserController
{
    public class ControllerGetTest : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;

        public ControllerGetTest(WebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task GetUser_UserExist_ShouldBe_OK()
        {
            // Arrange
            var client = _factory.CreateClient();
            const string methodToCall = "/api/v1/users";

            var expected= await SeedData.CreateUser(client,methodToCall);

            //Act
            var response = await client.GetAsync($"{methodToCall}/{expected.Name}");
            var actual = await response.Content.ReadAsAsync<UserModel>();

            //Assert
            response.EnsureSuccessStatusCode();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(actual);
            Assert.Equal(expected.Id, actual.Id);

            await client.DeleteAsync($"{methodToCall}/{expected.Id}");
        }

        [Fact]
        public async Task GetUser_UserExist_ShouldBe_NotFound()
        {
            // Arrange
            var client = _factory.CreateClient();
            var expected = Guid.NewGuid().ToString();

            const string methodToCall = "/api/v1/users";

            //Act
            var response = await client.GetAsync($"{methodToCall}/{expected}");

            //Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}