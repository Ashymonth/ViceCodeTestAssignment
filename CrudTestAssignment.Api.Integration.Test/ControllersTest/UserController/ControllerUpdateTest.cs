using CrudTestAssignment.DAL.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using CrudTestAssignment.Api.Api.V1.Models;
using Xunit;

namespace CrudTestAssignment.Api.Integration.Test.ControllersTest.UserController
{
    public class ControllerUpdateTest : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;

        public ControllerUpdateTest(WebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task UpdateUser_UserExist_ShouldBe_OK()
        {
            // Arrange
            const string methodToCall = "/api/v1/users";

            var client = _factory.CreateClient();
            var user = await SeedData.CreateUser(client, methodToCall);

            var expected = Guid.NewGuid().ToString();

            // Act
            var updateResponse = await client.PutAsJsonAsync($"{methodToCall}/{user.Id}", new UserModel { Name = expected });
            var actual = await updateResponse.Content.ReadAsAsync<UserModel>();

            //Assert
            updateResponse.EnsureSuccessStatusCode();

            Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
            Assert.NotNull(actual);
            Assert.Equal(expected, actual.Name);

            await client.DeleteAsync($"{methodToCall}/{actual.Id}");
        }

        [Fact]
        public async Task UpdateUser_UserNotExist_ShouldBe_NotFound()
        {
            // Arrange
            var client = _factory.CreateClient();
            const string methodToCall = "/api/v1/users";

            // Act
            var updateResponse = await client.PutAsJsonAsync($"{methodToCall}/0", new UserModel { Name = Guid.NewGuid().ToString() });

            //Assert
            Assert.Equal(HttpStatusCode.NotFound, updateResponse.StatusCode);
        }

        [Fact]
        public async Task UpdateUser_InvalidName_ShouldBe_BadRequest()
        {
            // Arrange
            const string methodToCall = "/api/v1/users";

            var client = _factory.CreateClient();
            var user = await SeedData.CreateUser(client, methodToCall);

            const string expected = "";

            // Act
            var updateResponse = await client.PutAsJsonAsync($"{methodToCall}/{user.Id}", new UserModel { Name = expected });

            //Assert

            Assert.Equal(HttpStatusCode.BadRequest, updateResponse.StatusCode);

            await client.DeleteAsync($"{methodToCall}/{user.Id}");
        }
    }
}